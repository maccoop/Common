using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static SingletonBehaviour<T> _singleton;
    public static T Singleton
    {
        get
        {
            if (_singleton == null)
            {
                var obj = new GameObject(typeof(T).Name);
                _singleton = obj.AddComponent<T>();
                _singleton.Init();
            }
            return _singleton.GetComponent<T>();
        }
    }

    public virtual void Awake()
    {
        _singleton = this;
    }

    protected abstract void Init();
}
