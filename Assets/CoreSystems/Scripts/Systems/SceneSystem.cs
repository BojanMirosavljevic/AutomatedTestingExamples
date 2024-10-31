using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SceneSystem : PersistentSingleton<SceneSystem>
{
    public event Action<Scene> SceneChangeStarted;
    public event Action<Scene> SceneChangeEnded;

    [SerializeField] private CanvasGroup blackFaderCanvas;

    private readonly float fadeDuration = 0.3f;
    private bool isFaded = false;

    public void LoadScene(Scene newScene, Action onSceneChanged = null)
    {
        SceneChangeStarted?.Invoke(newScene);

        InputSystem.Instance.DisableInputSystem("LoadScene");

        isFaded = false;
        StartCoroutine(FadeTo(true, fadeDuration, 0f, () => {
            isFaded = true;
        }));
        
        StartCoroutine(LoadSceneAsync((int)newScene, () =>
        {
            SceneChangeEnded?.Invoke(newScene);
            onSceneChanged?.Invoke();
        }));
    }
    
    IEnumerator LoadSceneAsync(int sceneIndex, Action callback)
    {
        yield return new WaitUntil(() => isFaded);

        SceneManager.LoadScene(sceneIndex);

        StartCoroutine(FadeTo(false, fadeDuration, 0.5f, () => {
            InputSystem.Instance.EnableInputSystem("LoadScene");
            callback?.Invoke();
        }));
    }

    public Scene GetActiveScene()
    {
        return (Scene)SceneManager.GetActiveScene().buildIndex;
    }

    public bool IsActiveScene(Scene scene)
    {
        return GetActiveScene() == scene;
    }

    private IEnumerator FadeTo(bool black, float duration, float delay, Action onComplete)
    {
        yield return new WaitForSecondsRealtime(delay);

        float startAlpha = blackFaderCanvas.alpha;
        float endAlpha = black ? 1f : 0f;
        if (startAlpha != endAlpha)
        {
            float timePassed = 0f;

            while (timePassed < duration)
            {
                timePassed += Time.deltaTime;

                blackFaderCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, timePassed / duration);
                yield return new WaitForEndOfFrame();
            }

            blackFaderCanvas.alpha = endAlpha;
        }

        onComplete?.Invoke();
    }
}

public enum Scene
{
    None = -1,
    Loading = 0,
    Game = 1
}