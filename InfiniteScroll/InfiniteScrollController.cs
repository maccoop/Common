using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Debug = SangCustomLog;

[System.Serializable]
public enum InfiniteScrollDirection
{
    Vertical, Horizontal
}

[System.Serializable]
public class InfiniteContentPadding
{
    [SerializeField] private int _top, _down, _left, _right;

    public int Top { get => _top; set => _top = value; }
    public int Down { get => _down; set => _down = value; }
    public int Left { get => _left; set => _left = value; }
    public int Right { get => _right; set => _right = value; }
}

public class InfiniteScrollController : MonoBehaviour
{
    private const int VARIABLE = 50;

    [SerializeField] private ScrollRect _scrollrect;
    [SerializeField] private AbstractLazyLoadItem _prefab;
    [SerializeField] private InfiniteScrollDirection _direction;
    [SerializeField] private InfiniteContentPadding _padding;
    [SerializeField] private int _spacing;


    private RectTransform _viewport;
    private RectTransform _content;
    private Scrollbar _scrollbarVertical;
    private Scrollbar _scrollbarHorizontal;

    private List<AbstractLazyLoadItem> _items;
    private Dictionary<object, AbstractLazyLoadItem> _objects;
    private Dictionary<object, string> _data;
    private Dictionary<object, (int position, int size)> _itemSize;
    private int _currentPosition;
    private int indexPosition
    {
        get
        {
            switch (_direction)
            {
                case InfiniteScrollDirection.Vertical:
                    return -1;
                case InfiniteScrollDirection.Horizontal:
                    return 1;
            }
            return 1;
        }
    }
    private bool isReload = false;

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        _viewport = _scrollrect.viewport;
        _content = _scrollrect.content;
        _scrollbarHorizontal = _scrollrect.horizontalScrollbar;
        _scrollbarVertical = _scrollrect.verticalScrollbar;
        _scrollbarVertical.onValueChanged.AddListener(OnChangeScrollBar);
        _scrollbarHorizontal.onValueChanged.AddListener(OnChangeScrollBar);
        switch (_direction)
        {
            case InfiniteScrollDirection.Vertical:
                {
                    _content.anchorMin = new Vector2(0, 1);
                    _content.anchorMax = new Vector2(1, 1);
                    _currentPosition = indexPosition * _padding.Top;
                    _scrollrect.vertical = true;
                    _scrollrect.horizontal = false;
                    _viewport.anchorMin = new Vector2(0.5f, 1);
                    _viewport.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case InfiniteScrollDirection.Horizontal:
                {
                    _content.anchorMin = new Vector2(0, 0.5f);
                    _content.anchorMax = new Vector2(0, 0.5f);
                    _currentPosition = indexPosition * _padding.Left;
                    _scrollrect.vertical = false;
                    _scrollrect.horizontal = true;
                    _viewport.anchorMin = _content.anchorMin;
                    _viewport.anchorMax = _content.anchorMax;
                    break;
                }
        }
    }

    public void Init()
    {
        _objects = new Dictionary<object, AbstractLazyLoadItem>();
        _itemSize = new Dictionary<object, (int, int)>();
        _data = new Dictionary<object, string>();
        _items = new List<AbstractLazyLoadItem>();
    }

    public void AddItem(object index, string data)
    {
        if (_data.ContainsKey(index))
        {
            Debug.LogWarning($"Item with index[{index}] was set data!");
        }
        else
        {
            _data.Add(index, data);
            _objects.Add(index, null);
        }
    }

    public void UpdateItem(object index, string data)
    {
        if (_data.ContainsKey(index))
        {
            Debug.LogWarning($"Item with index[{index}] was update data!");
            _data[index] = data;
        }
    }

    public void AddHeight(object index, int size)
    {
        if (_itemSize.ContainsKey(index))
        {
            Debug.LogWarning($"Item with index[{index}] was set heigt!");
            return;
        }
        _itemSize.Add(index, (_currentPosition, size));
        _currentPosition += indexPosition * size;
        ChangeSizeContent(size);
    }

    public void RefreshItemList()
    {
        if (isReload)
        {
            foreach (var e in _items)
            {
                e.ActiveSelf = false;
            }
            isReload = false;
        }
        foreach (var e in _data)
        {
            if (IsViewportBound(e.Key))
            {
                DisplayItem(e.Key);
            }
            else
            {
                RemoveReferenceoObject(e.Key);
            }
        }
    }

    public void RemoveItem(object index)
    {
        _data.Remove(index);
        _objects.Remove(index);

        /// Calculate size of item
        var size = _itemSize[index];
        var lastPosition = size.position;
        var currentIndex = _itemSize.Keys.ToList().IndexOf(index);
        for (int i = currentIndex + 1; i < _itemSize.Count; i++)
        {
            var currentKey = _itemSize.Keys.ToList()[i];
            var curentSize = _itemSize[currentKey].size;
            _itemSize[currentKey] = (lastPosition, curentSize);
            lastPosition += curentSize + _spacing;
        }
        _currentPosition -= indexPosition * size.size + _spacing;
        ///
        _itemSize.Remove(index);
        RefreshItemList();
    }

    private void OnChangeScrollBar(float value)
    {
        if (isReload)
        {
            return;
        }
        RefreshItemList();
    }

    private void ChangeSizeContent(int height)
    {
        _currentPosition += indexPosition * _spacing;
        switch (_direction)
        {
            case InfiniteScrollDirection.Vertical:
                {
                    _content.sizeDelta = new Vector2(0, Mathf.Abs(_currentPosition) + _padding.Down);
                    break;
                }
            case InfiniteScrollDirection.Horizontal:
                {
                    _content.sizeDelta = new Vector2(Mathf.Abs(_currentPosition) + _padding.Right, 0);
                    break;
                }
        }
    }

    private void RemoveReferenceoObject(object index)
    {
        if (_objects.ContainsKey(index))
        {
            if (_objects[index] != null)
            {
                _objects[index].ActiveSelf = false;
                _objects[index] = null;
            }
        }
        else
        {
            throw new NullReferenceException($"Object with index[{index}]");
        }
    }

    private void DisplayItem(object index)
    {
        var item = GetItemObject(index);
        item.ActiveSelf = true;
        ChangeSizeAndPositionItem(index, item);
        item.SetData(_data[index]);
        _objects[index] = item;
    }

    private void ChangeSizeAndPositionItem(object index, AbstractLazyLoadItem item)
    {
        var csize = _itemSize[index];
        Vector3 itemSize = Vector3.zero;
        Vector3 itemPosition = Vector3.zero;
        switch (_direction)
        {
            case InfiniteScrollDirection.Vertical:
                {
                    itemSize = new Vector3(_viewport.sizeDelta.x - (_padding.Left + _padding.Right), csize.size);
                    itemPosition = new Vector3(_padding.Left, csize.position);
                    break;
                }
            case InfiniteScrollDirection.Horizontal:
                {
                    itemSize = new Vector3(csize.size, _viewport.sizeDelta.y - (_padding.Top + _padding.Down));
                    itemPosition = new Vector3(csize.position, -_padding.Top);
                    break;
                }
        }
        item.RectTransfrom.sizeDelta = itemSize;
        item.RectTransfrom.anchoredPosition = itemPosition;
    }

    private AbstractLazyLoadItem GetItemObject(object index)
    {
        if (_objects.ContainsKey(index))
        {
            if (_objects[index] != null)
            {
                return _objects[index];
            }
        }
        else
        {
            throw new NullReferenceException($"Object not set with index[{index}]");
        }
        var item = _items.Find(x => x.ActiveSelf == false);
        if (item != null)
        {
            return item;
        }
        //Debug.Log("Instantiate new prefab...");
        var obj = Instantiate(_prefab, _content);
        var rect = obj.GetComponent<RectTransform>();
        SetupItemRectranfrom(rect);
        _items.Add(obj.GetComponent<AbstractLazyLoadItem>());
        return obj.GetComponent<AbstractLazyLoadItem>();
    }

    public void Reload(LazyReloadStatus status)
    {
        if (status == LazyReloadStatus.Success)
        {
            return;
        }
        _objects = new Dictionary<object, AbstractLazyLoadItem>();
        _itemSize = new Dictionary<object, (int, int)>();
        _data = new Dictionary<object, string>();
        switch (_direction)
        {
            case InfiniteScrollDirection.Vertical:
                {
                    _currentPosition = indexPosition * _padding.Top;
                    break;
                }
            case InfiniteScrollDirection.Horizontal:
                {
                    _currentPosition = indexPosition * _padding.Left;
                    break;
                }
        }
        isReload = true;
    }

    private void SetupItemRectranfrom(RectTransform rect)
    {
        switch (_direction)
        {
            case InfiniteScrollDirection.Vertical:
                {
                    rect.pivot = new Vector2(0, 1);
                    rect.anchorMin = new Vector2(0, 1);
                    rect.anchorMax = new Vector2(0, 1);
                    break;
                }
            case InfiniteScrollDirection.Horizontal:
                {
                    rect.pivot = new Vector2(0, 1);
                    rect.anchorMin = new Vector2(0, 1f);
                    rect.anchorMax = new Vector2(0, 1f);
                    break;
                }
        }
    }

    private bool IsViewportBound(object index)
    {
        var item = _itemSize[index];
        float min = 0, max = 0;
        switch (_direction)
        {
            case InfiniteScrollDirection.Vertical:
                {
                    min = _content.anchoredPosition.y + item.position - item.size;
                    max = _content.anchoredPosition.y + item.position;
                    return min < MinPosition && max > MaxPosition;
                }
            case InfiniteScrollDirection.Horizontal:
                {
                    min = _content.anchoredPosition.x + item.position + item.size;
                    max = _content.anchoredPosition.x + item.position;
                    return min > MinPosition && max < MaxPosition;
                }
        }
        return false;
    }

    private int MinPosition
    {
        get
        {
            switch (_direction)
            {
                case InfiniteScrollDirection.Vertical:
                    {
                        return 0 + VARIABLE;
                    }
                case InfiniteScrollDirection.Horizontal:
                    {
                        return 0 - VARIABLE;
                    }
            }
            return 0;
        }
    }

    private float MaxPosition
    {
        get
        {
            switch (_direction)
            {
                case InfiniteScrollDirection.Vertical:
                    {
                        return -(_viewport.sizeDelta.y + VARIABLE);
                    }
                case InfiniteScrollDirection.Horizontal:
                    {
                        return (_viewport.sizeDelta.x + VARIABLE);
                    }
            }
            return -(_viewport.sizeDelta.y + VARIABLE);
        }
    }
}
