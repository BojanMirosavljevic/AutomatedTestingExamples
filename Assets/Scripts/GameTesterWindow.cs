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
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameTesterWindow : EditorWindow
{
    private GameObject GameAutoRunner;
    private string currentLevel = "1";
    private string timeScale = "1.0";

    [MenuItem("Testing/Game Tester")]
    public static void ShowWindow()
    {
        EditorWindow window = GetWindow(typeof(GameTesterWindow));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Running full game");

        GUILayout.BeginVertical();
            GUILayout.Label("Start from level:");
            currentLevel = GUILayout.TextField(currentLevel);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
            if (GUILayout.Button("START"))
            {
                if(!EditorApplication.isPlaying)
                {
                    EditorUtility.DisplayDialog("Game Tester Error", "Game is not playing!", "OK");
                    return;
                }
                if (GameAutoRunner != null)
                {
                    EditorUtility.DisplayDialog("Game Tester Error", "Runner already active!", "OK");
                    return;
                }

                GameAutoRunner = new GameObject("GameAutoRunner");
                GameAutoRunner.AddComponent<GameAutoRunner>().Init(Convert.ToInt32(currentLevel));
            }
            if (GUILayout.Button("STOP"))
            {
                if(!EditorApplication.isPlaying)
                {
                    EditorUtility.DisplayDialog("Game Tester Error", "Game is not playing!", "OK");
                    return;
                }
                Destroy(GameAutoRunner.gameObject);
            }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
            GUILayout.Label("TimeScale:");
            timeScale = GUILayout.TextField(timeScale);
            Time.timeScale = Convert.ToSingle(timeScale);
        GUILayout.EndVertical();
    }
}