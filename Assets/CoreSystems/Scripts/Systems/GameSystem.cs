using UnityEngine;

public class GameSystem : PersistentSingleton<GameSystem>
{
    public LevelConfig LoadedLevelConfig;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    public void RunLevel(int level)
    {
        LoadedLevelConfig = Resources.Load<LevelConfig>("Levels/Level"+level);
        SceneSystem.Instance.LoadScene(Scene.Game);
    }
}