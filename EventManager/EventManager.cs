using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Newtonsoft.Json;
using System;

public delegate void OnEventReceive<T>(T result, Guid guid);

public class EventManager : SingletonBehaviour<EventManager>
{
    private Dictionary<string, OnEventReceive<object>> _dictEvent;
    private ILog Debug = new CustomLog();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="action"></param>
    public void StartListener(string key, OnEventReceive<object> action)
    {
        if (!_dictEvent.ContainsKey(key))
        {
            _dictEvent.Add(key, new OnEventReceive<object>(action));
        }
        else
        {
            _dictEvent[key] += action;
        }
        Debug?.Log($"Start listener: {key}");
    }

    public void StopListener(string key, OnEventReceive<object> action)
    {
        if (!_dictEvent.ContainsKey(key))
        {
            Debug?.Log($"No event as key: {key}");
            return;
        }
        _dictEvent[key] -= (action);
        Debug?.Log($"Stop listener: {key}");
        Debug?.Log($"Event {key} has amount listener: {_dictEvent[key].GetInvocationList().Length}");
    }

    public void SendEvent(string key, object value, Guid guid)
    {
        if (!_dictEvent.ContainsKey(key))
        {
            Debug.Log($"No event as key: {key}");
            return;
        }
        Debug?.Log($"Send event {key} with value: " + JsonConvert.SerializeObject(value));
        _dictEvent[key].Invoke(value, guid);
    }

    protected override void Init()
    {
        _dictEvent = new Dictionary<string, OnEventReceive<object>>();
        DontDestroyOnLoad(gameObject);
    }
}
