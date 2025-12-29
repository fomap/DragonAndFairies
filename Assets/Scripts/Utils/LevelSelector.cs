using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{
    public Button[] levelButtons;

    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject PauseBtn;

    [SerializeField] GameObject levelSelectPanel;

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




        StartCoroutine(LoadLevelAsync(buildIndex));


        menuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        PauseBtn.SetActive(true);
        GlobalSkyfallEventManager.Instance?.ResumeGame();





        //SceneManager.LoadScene(buildIndex);

    }

    private IEnumerator LoadLevelAsync(int buildIndex)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {   
            if (asyncLoad.progress >= 0.9f)
            {
            asyncLoad.allowSceneActivation = true;
            }

            yield return null;

            // yield return new WaitForSeconds(0.5f); 
        }
        
          menuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        PauseBtn.SetActive(true);
            

    }

}
