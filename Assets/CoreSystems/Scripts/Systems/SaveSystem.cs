using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SaveSystem : PersistentSingleton<SaveSystem>
{
    public JsonSerializerSettings JSet;
    public PlayerSave Player;

    private List<string> ChangedSaves;
    private Coroutine QueuedSave;

    private Dictionary<string, GenericSave> Saves;

    protected override void OnInitialize()
    {
        JSet = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        
        Saves = new Dictionary<string, GenericSave>();
        ChangedSaves = new List<string>();
        
        Player = (PlayerSave)Load(new PlayerSave());
        Saves.Add(Player.Name, Player);
    }

    public GenericSave Load(GenericSave save)
    {
        if (PlayerPrefs.HasKey(save.Name))
        {
            return JsonConvert.DeserializeObject<GenericSave>(PlayerPrefs.GetString(save.Name), JSet);
        }
        
        return save;
    }

    public void Flag(GenericSave save)
    {
        if (ChangedSaves.Contains(save.Name))
        {
            return;
        }
        
        ChangedSaves.Add(save.Name);
        if (QueuedSave == null)
        {
            QueuedSave = StartCoroutine(DelayedSave());
        }
    }

    private IEnumerator DelayedSave()
    {
        yield return new WaitForEndOfFrame();
        foreach (string save in ChangedSaves)
        {
            Saves[save].Save();
        }
        ChangedSaves.Clear();
        PlayerPrefs.Save();
        
        QueuedSave = null;
    }
}

public abstract class GenericSave
{
    public string Name;
    
    public GenericSave()
    {
        Name = this.GetType().ToString();
    }
    
    public void Save()
    {
        PlayerPrefs.SetString(Name, JsonConvert.SerializeObject(this, SaveSystem.Instance.JSet));
    }
}