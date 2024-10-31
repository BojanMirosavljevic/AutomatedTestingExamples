using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QueueSystem : PersistentSingleton<QueueSystem>
{
    public List<QueueItem> Items;
    
    protected override void Awake()
    {
        base.Awake();
        Items = new List<QueueItem>();
    }

    public void Enqueue(QueueItem item)
    {
        Items.Add(item);
    }

    public void RunNext()
    {
        if (Items.Count > 0)
        {
            QueueItem item = Items[0];
            Items.RemoveAt(0);
            item.Run();
        }
    }
}

public abstract class QueueItem
{
    public Action onFinishCallback;

    public QueueItem()
    {
        onFinishCallback = QueueSystem.Instance.RunNext;
    }

    public QueueItem(Action callback)
    {
        onFinishCallback = callback;
    }

    public virtual void Run()
    {
        onFinishCallback?.Invoke();
    }
}

public class CustomQueueItem : QueueItem
{
    public Action customAction;
    
    public CustomQueueItem(Action customAction)
    {
        this.customAction = customAction;
    }
    
    public override void Run()
    {
        customAction?.Invoke();
        onFinishCallback?.Invoke();
    }
}

public class QueueDelay : QueueItem
{
    private float seconds;
    public QueueDelay(float seconds)
    {
        this.seconds = seconds;
    }

    public override void Run()
    {
        Systems.Instance.DoCoroutine(Wait(), onFinishCallback);
    }

    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(seconds);
    }
}