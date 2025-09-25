using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;
    
    [Header("Chunk Settings")]
    [SerializeField] private float chunkSize = 8f;
    
    private List<Chunk> allChunks = new List<Chunk>();
    private Dictionary<Transform, Chunk> objectToChunkMap = new Dictionary<Transform, Chunk>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeChunks();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeChunks()
    {
        // Find all chunks in the scene
        Chunk[] chunks = FindObjectsOfType<Chunk>();
        allChunks.AddRange(chunks);
       // Debug.Log($"Found {allChunks.Count} chunks in scene");
    }

    public void RegisterObjectInChunk(Transform obj, Chunk chunk)
    {
        if (objectToChunkMap.ContainsKey(obj))
        {
            objectToChunkMap[obj] = chunk;
        }
        else
        {
            objectToChunkMap.Add(obj, chunk);
        }
    }

    public void UnregisterObjectFromChunk(Transform obj)
    {
        if (objectToChunkMap.ContainsKey(obj))
        {
            objectToChunkMap.Remove(obj);
        }
    }

    public void ValidateObjectParenting(Transform obj, string objectTag)
    {
        if (obj == null) return;

        Chunk correctChunk = FindChunkContainingPosition(obj.position);
        
        if (correctChunk != null)
        {
          
            if (!objectToChunkMap.ContainsKey(obj) || objectToChunkMap[obj] != correctChunk)
            {
              //  Debug.Log($"Correcting parenting: {obj.name} should be in chunk at {correctChunk.transform.position}");
                
                if (objectToChunkMap.ContainsKey(obj))
                {
                    UnregisterObjectFromChunk(obj);
                }
                
                correctChunk.ForceParentObject(obj);
            }
        }
        else
        {
        
            if (objectToChunkMap.ContainsKey(obj))
            {
                Debug.Log($"Correcting parenting: {obj.name} IS NOT in any chunk");
                
            
                Chunk currentChunk = objectToChunkMap[obj];
                currentChunk.ForceUnparentObject(obj);
                UnregisterObjectFromChunk(obj);
            }
        }
    }


    private Chunk FindChunkContainingPosition(Vector3 position)
    {
        foreach (Chunk chunk in allChunks)
        {
            if (chunk != null && IsPositionInChunk(position, chunk))
            {
                return chunk;
            }
        }
        return null;
    }

  
    private bool IsPositionInChunk(Vector3 position, Chunk chunk)
    {
        if (chunk == null) return false;

   
        Collider2D chunkCollider = chunk.GetComponent<Collider2D>();
        if (chunkCollider != null)
        {
            return chunkCollider.OverlapPoint(position);
        }

        Vector2 chunkCenter = chunk.transform.position;
        Vector2 minBounds = chunkCenter - new Vector2(chunkSize / 2f, chunkSize / 2f);
        Vector2 maxBounds = chunkCenter + new Vector2(chunkSize / 2f, chunkSize / 2f);
        
        return position.x >= minBounds.x && position.x <= maxBounds.x && 
               position.y >= minBounds.y && position.y <= maxBounds.y;
    }

    public bool IsObjectInAnyChunk(Transform obj)
    {
        return objectToChunkMap.ContainsKey(obj);
    }
    public void DebugParentingState()
    {
        Debug.Log($"Total objects in chunks: {objectToChunkMap.Count}");
        foreach (var kvp in objectToChunkMap)
        {
            Debug.Log($"{kvp.Key.name} -> Chunk at {kvp.Value.transform.position}");
        }
    }
}
