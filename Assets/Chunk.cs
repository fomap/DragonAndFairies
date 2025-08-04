using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // Make the player a child of the chunk when they enter

            yield return null; 
            other.transform.SetParent(transform);
        }
    }

    IEnumerator OnTriggerExit2D(Collider2D other)
    {
        // Check if the object leaving is the player
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // Detach the player from the chunk when they exit
            yield return null; 
            other.transform.SetParent(null);
        }
    }

}