
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class quit : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
             
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {

            Scene currScene = SceneManager.GetActiveScene();
            string currLvl = currScene.name;

            if (currLvl == "level10")
            {
                 GlobalSkyfallEventManager.Instance?.ResumeGame();
                SceneManager.LoadScene(0);
            }
            else if(currLvl == "level0")
            {
            Debug.Log("Quitting applicaiton");
            Application.Quit();
            }
       
        }
    }
}
