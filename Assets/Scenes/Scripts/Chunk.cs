
using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private bool automaticParentingEnabled = true;

    public void DisableAutomaticParenting()
    {
        automaticParentingEnabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticParentingEnabled) return;
        
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (gameObject.activeInHierarchy && ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ReportTriggerEnter(this, other);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!automaticParentingEnabled) return;
        
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (gameObject.activeInHierarchy && ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ReportTriggerExit(this, other);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!automaticParentingEnabled) return;
        
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            if (gameObject.activeInHierarchy && ChunkManager.Instance != null)
            {
                // Additional validation for objects that might have been missed
                if (!ChunkManager.Instance.IsObjectInAnyChunk(other.transform))
                {
                    ChunkManager.Instance.ReportTriggerEnter(this, other);
                }
            }
        }
    }

    // Optional: Manual parenting methods for specific cases
    public void RequestParenting(Transform obj)
    {
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.ForceParentObject(obj, this);
        }
    }

    public void RequestUnparenting(Transform obj)
    {
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.ForceUnparentObject(obj);
        }
    }
}