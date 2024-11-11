using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameAutoRunner : MonoBehaviour
{
    private int currentLevel;
    private int maxLevel;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        maxLevel = Resources.LoadAll<LevelConfig>("Levels").Length;
    }

    public void Init(int startFromLevel = 1)
    {
        DebugSystem.Instance.OverridePlayerMovement = true;
        currentLevel = startFromLevel;
        StartCoroutine(MainLogic());
    }

    private void OnDestroy()
    {
        DebugSystem.Instance.OverridePlayerMovement = false;
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
            if (currentLevel > maxLevel)
            {
                Destroy(gameObject);
                EditorUtility.DisplayDialog("Game Tester", "Game completed successfully!", "YAY");
                yield break;
            }
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
            GameObject.Find("ButtonHolder").transform.GetChild(currentLevel-1).GetComponent<Button>().onClick.Invoke();
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
            //find enemy to shoot
            enemies = GameObject.FindGameObjectsWithTag("Enemy")
                                .OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude)
                                .ToList();
                                
            if (enemies != null && enemies.Count > 0)
            {
                //try shoot
                player.TryShootEnemy(enemies[0].GetComponent<Enemy>());
            }

            //wait for end of frame to allow enemy to be destroyed before moving
            yield return new WaitForEndOfFrame();

            //find enemy to run away from
            enemies = GameObject.FindGameObjectsWithTag("Enemy")
                                .OrderBy(x => (x.transform.position - player.transform.position).sqrMagnitude)
                                .ToList();
            
            //if no enemy is found, move towards middle (0,0 coordinates)
            Vector2 moveTowards = Vector2.zero;
            int moveDirection = 1;

            if (enemies != null && enemies.Count > 0)
            {
                moveTowards = enemies[0].transform.position;
                moveDirection = -1;
            }
            
            if (moveTowards.x > player.transform.position.x)
            {
                DebugSystem.Instance.Horizontal = 1f * moveDirection;
            }
            else if (moveTowards.x < player.transform.position.x)
            {
                DebugSystem.Instance.Horizontal = -1f * moveDirection;
            }
            else
            {
                DebugSystem.Instance.Horizontal = 0f;
            }

            if (moveTowards.y > player.transform.position.y)
            {
                DebugSystem.Instance.Vertical = 1f * moveDirection;
            }
            else if (moveTowards.y < player.transform.position.y)
            {
                DebugSystem.Instance.Vertical = -1f * moveDirection;
            }
            else
            {
                DebugSystem.Instance.Vertical = 0f;
            }

            //wait for end of frame to allow time to pass for enemies to spawn
            yield return new WaitForEndOfFrame();
        }

        DebugSystem.Instance.Horizontal = 0f;
        DebugSystem.Instance.Vertical = 0f;
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
