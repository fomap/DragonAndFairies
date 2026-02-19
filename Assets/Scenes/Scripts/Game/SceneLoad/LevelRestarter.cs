using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelRestarter : MonoBehaviour
{
    private string currLvl;
    private bool isRestarting;

    private void Start()
    {
        Scene currScene = SceneManager.GetActiveScene();
        currLvl = currScene.name;  
    }

    private void Update()
    {
        if (!isRestarting && Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    private void RestartLevel()
    {
        if (isRestarting) return;
        isRestarting = true;
        StartCoroutine(RestartLevelAsync());
    }

     private IEnumerator RestartLevelAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(currLvl);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }
        op.allowSceneActivation = true;
    }
}
