using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int levelSceneIndex;
    [SerializeField] private Color completedColor;
    [SerializeField] private Color completedHighlightedColor;
    [SerializeField] private Color completedPressedColor;
    [SerializeField] private Color selectedColor;
    [SerializeField] private string levelId; 

    private Button button;

    public void TransitionToLevel()
    {
        FindObjectOfType<MainMenu>().StartLevel(levelSceneIndex);
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TransitionToLevel);
        if (!PlayerPrefs.HasKey(levelId))
        {
            return;
        }
        if (PlayerPrefs.GetInt(levelId) == 1)
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = completedColor;
            colorBlock.highlightedColor = completedHighlightedColor;
            colorBlock.pressedColor = completedPressedColor;
            colorBlock.selectedColor = selectedColor;
            button.colors = colorBlock;
        }
    }
}
