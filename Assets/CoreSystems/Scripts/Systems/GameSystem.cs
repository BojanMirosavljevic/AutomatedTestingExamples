using UnityEngine;

public class GameSystem : PersistentSingleton<GameSystem>
{
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
        LevelConfig config = Resources.Load<LevelConfig>("Levels/Level"+level);
        SceneSystem.Instance.LoadScene(Scene.Game);
    }
}