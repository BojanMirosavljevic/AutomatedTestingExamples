using UnityEngine;

public class GameSystem : PersistentSingleton<GameSystem>
{
    public LevelConfig LoadedLevelConfig;
    public int CurrentLevelNumber;

    public void RunLevel(int level)
    {
        CurrentLevelNumber = level;
        LoadCurrentLevelConfig();
        SceneSystem.Instance.LoadScene(Scene.Game);
    }

    private void LoadCurrentLevelConfig()
    {
        LoadedLevelConfig = Resources.Load<LevelConfig>("Levels/Level"+CurrentLevelNumber);
    }
}