using System;
using UnityEngine.Events;

[System.Serializable]
public class InAppEvent
{
    private UnityEvent<object> _eventReceiveData;
    private ILog Debug;
    private string _eventKey;
    private Guid _guid;

    public static InAppEvent Init(string key, bool hasListener = true)
    {
        var u = new InAppEvent(key, hasListener);
        return u;
    }

    private InAppEvent(string key, bool hasListener = true)
    {
        _eventKey = key;
        _guid = Guid.NewGuid();
        _eventReceiveData = new UnityEvent<object>();
        Debug = new SangLog4();
        if (hasListener)
            EventManager.Singleton.StartListener(_eventKey, OnEventReceive);
    }

    private void OnEventReceive(object data, Guid guid)
    {
        if (guid == _guid)
            return;
        Debug?.Log($"['{_eventKey}']Receive data: " + data);
        _eventReceiveData.Invoke(data);
        return;
    }

    public void SendEvent(string key, object data)
    {
        EventManager.Singleton.SendEvent(key, data, _guid);
    }

    public void SendEvent(object data)
    {
        EventManager.Singleton.SendEvent(_eventKey, data, _guid);
    }
    public UnityEvent<object> OnEventReceiveData => _eventReceiveData;
    public ILog Log
    {
        set
        {
            Debug = value;
        }
    }

}
