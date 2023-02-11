using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppTimer : MonoBehaviour
{
    public UnityEvent OnCompleted = new UnityEvent();
    public UnityEvent OnStart = new UnityEvent();
    public UnityEvent OnStick = new UnityEvent();
    public double Elapse => elapse;
    public double ElapsePercent => elapse / duration;
    public double Remaining => duration - elapse;
    public double RemainingPercent => (duration - elapse) / duration;

    private double elapse;

    private string key;

    private double duration;

    private static Dictionary<string, AppTimer> timerDict = new Dictionary<string, AppTimer>();

    public static AppTimer Start(double duration, string key)
    {
        if (timerDict.ContainsKey(key))
        {
            Cancle(key);
        }
        var obj = new GameObject();
        obj.name = "Timer " + key;
        var time = obj.AddComponent<AppTimer>();
        time.duration = duration;
        time.StartCountDown();
        time.key = key;
        timerDict.Add(key, time);
        return time;
    }

    private static void Stop(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            time.StopAllCoroutines();
            time.OnCompleted?.Invoke();
            Destroy(time.gameObject);
            timerDict.Remove(key);
        }
    }

    public static void Cancle(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            time.StopAllCoroutines();
            //time.OnCompleted?.Invoke();
            Destroy(time.gameObject);
            timerDict.Remove(key);
        }
    }

    private void StartCountDown()
    {
        OnStart?.Invoke();
        StartCoroutine(CountDown(this.duration));
    }

    private IEnumerator CountDown(double duration)
    {
        while (elapse <= duration)
        {
            elapse += Time.deltaTime;
            OnStick?.Invoke();
            yield return new WaitForFixedUpdate();
        }
        Stop(key);
    }

    public static double GetElapse(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.Elapse;
        }
        return 0;
    }
    public static double GetRemaining(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.Remaining;
        }
        return 0;
    }
    public static double GetElapsePercent(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.ElapsePercent;
        }
        return 0;
    }
    public static double GetRemainingPercent(string key)
    {
        if (timerDict.ContainsKey(key))
        {
            var time = timerDict[key];
            return time.RemainingPercent;
        }
        return 0;
    }
}
