using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public static ChunkManager Instance;

    [Header("Chunk Settings")]
    [SerializeField] private float chunkSize = 8f;

    private List<Chunk> allChunks = new List<Chunk>();
    private Dictionary<Transform, Chunk> objectToChunkMap = new Dictionary<Transform, Chunk>();
    
    // Queue for handling parenting operations to avoid conflicts
    private Queue<ParentingOperation> parentingQueue = new Queue<ParentingOperation>();
    private bool isProcessingQueue = false;

    private struct ParentingOperation
    {
        public Transform Object;
        public Chunk TargetChunk;
        public OperationType Type;
    }

    private enum OperationType
    {
        Parent,
        Unparent
    }

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

    private void Update()
    {
        // Process parenting operations in the main thread
        ProcessParentingQueue();
    }

    private void InitializeChunks()
    {
        Chunk[] chunks = FindObjectsOfType<Chunk>();
        allChunks.AddRange(chunks);
        
        // Disable automatic parenting in chunks
        foreach (Chunk chunk in allChunks)
        {
            chunk.DisableAutomaticParenting();
        }
    }

    private void ProcessParentingQueue()
    {
        if (isProcessingQueue || parentingQueue.Count == 0) return;
        
        isProcessingQueue = true;
        
        while (parentingQueue.Count > 0)
        {
            ParentingOperation operation = parentingQueue.Dequeue();
            ExecuteParentingOperation(operation);
        }
        
        isProcessingQueue = false;
    }

    private void ExecuteParentingOperation(ParentingOperation operation)
    {
        if (operation.Object == null) return;

        switch (operation.Type)
        {
            case OperationType.Parent:
                if (operation.TargetChunk != null && operation.TargetChunk.gameObject.activeInHierarchy)
                {
                    DirectParentObject(operation.Object, operation.TargetChunk);
                }
                break;
                
            case OperationType.Unparent:
                DirectUnparentObject(operation.Object);
                break;
        }
    }

    
    public void ReportTriggerEnter(Chunk chunk, Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            QueueParentingOperation(other.transform, chunk, OperationType.Parent);
        }
    }

    public void ReportTriggerExit(Chunk chunk, Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Box"))
        {
            QueueParentingOperation(other.transform, null, OperationType.Unparent);
        }
    }

    private void QueueParentingOperation(Transform obj, Chunk targetChunk, OperationType type)
    {
        ParentingOperation operation = new ParentingOperation
        {
            Object = obj,
            TargetChunk = targetChunk,
            Type = type
        };
        
        parentingQueue.Enqueue(operation);
    }

    private void DirectParentObject(Transform obj, Chunk chunk)
    {
        if (obj == null || chunk == null) return;

        // Remove from current chunk if exists
        if (objectToChunkMap.ContainsKey(obj))
        {
            if (objectToChunkMap[obj] == chunk) return; // Already in correct chunk
            DirectUnparentObject(obj);
        }

        // Perform parenting
        obj.SetParent(chunk.transform);
        objectToChunkMap[obj] = chunk;
        
        Debug.Log($"Parented {obj.name} to chunk at {chunk.transform.position}");
    }

    private void DirectUnparentObject(Transform obj)
    {
        if (obj == null || !objectToChunkMap.ContainsKey(obj)) return;

        Chunk previousChunk = objectToChunkMap[obj];
        
        // Only unparent if the chunk is active and available
        if (previousChunk != null && previousChunk.gameObject.activeInHierarchy)
        {
            obj.SetParent(null);
            objectToChunkMap.Remove(obj);
            Debug.Log($"Unparented {obj.name} from chunk at {previousChunk.transform.position}");
        }
        else
        {
            // Chunk is being deactivated, just remove from mapping
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
                QueueParentingOperation(obj, correctChunk, OperationType.Parent);
            }
        }
        else
        {
            if (objectToChunkMap.ContainsKey(obj))
            {
                QueueParentingOperation(obj, null, OperationType.Unparent);
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


    public void ForceParentObject(Transform obj, Chunk chunk)
    {
        QueueParentingOperation(obj, chunk, OperationType.Parent);
    }

    public void ForceUnparentObject(Transform obj)
    {
        QueueParentingOperation(obj, null, OperationType.Unparent);
    }

    public bool IsObjectInAnyChunk(Transform obj)
    {
        return objectToChunkMap.ContainsKey(obj);
    }
}






