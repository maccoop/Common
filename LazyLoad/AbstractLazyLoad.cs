using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public enum LazyReloadStatus
{
    Begin, Success
}

public abstract class AbstractLazyLoad : MonoBehaviour
{
    private float VALUE_TO_RELOAD_VERTICAL => 1 + 30f / ScrollRect.content.sizeDelta.y;
    private float VALUE_TO_RELOAD_HORIZONTAL => 0 - 30f / ScrollRect.content.sizeDelta.y;

    private protected QueueService<int> queue;
    private protected UnityEvent<int, string> _eventReceiveData;
    private protected UnityEvent<LazyReloadStatus> _eventReloadData;
    private protected TextMeshProUGUI textReload, textLoading;
    private bool isLoad = false;
    private bool mustReload;
    private ILog Debug = new CustomLog();

    private void Awake()
    {
        queue = new QueueService<int>();
        queue.OnDequeue.AddListener(OnDequeue);
        _eventReceiveData = new UnityEvent<int, string>();
        _eventReceiveData.AddListener(OnReceiveData);
        _eventReloadData = new UnityEvent<LazyReloadStatus>();

    }

    public virtual void Start()
    {
        ScrollbarVertical.onValueChanged.AddListener(OnVerticalScrollbarChange);
        ScrollbarHorizontal.onValueChanged.AddListener(OnHorizotalScrollbarChange);
        InitTextLoadingData();
        InitTextReload();
        Init();
    }

    private void InitTextReload()
    {
        var gameobject = new GameObject();
        gameobject.name = "TextLoadingData";
        gameobject.transform.parent = ScrollRect.content;
        var rect = gameobject.AddComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(0, 30);
        rect.anchoredPosition = new Vector2(0, 30);
        var text = gameobject.AddComponent<TMPro.TextMeshProUGUI>();
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.text = TextReload;
        text.font = TitleFont;
        text.fontSize = 36;
        text.color = Color.black;
        text.transform.localScale = new Vector3(1, 1, 1);
        textReload = text;
    }

    private void InitTextLoadingData()
    {
        var gameobject = new GameObject();
        gameobject.name = "TextReload";
        gameobject.transform.parent = ScrollRect.content;
        var rect = gameobject.AddComponent<RectTransform>();
        rect.pivot = new Vector2(0.5f, 1);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.sizeDelta = new Vector2(0, 30);
        rect.anchoredPosition = new Vector2(0, -30);
        var text = gameobject.AddComponent<TMPro.TextMeshProUGUI>();
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.text = TextLoadingData;
        text.font = TitleFont;
        text.fontSize = 36;
        text.color = Color.black;
        text.transform.localScale = new Vector3(1, 1, 1);
        textLoading = text;
    }

    #region Private Method

    private void OnDequeue(int page)
    {
        if (ListPage.ContainsKey(page))
        {
            OnChangePage(page);
            return;
        }
        SendRequestGetData(page);
    }

    private void OnHorizotalScrollbarChange(float value)
    {
        if (value >= 1 && !isLoad)
        {
            NextPage();
        }
        if (value <= VALUE_TO_RELOAD_HORIZONTAL)
        {
            TextReload = TextReload;
            mustReload = true;
        }
        else if (mustReload)
        {
            mustReload = false;
            ReloadPage();
        }
    }

    private void OnVerticalScrollbarChange(float value)
    {

        if (value <= 0 && !isLoad)
        {
            NextPage();
        }
        if (value >= VALUE_TO_RELOAD_VERTICAL)
        {
            TextReload = TextReload;
            mustReload = true;
        }
        else if (mustReload)
        {
            mustReload = false;
            ReloadPage();
        }
    }

    protected void NextPage()
    {
        if (isLoad)
            return;
        if (CurrentPage >= MaxPage)
        {
            Debug.Log("Current page is larget: " + CurrentPage + ":" + MaxPage);
            EventNullData.Invoke(MaxPage);
            return;
        }
        CurrentPage++;
        OnChangePage(CurrentPage);
    }

    [ContextMenu("Reload")]
    public void ReloadPage()
    {
        TextReload = TextReload;
        ScrollRect.verticalNormalizedPosition = 1;
        Debug.Log("Reload........");
        queue?.RemoveAll();
        ListPage = new Dictionary<int, string>();
        CurrentPage = 0;
        isLoad = false;
        _eventReloadData.Invoke(LazyReloadStatus.Begin);
        OnChangePage(CurrentPage);
    }

    private protected void OnChangePage(int page)
    {
        if (!ListPage.ContainsKey(page))
        {
            isLoad = true;
            Debug.Log($"Add {page} to queue get data");
            AddQueueGetData(page);
            return;
        }
        if (page != CurrentPage)
        {
            Debug.Log("page khac voi current page!!");
            if (page < CurrentPage)
            {
                OnChangePage(CurrentPage);
            }
            return; // Load other CurrentPage
        }
        if (page + 1 < MaxPage && !ListPage.ContainsKey(page + 1))
        {
            AddQueueGetData(page + 1);
        }
        Display(ListPage[page]);
        queue.EndQueue();
        isLoad = false;
        if (mustReload)
        {
            mustReload = false;
            _eventReloadData.Invoke(LazyReloadStatus.Success);
        }
        TextReload = "";
    }

    private void AddQueueGetData(int v)
    {
        queue.AddQueue(v);
    }

    protected void OnReceiveData(int page, string jsonBody)
    {
        if (ListPage.ContainsKey(page))
        {
            ListPage[page] = jsonBody;
        }
        else
        {
            ListPage.Add(page, jsonBody);
        }
        OnChangePage(page);
    }

    protected void OnReceiveError(int page, string jsonBody)
    {
        OnChangePage(page);
    }

    #endregion

    #region Properties

    public abstract void Init();
    public abstract void Display(string jsonBody);
    public abstract void SendRequestGetData(int page);
    public abstract string TextReload { get; set; }
    public abstract string TextLoadingData { get; set; }
    public abstract int MaxPage { get; }
    public abstract int CurrentPage { get; set; }
    public abstract TMPro.TMP_FontAsset TitleFont { get; }
    public abstract Dictionary<int, string> ListPage { get; set; }
    public abstract ScrollRect ScrollRect { get; set; }
    public abstract Scrollbar ScrollbarVertical { get; }
    public abstract Scrollbar ScrollbarHorizontal { get; }
    public abstract UnityEvent<int> EventNullData { get; }
    /// <summary>
    /// Require event invoke when receive data from server
    /// </summary>
    public UnityEvent<int, string> EventReceiveData { get => _eventReceiveData; }
    public UnityEvent<LazyReloadStatus> EventReloadData { get => _eventReloadData; }

    #endregion
}
