using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using System;

public class DragHandle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler //IPointerMoveHandler
{
    #region SerializeField
    [FoldoutGroup("Direction"), SerializeField]
    private bool _vertical;
    [FoldoutGroup("Direction"), SerializeField]
    private bool _horizontal;
    [BoxGroup("Config"), SerializeField]
    private float _speed = 1000;
    [BoxGroup("Config"), SerializeField]
    private bool _isUi = true;
    [BoxGroup("Config"), SerializeField]
    private bool _returnToBeginPosition;
    [BoxGroup("Config"), SerializeField]
    private float _minDistanceCheckingGesture = 50;
    [BoxGroup("Config"), SerializeField]
    private Vector2 _minPosition, _maxPosition;
    [BoxGroup("Config"), SerializeField]
    private bool _checkOutBox = true;
    #endregion

    #region Events
    public Action OnDropEvent;
    public Action OnBeginDragEvent;
    public Action OnDragEvent;
    public UnityEvent<GestureDirection> OnGestureDirectionEvent;
    #endregion

    #region Private
    private RectTransform _rectTranform;
    private Vector3 _beginObjectPosition;
    private Vector3 _beginObjectMovePosition;
    private Vector3 _deltaBeginPosition;
    private Vector3 _targetPosition;
    private bool _isDrag = false;
    private bool _isAuto = false;
    private bool _active = false;
    private bool _hover = false;
    private float _canvasScale;
    #endregion

    private void Awake()
    {
        _beginObjectPosition = transform.localPosition;
    }

    private void Start()
    {
        OnDragEvent += () => { };
        OnBeginDragEvent += () => { };
        OnDropEvent += () => { };
        _canvasScale = GetComponentInParent<Canvas>().GetComponent<RectTransform>().localScale.x;
    }

    private void Update()
    {
        if (_isAuto)
        {
            Vector3 target = GetTargetPosition();
            transform.localPosition = target;
            _isAuto = false;
        }
        else if (_isDrag)
        {
            var delta = transform.localPosition - _beginObjectMovePosition;
            CheckingGesture(delta);
        }
#if UNITY_ANDROID
#else
        
#endif
        CheckingOutGroup();
    }

    internal void SetTarget(Transform target)
    {
        _isAuto = true;
        _targetPosition = target.localPosition;
    }

    private void OnDisable()
    {
        _isDrag = false;
    }

    private void CheckingOutGroup()
    {
        if (!_checkOutBox)
            return;
        if (_horizontal)
        {
            if (Position.x < _minPosition.x)
            {
                Position = new Vector3(_minPosition.x, Position.y, Position.z);
            }
            else if (Position.x > _maxPosition.x)
            {
                Position = new Vector3(_maxPosition.x, Position.y, Position.z);
            }
        }
        if (_vertical)
        {
            if (Position.y < _minPosition.y)
            {
                Position = new Vector3(Position.x, _minPosition.y, Position.z);
            }
            else if (Position.y > _maxPosition.y)
            {
                Position = new Vector3(Position.x, _maxPosition.y, Position.z);
            }
        }
    }

    private void CheckingGesture(Vector3 delta)
    {
        if (Mathf.Abs(delta.x) > _minDistanceCheckingGesture)
        {
            if (delta.x > 0)
            {
                OnGestureDirectionEvent.Invoke(GestureDirection.Right);
            }
            else
            {
                OnGestureDirectionEvent.Invoke(GestureDirection.Left);
            }
        }
        else if (Mathf.Abs(delta.y) > _minDistanceCheckingGesture)
        {
            if (delta.y > 0)
            {
                OnGestureDirectionEvent.Invoke(GestureDirection.Top);
            }
            else
            {
                OnGestureDirectionEvent.Invoke(GestureDirection.Down);
            }
        }
    }

    private Vector3 GetTargetPosition()
    {
        var target = _targetPosition;
        if (!_horizontal)
        {
            target.x = transform.localPosition.x;
        }
        if (!_vertical)
        {
            target.y = transform.localPosition.y;
        }
        target.z = transform.localPosition.z;

        return target;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hover = false;
        OnPointerUp(eventData);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (_isDrag)
        {
            var currentPosition = (Vector3)eventData.position / _canvasScale;
            _targetPosition = currentPosition + _deltaBeginPosition;
            transform.localPosition = _targetPosition;
            OnDragEvent.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDrag = false;
        if (_returnToBeginPosition)
        {
            transform.localPosition = _beginObjectPosition;
        }
        if (eventData != null)
            OnDropEvent.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_hover)
        {
            _isDrag = true;
            transform.SetAsLastSibling();
            _beginObjectMovePosition = transform.localPosition;
            var _beginPointPosition = (Vector3)eventData.position / _canvasScale;
            _deltaBeginPosition = transform.localPosition - _beginPointPosition;
            OnBeginDragEvent.Invoke();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDrag)
        {
            var currentPosition = (Vector3)eventData.position / _canvasScale;
            _targetPosition = currentPosition + _deltaBeginPosition;
            transform.localPosition = _targetPosition;
            OnDragEvent.Invoke();
        }
    }

    public Vector3 Position
    {
        get
        {
            if (_isUi)
            {
                return CurrentRectTransform.anchoredPosition;
            }
            else
            {
                return transform.localPosition;
            }
        }
        set
        {
            if (_isUi)
            {
                CurrentRectTransform.anchoredPosition = value;
            }
            else
            {
                transform.localPosition = value;
            }
        }
    }

    RectTransform CurrentRectTransform
    {
        get
        {
            if (_rectTranform == null)
            {
                _rectTranform = GetComponent<RectTransform>();
            }
            return _rectTranform;
        }
    }

    public bool ReturnToBeginPosition { get => _returnToBeginPosition; set => _returnToBeginPosition = value; }
    public bool Active { get => _active; set => _active = value; }
    public bool CheckOutBox { get => _checkOutBox; set => _checkOutBox = value; }
}

[System.Serializable]
public enum GestureDirection
{
    Left, Right, Top, Down
}
