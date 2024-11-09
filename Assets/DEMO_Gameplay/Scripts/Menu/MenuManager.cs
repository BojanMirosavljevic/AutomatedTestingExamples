using UnityEngine;

public class MenuManager : StaticInstance<MenuManager>
{
    [SerializeField] private Transform StartLevelButtonHolder;
    [SerializeField] private ButtonStartLevel StartLevelButtonPrefab;

    private void Start()
    {
        for (int i = 1; i < 13; i++)
        {
            ButtonStartLevel button = Instantiate(StartLevelButtonPrefab, StartLevelButtonHolder);

            button.Init(i);
        }
    }
}
