using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelRestarter : MonoBehaviour
{
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string boxTag = "Box";
    [SerializeField] private string currLvl;


    // private void Awake()
    // {
    //     currLvl = ;
    // }
    private void Start()
    {
        StartCoroutine(CheckObjectsParent());
    }

    private void Update()
    {
        // Restart level when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    private IEnumerator CheckObjectsParent()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            
            // Check player
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null && !Chunk.IsObjectInAnyChunk(player.transform))
            {
                RestartLevel();
                yield break;
            }
            
            // Check all boxes
            GameObject[] boxes = GameObject.FindGameObjectsWithTag(boxTag);
            foreach (var box in boxes)
            {
                if (!Chunk.IsObjectInAnyChunk(box.transform))
                {
                    RestartLevel();
                    yield break;
                }
            }
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(currLvl);
    }
}

// public class LevelRestarter : MonoBehaviour
// {
//     [SerializeField] private float checkInterval = 0.5f;
//     [SerializeField] private string playerTag = "Player";
//     [SerializeField] private string boxTag = "Box";
    

     
//     private void Start()
//     {
//         StartCoroutine(CheckObjectsParent());
//     }

//     private IEnumerator CheckObjectsParent()
//     {
//         while (true)
//         {
//             yield return new WaitForSeconds(checkInterval);
            
//             // Check player
//             GameObject player = GameObject.FindGameObjectWithTag(playerTag);
//             if (player != null && player.transform.parent == null)
//             {
//                 RestartLevel();
//                 yield break;
//             }
            
//             // Check all boxes
//             GameObject[] boxes = GameObject.FindGameObjectsWithTag(boxTag);
//             foreach (var box in boxes)
//             {
//                 if (box.transform.parent == null)
//                 {
//                     RestartLevel();
//                     yield break;
//                 }
//             }
//         }
//     }

//     private void RestartLevel()
//     {
//         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//     }
// }