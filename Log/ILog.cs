using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILog
{
    public void Log(object message);
    public void LogWarning(object message);
    public void LogError(object message);
}
