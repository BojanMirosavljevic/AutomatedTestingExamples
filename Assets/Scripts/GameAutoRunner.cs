using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameAutoRunner : MonoBehaviour
{
    private int currentLevel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Init(int startFromLevel = 1)
    {
        currentLevel = startFromLevel;
        StartCoroutine(MainLogic());
    }

    private IEnumerator MainLogic()
    {
        while(true)
        {
            yield return StartLevel();
            yield return WaitWhileSceneLoaded(Scene.Game);
            yield return PlayLevel();
            yield return ExitLevel();
            yield return WaitWhileSceneLoaded(Scene.Menu);
        }
    }

    private IEnumerator WaitWhileSceneLoaded(Scene scene)
    {
        bool nextSceneLoaded = false;
        SceneSystem.Instance.SceneChangeEnded += (Scene newScene) =>
        {
            if (newScene == scene)
            {
                nextSceneLoaded = true;
            } 
        };
        yield return new WaitWhile(() => !nextSceneLoaded);
    }

    private IEnumerator StartLevel()
    {
        yield return new WaitForSeconds(1f);

        if (SceneSystem.Instance.GetActiveScene() == Scene.Menu)
        {
            GameSystem.Instance.RunLevel(currentLevel);
        }
    }

    private IEnumerator PlayLevel()
    {
        bool gameRunning = true;

        GameManager.Instance.GameEnded += (bool win) =>
        {
            gameRunning = false;
            if (win)
            {
                currentLevel++;
            }
        };

        List<GameObject> enemies = new List<GameObject>();
        PlayerController player = GameManager.Instance.Player;

        while (gameRunning)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy")
                                .OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude)
                                .ToList();
                                
            if (enemies != null && enemies.Count > 0)
            {
                //try shoot
                player.TryShootEnemy(enemies[0].GetComponent<Enemy>());
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    private IEnumerator ExitLevel()
    {
        yield return new WaitForSecondsRealtime(1f);

        if (SceneSystem.Instance.GetActiveScene() == Scene.Game)
        {
            GameObject.Find("ButtonMenu").GetComponent<Button>().onClick.Invoke();
        }
    }
}
