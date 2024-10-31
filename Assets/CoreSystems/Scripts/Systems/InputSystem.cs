using System.Collections.Generic;
using UnityEngine;

public class InputSystem : PersistentSingleton<InputSystem>
{
    private List<string> disableRequests = new List<string>();

    [SerializeField] private GameObject loadingScreenBlocker;

    public bool IsDisabled => disableRequests.Count > 0;
    public bool IsEnabled => disableRequests.Count == 0;
    
    public void DisableInputSystem(string request)
    {
        if (!disableRequests.Contains(request))
        {
            // Debug.LogError("Disabled by: " + request);
            disableRequests.Add(request);
        }
        
        CheckInputSystem();
    }

    public void EnableInputSystem(string request)
    {
        if (disableRequests.Contains(request))
        {
            disableRequests.Remove(request);
            // Debug.LogError("Enabled by: " + request);
        }
        
        CheckInputSystem();
    }
    
    public void ForceEnableInputSystem()
    {
        disableRequests = new List<string>();
        
        CheckInputSystem();
    }

    public void CheckInputSystem()
    {
        loadingScreenBlocker.SetActive(IsDisabled);
    }
}