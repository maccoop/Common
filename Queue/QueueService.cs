using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class QueueService<T>
{
    public ILog Debug;
    public int timeDelay => 0;
    private bool isDequeue = false;
    private Queue<T> queue;
    public UnityEvent<T> OnDequeue;
    T current;

    public QueueService()
    {
        OnDequeue = new UnityEvent<T>();
        queue = new Queue<T>();
    }

    public void AddQueue(T obj)
    {
        queue.Enqueue(obj);
        StartQueue();
    }

    private void StartQueue()
    {
        if (isDequeue)
        {
            Debug.Log("Queue is not free");
            return;
        }
        if (queue.Count == 0)
        {
            isDequeue = false;
            return;
        }
        isDequeue = true;
        current = queue.Dequeue();
        OnDequeue.Invoke(current);
    }

    public void EndQueue()
    {
        current = default(T);
        isDequeue = false;
        StartQueue();
    }

    public void RemoveAll()
    {
        queue = new Queue<T>();
        isDequeue = false;
    }
}
