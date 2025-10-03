using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] GameObject levelSelectPanel;
    // Start is called before the first frame update
    public void Play()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(sceneIndex + 1);
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



}
