using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chunk : MonoBehaviour
{
   
    private static readonly HashSet<Transform> childrenInChunks = new HashSet<Transform>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            other.transform.SetParent(transform);
            childrenInChunks.Add(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            other.transform.SetParent(null);
            childrenInChunks.Remove(other.transform);

        }
    }

    public static bool IsObjectInAnyChunk(Transform obj)
    {
        return childrenInChunks.Contains(obj);
    }
}


