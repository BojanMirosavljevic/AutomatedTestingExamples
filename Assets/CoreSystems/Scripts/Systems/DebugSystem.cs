using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DebugSystem : PersistentSingleton<DebugSystem>
{
    [SerializeField] private GameObject MainWindow;
    [SerializeField] private GameObject ButtonHolder;
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private GameObject InputPrefab;
    
    [SerializeField] private Button ToggleTestingToolButton;
    [SerializeField] private Button CloseTestingToolButton;
    
    [SerializeField] private bool visibleButton;

    private List<GameObject> InstantiatedDebugPrefabs;

    protected override void OnInitialize()
    {
#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        Destroy(gameObject);
        return;
#endif
        ToggleTestingToolButton.onClick.AddListener(ToggleMainWindow);
        ToggleTestingToolButton.gameObject.SetActive(visibleButton);

        CloseTestingToolButton.onClick.AddListener(CloseMainWindow);
        CloseMainWindow();

        InstantiatedDebugPrefabs = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            OpenMainWindow();
        }
    }

    public void OpenMainWindow()
    {
        StartCoroutine(ClearButtons());
        MainWindow.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMainWindow()
    {
        MainWindow.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ToggleMainWindow()
    {
        if (MainWindow.activeInHierarchy)
        {
            CloseMainWindow();
        }
        else
        {
            OpenMainWindow();
        }
    }

    private IEnumerator ClearButtons()
    {
        while (InstantiatedDebugPrefabs.Count > 0)
        {
            Destroy(InstantiatedDebugPrefabs[0]);
            InstantiatedDebugPrefabs.RemoveAt(0);
            yield return new WaitForEndOfFrame();
        }
        SetupButtons();
    }

    private void AddButton(string text, Action onClick, bool closeToolOnClick = true, Scene sceneOnly = Scene.None)
    {
        if (sceneOnly != Scene.None && sceneOnly != SceneSystem.Instance.GetActiveScene())
        {
            return;
        }
        
        GameObject go = Instantiate(ButtonPrefab, ButtonHolder.transform);
        Button button = go.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            onClick?.Invoke();
            if (closeToolOnClick)
            {
                CloseMainWindow();
            }
        });
        
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
        InstantiatedDebugPrefabs.Add(go);
    }

    private void AddIntegerInputField(string text, Action<int> onEdit, bool closeToolOnClick = true, int minimumValue = 0, int maximumValue = 99999, Scene sceneOnly = Scene.None)
    {
        if (sceneOnly != Scene.None && sceneOnly != SceneSystem.Instance.GetActiveScene())
        {
            Debug.LogError("sceneOnly: " +sceneOnly);
            Debug.LogError("GetActiveScene(): " +SceneSystem.Instance.GetActiveScene());
            return;
        }
        
        GameObject go = Instantiate(InputPrefab, ButtonHolder.transform);
        TMP_InputField input = go.GetComponent<TMP_InputField>();
        input.onEndEdit.AddListener((string inputValue) =>
        {
            int inputIntValue = Convert.ToInt32(inputValue);
            onEdit?.Invoke(Mathf.Clamp(inputIntValue, minimumValue, maximumValue));
            if (closeToolOnClick)
            {
                CloseMainWindow();
            }
        });
        input.placeholder.gameObject.GetComponent<TextMeshProUGUI>().text = text;
        
        InstantiatedDebugPrefabs.Add(go);
    }

    private void SetupButtons()
    {
        AddButton(
            "Set Last Wave",
            () =>
            {
                GameManager.Instance.WaveCount = GameSystem.Instance.LoadedLevelConfig.WavesCount;
                GameManager.Instance.SetRandomizedTimeToSpawn();
                GameManager.Instance.SetLevelProgressText();

                if (GameManager.Instance.EnemiesCount == 0)
                {
                    GameManager.Instance.StartEndGameFlow();
                }
            },
            sceneOnly: Scene.Game
        );
        
        AddButton(
            "Pass level",
            () =>
            {
                GameManager.Instance.StartEndGameFlow();
            },
            sceneOnly: Scene.Game
        );
    }
}