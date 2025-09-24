
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
            // Check if this chunk GameObject is actively enabled in the hierarchy before parenting.
            if (gameObject.activeInHierarchy)
            {
                other.transform.SetParent(transform);
                childrenInChunks.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            // Similarly, check before unparenting.
            if (gameObject.activeInHierarchy)
            {
                other.transform.SetParent(null);
                childrenInChunks.Remove(other.transform);
            }
        }
    }

    public static bool IsObjectInAnyChunk(Transform obj)
    {
        return childrenInChunks.Contains(obj);
    }
}

// using System.Collections.Generic;
// using UnityEngine;

// public class Chunk : MonoBehaviour
// {
//     [SerializeField] private Vector2Int gridPosition;
//     private HashSet<Transform> currentObjects = new HashSet<Transform>();
    
//     [Header("Bounds Settings")]
//     [SerializeField] private float chunkSize = 8f;
//     [SerializeField] private float boundsBuffer = 0.1f;

//     public Vector2Int GridPosition => gridPosition;

//     public void Initialize(Vector2Int gridPos)
//     {
//         gridPosition = gridPos;
//         transform.position = new Vector3(gridPos.x * chunkSize, gridPos.y * chunkSize, 0f);
//     }

//     public void AddObject(Transform obj, string objectTag)
//     {
//         if (obj == null || !(objectTag == "Player" || objectTag == "Box")) return;

//         if (!currentObjects.Contains(obj))
//         {
//             obj.SetParent(transform);
//             currentObjects.Add(obj);
//             Debug.Log($"{obj.name} parented to chunk at {gridPosition}");
//         }
//     }

//     public void RemoveObject(Transform obj)
//     {
//         if (obj != null && currentObjects.Contains(obj))
//         {
//             // Only unparent if object is truly outside this chunk's bounds
//             if (!IsWithinBounds(obj.position))
//             {
//                 obj.SetParent(null);
//                 currentObjects.Remove(obj);
//                 Debug.Log($"{obj.name} unparented from chunk at {gridPosition}");
//             }
//             else
//             {
//                 Debug.LogWarning($"{obj.name} tried to unparent but is still within bounds of chunk {gridPosition}");
//             }
//         }
//     }

//     public bool IsWithinBounds(Vector2 position)
//     {
//         Vector2 chunkCenter = transform.position;
//         Vector2 minBounds = chunkCenter - new Vector2(chunkSize / 2f, chunkSize / 2f) + new Vector2(boundsBuffer, boundsBuffer);
//         Vector2 maxBounds = chunkCenter + new Vector2(chunkSize / 2f, chunkSize / 2f) - new Vector2(boundsBuffer, boundsBuffer);
        
//         return position.x >= minBounds.x && position.x <= maxBounds.x && 
//                position.y >= minBounds.y && position.y <= maxBounds.y;
//     }

//     // Method for Fey to check if she should be parented to this chunk
//     public bool ShouldParentObject(Vector2 objectPosition)
//     {
//         return IsWithinBounds(objectPosition);
//     }

//     private void OnDestroy()
//     {
//         foreach (Transform obj in currentObjects)
//         {
//             if (obj != null)
//             {
//                 obj.SetParent(null);
//             }
//         }
//         currentObjects.Clear();
//     }

//     // Debug visualization
//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = currentObjects.Count > 0 ? Color.green : Color.blue;
//         Gizmos.DrawWireCube(transform.position, new Vector3(chunkSize, chunkSize, 0.1f));
        
//         // Draw bounds with buffer
//         Gizmos.color = Color.red;
//         Vector2 minBounds = (Vector2)transform.position - new Vector2(chunkSize / 2f - boundsBuffer, chunkSize / 2f - boundsBuffer);
//         Vector2 maxBounds = (Vector2)transform.position + new Vector2(chunkSize / 2f - boundsBuffer, chunkSize / 2f - boundsBuffer);
//         Gizmos.DrawWireCube((minBounds + maxBounds) / 2f, new Vector3(chunkSize - boundsBuffer * 2f, chunkSize - boundsBuffer * 2f, 0.1f));
//     }
// }


// using System.Collections.Generic;
// using UnityEngine;

// public class Chunk : MonoBehaviour
// {
//     [SerializeField] private Vector2Int gridPosition;
//     private HashSet<Transform> currentObjects = new HashSet<Transform>();

//     public Vector2Int GridPosition => gridPosition;

//     public void Initialize(Vector2Int gridPos)
//     {
//         gridPosition = gridPos;
//         transform.position = new Vector3(gridPos.x * 8f, gridPos.y * 8f, 0f);
//     }

//     public void AddObject(Transform obj, string objectTag)
//     {
//         if (obj == null || !(objectTag == "Player" || objectTag == "Box")) return;

//         if (!currentObjects.Contains(obj))
//         {
//             obj.SetParent(transform);
//             currentObjects.Add(obj);
//         }
//     }

//     public void RemoveObject(Transform obj)
//     {
//         if (obj != null && currentObjects.Contains(obj))
//         {
//             obj.SetParent(null);
//             currentObjects.Remove(obj);
//         }
//     }

//     private void OnDestroy()
//     {
//         foreach (Transform obj in currentObjects)
//         {
//             if (obj != null)
//             {
//                 obj.SetParent(null);
//             }
//         }
//         currentObjects.Clear();
//     }
    

    
// }




// public class Chunk : MonoBehaviour
// {
   
//     private static readonly HashSet<Transform> childrenInChunks = new HashSet<Transform>();

//     private void OnTriggerEnter2D(Collider2D other)
//     {
        
//         if (other.CompareTag("Player") || other.CompareTag("Box"))
//         {
//             other.transform.SetParent(transform);
//             childrenInChunks.Add(other.transform);
//         }
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         if (other.CompareTag("Player") || other.CompareTag("Box"))
//         {
//             other.transform.SetParent(null);
//             childrenInChunks.Remove(other.transform);

//         }
//     }

//     public static bool IsObjectInAnyChunk(Transform obj)
//     {
//         return childrenInChunks.Contains(obj);
//     }
// }


