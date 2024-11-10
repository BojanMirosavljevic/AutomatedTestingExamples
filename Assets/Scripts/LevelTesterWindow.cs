using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class LevelTesterWindow : EditorWindow
{
    private List<LevelConfig> remoteConfigs;

    private string remoteConfigText;
    private string resultText = "Not started yet.";
    private Vector2 scrollPosition;

    [MenuItem("Testing/Level Tester")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(LevelTesterWindow));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Download Remote Levels"))
            {
                StartDownloadRemoteConfig();
            }
            UpdateRemoteConfigText();
            GUILayout.Label(remoteConfigText);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            if (GUILayout.Button("Test All Levels"))
            {
                TestAllLevels();
            }
        GUILayout.EndHorizontal();

        GUILayout.Label("Result:");

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(400));
            GUILayout.TextArea(resultText);
        GUILayout.EndScrollView();
    }

    private void UpdateRemoteConfigText()
    {
        if (remoteConfigs != null)
        {
            remoteConfigText = "Remote levels count: " + remoteConfigs.Count;
        }
        else
        {
            remoteConfigText = "Remote levels count: NULL";
        }
    }

    private void UpdateTestResultText(string result)
    {
        resultText = result;
    }

    private void StartDownloadRemoteConfig()
    {
        EditorCoroutineUtility.StartCoroutine(DownloadLevelConfigs(), this);
    }

    private IEnumerator DownloadLevelConfigs()
    {
        string uri = "https://bojanmiros.com/AutomatedTestingExamples/index.php";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    try
                    {
                        remoteConfigs = JsonConvert.DeserializeObject<List<LevelConfig>>(webRequest.downloadHandler.text);
                    }
                    catch(Exception e)
                    {
                        Debug.LogException(e);
                    }
                    break;
            }

            UpdateRemoteConfigText();
        }
    }

    private void TestAllLevels()
    {
        List<LevelConfig> localConfigs = Resources.LoadAll<LevelConfig>("Levels").OrderBy(s => int.Parse(Regex.Match(s.name, @"\d+").Value)).ToList();
        if (remoteConfigs == null || remoteConfigs.Count == 0)
        {
            UpdateTestResultText("Remote Config is NULL or empty!");
        }
        else
        {
            //note: this should be implemented with IComparable on LevelConfig, but implemented like this for example simplicity purposes
            string testResult = "";
            for(int i = 0; i < remoteConfigs.Count; i++)
            {
                string levelResult = "";
                levelResult += GetLevelResult("WavesCount", remoteConfigs[i].WavesCount, localConfigs[i].WavesCount);
                levelResult += GetLevelResult("SpawnAmountMin", remoteConfigs[i].SpawnAmountMin, localConfigs[i].SpawnAmountMin);
                levelResult += GetLevelResult("SpawnAmountMax", remoteConfigs[i].SpawnAmountMax, localConfigs[i].SpawnAmountMax);
                levelResult += GetLevelResult("SpawnTimeMin", remoteConfigs[i].SpawnTimeMin, localConfigs[i].SpawnTimeMin);
                levelResult += GetLevelResult("SpawnTimeMax", remoteConfigs[i].SpawnTimeMax, localConfigs[i].SpawnTimeMax);
                levelResult += GetLevelResult("AbominationFrequency", remoteConfigs[i].AbominationFrequency, localConfigs[i].AbominationFrequency);

                if (string.IsNullOrEmpty(levelResult))
                {
                    testResult += "Level " + (i+1) + ": OK\n\n";
                }
                else
                {
                    testResult += "Level " + (i+1) + ": FAIL\n" + levelResult + "\n\n";
                }
            }
            UpdateTestResultText(testResult);
        }
    }

    private string GetLevelResult<T>(string variableName, T remoteValue, T localValue)
    {
        string variableResult = "";

        if (!remoteValue.Equals(localValue))
        {
            variableResult += "\n"+variableName+" should be " + remoteValue + " instead of " + localValue;
        }

        return variableResult;
    }
}