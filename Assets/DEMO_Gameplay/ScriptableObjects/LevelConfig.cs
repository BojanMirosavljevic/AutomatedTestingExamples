using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/New Level")]
public class LevelConfig : ScriptableObject
{
    #region Level settings
    [Header("Level settings")]
    public int ZombiesCount;
    public int AbominationCount;
    public float SpawnTimeMin;
    public float SpawnTimeMax;
    #endregion
}