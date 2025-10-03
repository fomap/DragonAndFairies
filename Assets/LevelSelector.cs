using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {

        for (int i = 0; i < levelButtons.Length; i++)
        {

            int levelIndex = i + 1;

            Text buttonText = levelButtons[i].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = (levelIndex).ToString();
            }

            levelButtons[i].onClick.AddListener(() => LoadLevelByIndex(levelIndex));
        }
    }

    public void LoadLevelByIndex(int buildIndex)
    {


        Time.timeScale = 1f;
        SceneManager.LoadScene(buildIndex);

    }



    public void LoadLevel(string sceneName)
    {
      
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

}
