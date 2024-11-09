using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonStartLevel : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    [SerializeField] private TextMeshProUGUI LevelNumberText;
    
    public void Init(int level)
    {
        LevelNumberText.text = level.ToString();
        StartButton.onClick.AddListener(() => 
        {
                GameSystem.Instance.RunLevel(level);
        });
    }
}
