
// using System.Collections.Generic;
// using UnityEngine;

// public class ChunkManager : MonoBehaviour
// {
//     public static ChunkManager Instance;
    
//     [SerializeField] private float chunkSize = 8f;
//     private Dictionary<Vector2Int, Chunk> chunkGrid = new Dictionary<Vector2Int, Chunk>();
//     private Dictionary<Transform, Chunk> objectToChunkMap = new Dictionary<Transform, Chunk>();

//     private void Awake()
//     {
//         Instance = this;
//         InitializeChunkGrid();
//     }

//     private void InitializeChunkGrid()
//     {
//         Chunk[] allChunks = FindObjectsOfType<Chunk>();
//         foreach (Chunk chunk in allChunks)
//         {
//             Vector2Int gridPos = WorldToGridPosition(chunk.transform.position);
//             chunkGrid[gridPos] = chunk;
//             chunk.Initialize(gridPos);
//         }
//     }

//     public Vector2Int WorldToGridPosition(Vector3 worldPosition)
//     {
//         int gridX = Mathf.RoundToInt(worldPosition.x / chunkSize);
//         int gridY = Mathf.RoundToInt(worldPosition.y / chunkSize);
//         return new Vector2Int(gridX, gridY);
//     }

//     public void UpdateObjectChunk(Transform obj, string objectTag)
//     {
//         if (obj == null || !(objectTag == "Player" || objectTag == "Box")) return;

//         Vector2Int gridPos = WorldToGridPosition(obj.position);
        
//         // Check if object is already in the correct chunk
//         if (objectToChunkMap.ContainsKey(obj) && objectToChunkMap[obj] != null)
//         {
//             Chunk currentChunk = objectToChunkMap[obj];
//             if (currentChunk.GridPosition == gridPos && currentChunk.IsWithinBounds(obj.position))
//             {
//                 // Object is already in the correct chunk and within bounds
//                 return;
//             }
//         }

//         // Remove from previous chunk with validation
//         if (objectToChunkMap.ContainsKey(obj))
//         {
//             Chunk previousChunk = objectToChunkMap[obj];
//             if (previousChunk != null)
//             {
//                 previousChunk.RemoveObject(obj); // This now includes bounds checking
//             }
//             objectToChunkMap.Remove(obj);
//         }

//         // Add to new chunk with validation
//         if (chunkGrid.TryGetValue(gridPos, out Chunk newChunk))
//         {
//             if (newChunk.ShouldParentObject(obj.position))
//             {
//                 newChunk.AddObject(obj, objectTag);
//                 objectToChunkMap[obj] = newChunk;
//             }
//             else
//             {
//                 // Object is not within bounds of any chunk, leave unparented
//                 obj.SetParent(null);
//             }
//         }
//         else
//         {
//             // Not in any chunk, ensure unparented
//             obj.SetParent(null);
//         }
//     }

//     // Method for Fey to force a validation check
//     public void ValidateObjectParenting(Transform obj, string objectTag)
//     {
//         if (obj == null) return;

//         bool shouldBeParented = false;
//         Chunk correctChunk = null;

//         // Check all chunks to see if object should be parented to any
//         foreach (var chunk in chunkGrid.Values)
//         {
//             if (chunk.ShouldParentObject(obj.position))
//             {
//                 shouldBeParented = true;
//                 correctChunk = chunk;
//                 break;
//             }
//         }

//         if (shouldBeParented && correctChunk != null)
//         {
//             // Object should be parented but isn't, or is parented to wrong chunk
//             if (!objectToChunkMap.ContainsKey(obj) || objectToChunkMap[obj] != correctChunk)
//             {
//                 UpdateObjectChunk(obj, objectTag);
//             }
//         }
//         else if (!shouldBeParented)
//         {
//             // Object should not be parented but is
//             if (objectToChunkMap.ContainsKey(obj))
//             {
//                 Chunk currentChunk = objectToChunkMap[obj];
//                 if (currentChunk != null)
//                 {
//                     currentChunk.RemoveObject(obj);
//                 }
//                 objectToChunkMap.Remove(obj);
//                 obj.SetParent(null);
//             }
//         }
//     }

//     public bool IsObjectInAnyChunk(Transform obj)
//     {
//         return objectToChunkMap.ContainsKey(obj);
//     }
// }





























// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;


// // public class ChunkManager : MonoBehaviour
// // {
// //     public static ChunkManager Instance;
    
// //     private Dictionary<Transform, Chunk> objectToChunkMap = new Dictionary<Transform, Chunk>();
// //     private Dictionary<Chunk, List<Transform>> chunkContents = new Dictionary<Chunk, List<Transform>>();

// //     private void Awake()
// //     {
// //         Instance = this;
// //     }

// //     public void RegisterChunk(Chunk chunk)
// //     {
// //         if (!chunkContents.ContainsKey(chunk))
// //         {
// //             chunkContents[chunk] = new List<Transform>();
// //         }
// //     }

// //     public void RequestParenting(Transform obj, Chunk targetChunk)
// //     {
// //         StartCoroutine(ProcessParentingRequest(obj, targetChunk));
// //     }

// //     private IEnumerator ProcessParentingRequest(Transform obj, Chunk targetChunk)
// //     {
// //         yield return new WaitForEndOfFrame();
        
// //         // Remove from current chunk
// //         if (objectToChunkMap.ContainsKey(obj))
// //         {
// //             Chunk currentChunk = objectToChunkMap[obj];
// //             chunkContents[currentChunk].Remove(obj);
// //             obj.SetParent(null);
// //         }

// //         // Add to new chunk
// //         if (targetChunk != null && chunkContents.ContainsKey(targetChunk))
// //         {
// //             obj.SetParent(targetChunk.transform);
// //             objectToChunkMap[obj] = targetChunk;
// //             chunkContents[targetChunk].Add(obj);
// //         }
// //         else
// //         {
// //             objectToChunkMap.Remove(obj);
// //         }
// //     }
// // }