using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/New Level")]
public class LevelConfig : ScriptableObject
{
    #region Level settings
    [Header("Level settings")]
    public int WavesCount;
    public int SpawnAmountMin;
    public int SpawnAmountMax;
    public float SpawnTimeMin;
    public float SpawnTimeMax;
    public int AbominationFrequency;
    #endregion
}