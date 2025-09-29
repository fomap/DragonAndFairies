using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelRestarter : MonoBehaviour
{
     private string currLvl;

    private void Start()
    {
        Scene currScene = SceneManager.GetActiveScene();
        currLvl = currScene.name;  
    }

    private void Update()
    {
        // Restart level when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }


    private void RestartLevel()
    {
        SceneManager.LoadScene(currLvl);
    }
}
