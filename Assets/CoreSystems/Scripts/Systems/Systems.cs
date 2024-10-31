using System;
using System.Collections;
using UnityEngine;

public class Systems : PersistentSingleton<Systems>
{
    public bool SystemsInitialized;

    
    protected override void OnInitialize()
    {
        Application.targetFrameRate = GameConstants.Instance.GameFps;

        SystemsInitialized = true;
    }

    public void DoCoroutine(IEnumerator coroutine, Action callback = null)
    {
        StartCoroutine(ResolveCoroutine(coroutine, callback));
    }

    public IEnumerator ResolveCoroutine(IEnumerator coroutine, Action callback = null)
    {
        yield return coroutine;
        callback?.Invoke();
    }
}
