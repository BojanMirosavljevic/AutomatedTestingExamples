using Newtonsoft.Json;

public class PlayerSave : GenericSave
{
    [JsonProperty]
    private int enemyKillCounter = 0;

    [JsonIgnore]
    public int EnemyKillCounter
    {
        get { return enemyKillCounter; }
        set
        {
            enemyKillCounter = value;
            SaveSystem.Instance.Flag(this);
        }
    }
}