using UnityEngine;

public class Skyfall : MonoBehaviour
{
    [Header("Skyfall Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float maxFallSpeed = 15f;
    [SerializeField] private float fallAcceleration = 25f;
    [SerializeField] private float snapThreshold = 0.01f;

    [Header("Object Type")]
    [SerializeField] private string objectTag = "Untagged";

    [Header("Abyss Settings")]
    [SerializeField] private LayerMask boundaryLayer;
    [SerializeField] private float abyssThresholdY = -10f;
    [SerializeField] private float boundaryCheckDistance = 0.5f;
    [SerializeField] private float abyssDestroyDelay = 2f;

    private bool isFalling = false;
    private bool isInAbyss = false;
    private float currentFallVelocity = 0f;
    private bool wasParentedLastFrame = true;
    private bool movementLocked = false;

    // Events for other scripts to listen to
    public System.Action OnStartFalling;
    public System.Action OnStopFalling;
    public System.Action OnEnterAbyss;

    private void Update()
    {
        if (!isInAbyss)
        {
            CheckChunkParentingStatus();
            HandleFalling();
            CheckForBoundary();
        }
    }

    private void CheckForBoundary()
    {
        if (!isFalling || movementLocked) return;

        // Check if we hit a boundary below us
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            boundaryCheckDistance,
            boundaryLayer
        );

        if (hit.collider != null)
        {
            // We hit a boundary - stop falling and position ourselves on the boundary
            transform.position = new Vector3(
                transform.position.x,
                hit.point.y + boundaryCheckDistance, // Position above the boundary
                transform.position.z
            );
            StopFalling();
        }
        else
        {
            // No boundary detected - we're falling into the abyss!
            if (!isInAbyss)
            {
                EnterAbyss();
            }
        }
    }

    private void EnterAbyss()
    {
        isInAbyss = true;
        isFalling = false;

        Debug.Log($"{gameObject.name} is falling into the abyss!");
        OnEnterAbyss?.Invoke();

        // Start the destruction countdown
        StartCoroutine(DestroyAfterAbyss());
    }

    private System.Collections.IEnumerator DestroyAfterAbyss()
    {
        // Optional: Add visual/audio effects here
        // Example: fade out, play falling sound, etc.

        yield return new WaitForSeconds(abyssDestroyDelay);

        // Destroy the object
        Debug.Log("DEstroyedAAAAAAAAAAAAAAAA");
        // Destroy(gameObject);
    }

    private void CheckChunkParentingStatus()
    {
        if (movementLocked || isInAbyss) return;

        bool isCurrentlyParented = IsParentedToChunk();

        // Start falling if we just lost parenting
        if (!isCurrentlyParented && wasParentedLastFrame && !isFalling)
        {
            StartFalling();
        }
        // Stop falling if we just gained parenting
        else if (isCurrentlyParented && !wasParentedLastFrame && isFalling)
        {
            StopFalling();
        }

        wasParentedLastFrame = isCurrentlyParented;
    }

    private void StartFalling()
    {
        if (isFalling || isInAbyss) return;

        Debug.Log($"{gameObject.name} started falling - no chunk parent!");
        isFalling = true;
        currentFallVelocity = 0f;

        // Unparent completely to ensure clean falling
        transform.SetParent(null);

        OnStartFalling?.Invoke();
    }


    private void StopFalling()
    {
        if (!isFalling || isInAbyss) return;

        Debug.Log($"{gameObject.name} stopped falling - found chunk parent!");
        isFalling = false;
        currentFallVelocity = 0f;

        // Snap to grid when landing
        SnapToGrid();

        OnStopFalling?.Invoke();
    }



private void HandleFalling()
{
    if (!isFalling || movementLocked || isInAbyss) return;


    currentFallVelocity += fallAcceleration * Time.deltaTime;
    currentFallVelocity = Mathf.Min(currentFallVelocity, maxFallSpeed);


    Vector3 newPosition = transform.position + Vector3.down * currentFallVelocity * Time.deltaTime;
    transform.position = newPosition;


    CheckForChunkAtPosition(newPosition);

   
    if (transform.position.y < abyssThresholdY)
    {
        EnterAbyss();
    }
}
    // private void HandleFalling()
    // {
    //     if (!isFalling || movementLocked || isInAbyss) return;

    //     // Accelerate falling speed
    //     currentFallVelocity += fallAcceleration * Time.deltaTime;
    //     currentFallVelocity = Mathf.Min(currentFallVelocity, maxFallSpeed);

    //     // Move downward
    //     Vector3 newPosition = transform.position + Vector3.down * currentFallVelocity * Time.deltaTime;
    //     transform.position = newPosition;

    //     // Check if we should be parented to any chunk at our new position
    //     CheckForChunkAtPosition(newPosition);
    // }

    private void CheckForChunkAtPosition(Vector3 position)
    {
        // Use ChunkManager to find if we're in any chunk at current position
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.ValidateObjectParenting(transform, objectTag);

            // If we're now parented, stop falling
            if (IsParentedToChunk())
            {
                StopFalling();
            }
        }
    }

    public bool IsParentedToChunk()
    {
        // Check direct parenting
        if (transform.parent != null && transform.parent.GetComponent<Chunk>() != null)
            return true;

        // Check through ChunkManager for redundancy
        if (ChunkManager.Instance != null && ChunkManager.Instance.IsObjectInAnyChunk(transform))
            return true;

        return false;
    }


    // private void Update()
    // {
    //     CheckChunkParentingStatus();
    //     HandleFalling();
    // }

    // private void CheckChunkParentingStatus()
    // {
    //     if (movementLocked) return;

    //     bool isCurrentlyParented = IsParentedToChunk();

    //     // Start falling if we just lost parenting
    //     if (!isCurrentlyParented && wasParentedLastFrame && !isFalling)
    //     {
    //         StartFalling();
    //     }
    //     // Stop falling if we just gained parenting
    //     else if (isCurrentlyParented && !wasParentedLastFrame && isFalling)
    //     {
    //         StopFalling();
    //     }

    //     wasParentedLastFrame = isCurrentlyParented;
    // }

    // private void StartFalling()
    // {
    //     if (isFalling) return;

    //     Debug.Log($"{gameObject.name} started falling - no chunk parent!");
    //     isFalling = true;
    //     currentFallVelocity = 0f;

    //     // Unparent completely to ensure clean falling
    //     transform.SetParent(null);

    //     OnStartFalling?.Invoke();
    // }

    // private void StopFalling()
    // {
    //     if (!isFalling) return;

    //     Debug.Log($"{gameObject.name} stopped falling - found chunk parent!");
    //     isFalling = false;
    //     currentFallVelocity = 0f;

    //     // Snap to grid when landing
    //     SnapToGrid();

    //     OnStopFalling?.Invoke();
    // }

    // private void HandleFalling()
    // {
    //     if (!isFalling || movementLocked) return;

    //     // Accelerate falling speed
    //     currentFallVelocity += fallAcceleration * Time.deltaTime;
    //     currentFallVelocity = Mathf.Min(currentFallVelocity, maxFallSpeed);

    //     // Move downward
    //     Vector3 newPosition = transform.position + Vector3.down * currentFallVelocity * Time.deltaTime;
    //     transform.position = newPosition;

    //     // Check if we should be parented to any chunk at our new position
    //     CheckForChunkAtPosition(newPosition);
    // }

    // private void CheckForChunkAtPosition(Vector3 position)
    // {
    //     // Use ChunkManager to find if we're in any chunk at current position
    //     if (ChunkManager.Instance != null)
    //     {
    //         ChunkManager.Instance.ValidateObjectParenting(transform, objectTag);

    //         // If we're now parented, stop falling
    //         if (IsParentedToChunk())
    //         {
    //             StopFalling();
    //         }
    //     }
    // }

    // public bool IsParentedToChunk()
    // {
    //     // Check direct parenting
    //     if (transform.parent != null && transform.parent.GetComponent<Chunk>() != null)
    //         return true;

    //     // Check through ChunkManager for redundancy
    //     if (ChunkManager.Instance != null && ChunkManager.Instance.IsObjectInAnyChunk(transform))
    //         return true;

    //     return false;
    // }

    private void SnapToGrid()
    {
        transform.position = GetGridAlignedPosition(transform.position);
    }

    private Vector3 GetGridAlignedPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            position.z
        );
    }

    // Public API for other scripts to control this component
    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        // If we're unlocking movement, check if we need to start falling
        if (!locked && !isInAbyss)
        {
            CheckChunkParentingStatus();
        }
    }

    public bool IsFalling()
    {
        return isFalling;
    }

    public bool IsInAbyss()
    {
        return isInAbyss;
    }

    public void ForceParentingCheck()
    {
        CheckChunkParentingStatus();
    }

    public void SetObjectTag(string newTag)
    {
        objectTag = newTag;
    }

     private void OnDrawGizmos()
    {
        if (isFalling)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
            
            // Draw fall velocity indicator
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Vector3.down * (currentFallVelocity * 0.1f));
            
            // Draw boundary check ray
            Gizmos.color = isInAbyss ? Color.red : Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.down * boundaryCheckDistance);
        }
        
        if (isInAbyss)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }


    // private void OnDrawGizmos()
    // {
    //     if (isFalling)
    //     {
    //         Gizmos.color = Color.blue;
    //         Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);

    //         // Draw fall velocity indicator
    //         Gizmos.color = Color.cyan;
    //         Gizmos.DrawRay(transform.position, Vector3.down * (currentFallVelocity * 0.1f));
    //     }
    // }

}












/*

using UnityEngine;

public class Skyfall : MonoBehaviour
{
    [Header("Skyfall Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float fallSpeed = 8f;
    [SerializeField] private float maxFallSpeed = 15f;
    [SerializeField] private float fallAcceleration = 25f;
    [SerializeField] private float snapThreshold = 0.01f;

    [Header("Object Type")]
    [SerializeField] private string objectTag = "Untagged"; // "Player", "Box", etc.

    private bool isFalling = false;
    private float currentFallVelocity = 0f;
    private bool wasParentedLastFrame = true;
    private bool movementLocked = false;

    // Events for other scripts to listen to
    public System.Action OnStartFalling;
    public System.Action OnStopFalling;

    private void Update()
    {
        CheckChunkParentingStatus();
        HandleFalling();
    }

    private void CheckChunkParentingStatus()
    {
        if (movementLocked) return;

        bool isCurrentlyParented = IsParentedToChunk();

        // Start falling if we just lost parenting
        if (!isCurrentlyParented && wasParentedLastFrame && !isFalling)
        {
            StartFalling();
        }
        // Stop falling if we just gained parenting
        else if (isCurrentlyParented && !wasParentedLastFrame && isFalling)
        {
            StopFalling();
        }

        wasParentedLastFrame = isCurrentlyParented;
    }

    private void StartFalling()
    {
        if (isFalling) return;

        Debug.Log($"{gameObject.name} started falling - no chunk parent!");
        isFalling = true;
        currentFallVelocity = 0f;

        // Unparent completely to ensure clean falling
        transform.SetParent(null);

        OnStartFalling?.Invoke();
    }

    private void StopFalling()
    {
        if (!isFalling) return;

        Debug.Log($"{gameObject.name} stopped falling - found chunk parent!");
        isFalling = false;
        currentFallVelocity = 0f;

        // Snap to grid when landing
        SnapToGrid();

        OnStopFalling?.Invoke();
    }

    private void HandleFalling()
    {
        if (!isFalling || movementLocked) return;

        // Accelerate falling speed
        currentFallVelocity += fallAcceleration * Time.deltaTime;
        currentFallVelocity = Mathf.Min(currentFallVelocity, maxFallSpeed);

        // Move downward
        Vector3 newPosition = transform.position + Vector3.down * currentFallVelocity * Time.deltaTime;
        transform.position = newPosition;

        // Check if we should be parented to any chunk at our new position
        CheckForChunkAtPosition(newPosition);
    }

    private void CheckForChunkAtPosition(Vector3 position)
    {
        // Use ChunkManager to find if we're in any chunk at current position
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.ValidateObjectParenting(transform, objectTag);

            // If we're now parented, stop falling
            if (IsParentedToChunk())
            {
                StopFalling();
            }
        }
    }

    public bool IsParentedToChunk()
    {
        // Check direct parenting
        if (transform.parent != null && transform.parent.GetComponent<Chunk>() != null)
            return true;

        // Check through ChunkManager for redundancy
        if (ChunkManager.Instance != null && ChunkManager.Instance.IsObjectInAnyChunk(transform))
            return true;

        return false;
    }

    private void SnapToGrid()
    {
        transform.position = GetGridAlignedPosition(transform.position);
    }

    private Vector3 GetGridAlignedPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            position.z
        );
    }

    // Public API for other scripts to control this component
    public void SetMovementLocked(bool locked)
    {
        movementLocked = locked;

        // If we're unlocking movement, check if we need to start falling
        if (!locked)
        {
            CheckChunkParentingStatus();
        }
    }

    public bool IsFalling()
    {
        return isFalling;
    }

    public void ForceParentingCheck()
    {
        CheckChunkParentingStatus();
    }

    public void SetObjectTag(string newTag)
    {
        objectTag = newTag;
    }

    private void OnDrawGizmos()
    {
        if (isFalling)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);

            // Draw fall velocity indicator
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Vector3.down * (currentFallVelocity * 0.1f));
        }
    }
    
}








*/