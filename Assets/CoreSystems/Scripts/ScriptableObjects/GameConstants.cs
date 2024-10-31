using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConstants", menuName = "Scriptable Objects/New GameConstants")]
public class GameConstants : ScriptableObject
{
    private static GameConstants instance;

    public static GameConstants Instance
    {
        get { return instance ?? Resources.Load<GameConstants>("Configs/GameConstants"); }
    }

    #region Global settings
    [Header("Global settings")]
    [Tooltip("Game settings")]
    public int GameFps;

    #endregion
}