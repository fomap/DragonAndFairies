using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject levelSelectPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject PauseBtn;
    // Start is called before the first frame update
    public void Play()
    {
        Time.timeScale = 1f;

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(LoadLevelAsync(sceneIndex));


        Debug.Log(sceneIndex + 1);
        menuPanel.SetActive(false);
        PauseBtn.SetActive(true);
        GlobalSkyfallEventManager.Instance?.ResumeGame();
        SceneManager.LoadScene(sceneIndex + 1);
    }

    // Update is called once per frame
    public void ShowLVLSelect()
    {
        levelSelectPanel.SetActive(true);

    }

    public void HideLVLSelect()
    {
        levelSelectPanel.SetActive(false);
    }

    public void ResetAllProgress()
    {
        GameProgressManager.Instance.ResetAllProgress();
    }


   
    private IEnumerator LoadLevelAsync(int buildIndex)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex);



        while (!asyncLoad.isDone)
        {

                   yield return null;


            // yield return new WaitForSeconds(0.5f); 
        }
        
          menuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        PauseBtn.SetActive(true);
            

    }


}
