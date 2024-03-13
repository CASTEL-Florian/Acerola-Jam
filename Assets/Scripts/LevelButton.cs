using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int levelSceneIndex;
    [SerializeField] private Color completedColor;
    [SerializeField] private Color completedSelectedColor;
    [SerializeField] private Color completedPressedColor;

    private Button button;

    public void TransitionToLevel()
    {
        FindObjectOfType<MainMenu>().StartLevel(levelSceneIndex);
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(TransitionToLevel);
        if (!PlayerPrefs.HasKey(levelSceneIndex.ToString()))
        {
            return;
        }
        if (PlayerPrefs.GetInt(levelSceneIndex.ToString()) == 1)
        {
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = completedColor;
            colorBlock.highlightedColor = completedSelectedColor;
            colorBlock.pressedColor = completedPressedColor;
            button.colors = colorBlock;
        }
    }
}
