using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private static readonly HashSet<Transform> childrenInChunks = new HashSet<Transform>();

    private void Start()
    {
        if (ChunkManager.Instance != null)
        {
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {

            if (gameObject.activeInHierarchy)
            {
                ParentObject(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
           
            if (gameObject.activeInHierarchy)
            {
                UnparentObject(other.transform);
            }
        }
    }

   
    public void ForceParentObject(Transform obj)
    {
        StartCoroutine(DelayedParent(obj, true));
    }

    public void ForceUnparentObject(Transform obj)
    {
        StartCoroutine(DelayedParent(obj, false));
    }

    private void ParentObject(Transform obj)
    {
        if (obj == null || childrenInChunks.Contains(obj)) return;

        obj.SetParent(transform);
        childrenInChunks.Add(obj);

        
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.RegisterObjectInChunk(obj, this);
        }
    }

    private void UnparentObject(Transform obj)
    {
        if (obj != null && childrenInChunks.Contains(obj))
        {
            obj.SetParent(null);
            childrenInChunks.Remove(obj);

            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.UnregisterObjectFromChunk(obj);
            }
        }
    }

    private IEnumerator DelayedParent(Transform obj, bool parentToChunk)
    {
       
        yield return null;

        if (obj == null) yield break;

        if (parentToChunk)
        {
            ParentObject(obj);
        }
        else
        {
            UnparentObject(obj);
        }
    }

    public static bool IsObjectInAnyChunk(Transform obj)
    {
        return childrenInChunks.Contains(obj);
    }
}

