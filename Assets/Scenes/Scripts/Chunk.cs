using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Chunk : MonoBehaviour
{
    private static List<Transform> childrenInChunks = new List<Transform>();

    IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            yield return null; 
            other.transform.SetParent(transform);
            if (!childrenInChunks.Contains(other.transform))
            {
                childrenInChunks.Add(other.transform);
            }
        }
    }

    IEnumerator OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            yield return null; 
            other.transform.SetParent(null);
            if (childrenInChunks.Contains(other.transform))
            {
                childrenInChunks.Remove(other.transform);
            }
        }
    }

    public static bool IsObjectInAnyChunk(Transform obj)
    {
        return childrenInChunks.Contains(obj);
    }
}