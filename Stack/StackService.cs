using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class StackService<T>
{
    public ILog Debug;
    private bool _autoPop;
    private bool _isPop = false;
    private Stack<T> _stack;
    public UnityEvent<T> OnPop;
    T current;

    public bool AutomationPop { set => _autoPop = value; }

    public StackService()
    {
        OnPop = new UnityEvent<T>();
        _stack = new Stack<T>();
        _autoPop= true;
    }

    public void Push(T obj)
    {
        _stack.Push(obj);
        if (_autoPop)
            StartStack();
    }

    private void StartStack()
    {
        if (_isPop)
        {
            Debug.Log("Queue is not free");
            return;
        }
        if (_stack.Count == 0)
        {
            _isPop = false;
            return;
        }
        _isPop = true;
        current = _stack.Pop();
        OnPop.Invoke(current);
    }

    public void EndQueue()
    {
        current = default(T);
        _isPop = false;
        if (_autoPop)
            StartStack();
    }

    public void RemoveAll()
    {
        _stack = new Stack<T>();
        _isPop = false;
    }

    public void Pop()
    {
        if (_stack.Count == 0)
        {
            _isPop = false;
            return;
        }
        current = _stack.Pop();
        OnPop.Invoke(current);
    }
}
