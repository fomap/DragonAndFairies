using System.Collections;
using UnityEngine;

public class Skyfall : MonoBehaviour
{
    [Header("Skyfall Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float maxFallSpeed = 0.2f;
    [SerializeField] private float fallAcceleration = 0.2f;

    [Header("Object Type")]
    [SerializeField] private string objectTag = "Untagged";

    [Header("Abyss Settings")]
    [SerializeField] private LayerMask boundaryLayer;
    [SerializeField] private LayerMask wallLayer ;
    [SerializeField] private float boundaryCheckDistance = 0.5f;
    [SerializeField] private float abyssDestroyDelay = 2f;

    // Events
    public System.Action OnStartFalling;
    public System.Action OnStopFalling;
    public System.Action OnEnterAbyss;

    private bool isFalling = false;
    private bool isInAbyss = false;
    private float currentFallVelocity = 0f;
    private bool wasParentedLastFrame = true;
    private bool movementLocked = false;

    private void FixedUpdate()
    {
        if (!isInAbyss)
        {
            CheckChunkParentingStatus();
            HandleFalling();
            CheckForBoundary();
        }
    }

    // private void CheckForBoundary()
    // {
    //     if (!isFalling) return;


    //     int boundaryOnlyLayer = 3; 

    //     RaycastHit2D hit = Physics2D.Raycast(
    //         transform.position,
    //         Vector2.down,
    //         boundaryCheckDistance,
    //         boundaryOnlyLayer 
    //     );

    //     Debug.Log($"{gameObject.name}: Boundary check - Hit: {hit.collider?.name ?? "None"}");

    //     if (hit.collider == null && !isInAbyss)
    //     {
    //         Debug.Log($"{gameObject.name}: No boundary below, entering abyss");
    //         EnterAbyss();
    //     }
    // }


    private void CheckForBoundary()
{
    if (!isFalling) return;

    // Ray should detect both walls + boundaries
    int groundMask = wallLayer | boundaryLayer;

    RaycastHit2D hit = Physics2D.Raycast(
        transform.position,
        Vector2.down,
        boundaryCheckDistance,
        groundMask
    );

    if (hit.collider == null)
    {
        // Nothing at all beneath Fey → abyss
        if (!isInAbyss)
        {
            Debug.Log($"{gameObject.name}: No ground detected, entering abyss");
            EnterAbyss();
        }
        return;
    }

    int hitLayer = hit.collider.gameObject.layer;

    if (((1 << hitLayer) & wallLayer) != 0)
    {
        Debug.Log($"{gameObject.name}: Wall detected: {hit.collider.name}");
        if (!isInAbyss) EnterAbyss();
    }
    else if (((1 << hitLayer) & boundaryLayer) != 0)
    {
        Debug.Log($"{gameObject.name}: Boundary detected: {hit.collider.name}");
    }
}

    // private void CheckForBoundary()
    // {
    //     // if (!isFalling || movementLocked) return;
    //     if (!isFalling)
    //     {
    //         Debug.Log($"{gameObject.name}: Not falling, skipping boundary check");
    //         return;

    //     }

    //     RaycastHit2D hit = Physics2D.Raycast(
    //         transform.position,
    //         Vector2.down,
    //         boundaryCheckDistance,
    //         boundaryLayer
    //     );

    //     Debug.Log($"{gameObject.name}: Boundary check - Position: {transform.position}, Hit: {hit.collider?.name ?? "None"}, Distance: {hit.distance}");


    //     if (hit.collider == null && !isInAbyss)
    //     {
    //         Debug.Log($"{gameObject.name}: No boundary below, entering abyss");
    //         EnterAbyss();
    //     }
    //     else if (!isInAbyss)
    //     {
    //         Debug.Log($"{gameObject.name}: Boundary detected: {hit.collider.name} at position {hit.collider.transform.position}");
    //     }
    // }

    private void EnterAbyss()
    {
        isInAbyss = true;
        isFalling = false;

        Debug.Log($"{gameObject.name} is falling into the abyss!");
        OnEnterAbyss?.Invoke();

        StartCoroutine(DestroyAfterAbyss());
    }

    private IEnumerator DestroyAfterAbyss()
    {
         yield return new WaitForSeconds(abyssDestroyDelay);

        Debug.Log($"Destroyed {gameObject.name} that fell into abyss");

        isFalling = false;
        GlobalSkyfallEventManager.Instance?.NotifyStopFalling(); 
        Destroy(gameObject);
    }

    private void CheckChunkParentingStatus()
    {
        //if (movementLocked || isInAbyss) return;
        if (isInAbyss) return;

        bool isCurrentlyParented = IsParentedToChunk();

        if (!isCurrentlyParented && wasParentedLastFrame && !isFalling)
        {
            StartFalling();
        }
        else if (isCurrentlyParented && !wasParentedLastFrame && isFalling)
        {
            StopFalling();
        }

        wasParentedLastFrame = isCurrentlyParented;
    }


 
    private void StartFalling()
    {
        if (isFalling || isInAbyss) return;

        isFalling = true;
        currentFallVelocity = 0f;
        transform.SetParent(null);

        OnStartFalling?.Invoke();

        Debug.Log($" ok imagine {gameObject.name} is falling ");
        //yield return new WaitForEndOfFrame();

        GlobalSkyfallEventManager.Instance?.NotifyStartFalling();


    }

    private void StopFalling()
    {
        if (!isFalling || isInAbyss) return;

        isFalling = false;
        currentFallVelocity = 0f;
        
        SnapToGrid();


        OnStopFalling?.Invoke();
        GlobalSkyfallEventManager.Instance?.NotifyStopFalling();
    }





    private void HandleFalling()
    {
        //if (!isFalling || movementLocked || isInAbyss) return;
        if (!isFalling || isInAbyss) return;

        currentFallVelocity += fallAcceleration * Time.deltaTime;
        currentFallVelocity = Mathf.Min(currentFallVelocity, maxFallSpeed);

        Vector3 newPosition = transform.position + Vector3.down * currentFallVelocity * Time.deltaTime;
        transform.position = newPosition;

        CheckForChunkAtPosition(newPosition);
    }

    private void CheckForChunkAtPosition(Vector3 position)
    {
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.ValidateObjectParenting(transform, objectTag);

            if (IsParentedToChunk())
            {

                StopFalling();
            }
        }
    }


    public bool IsParentedToChunk()
    {
        if (transform.parent != null && transform.parent.GetComponent<Chunk>() != null)
            return true;

        if (ChunkManager.Instance != null && ChunkManager.Instance.IsObjectInAnyChunk(transform))
            return true;

        return false;
    }

    private void SnapToGrid()
    {
        Vector3 gridPosition = GetGridAlignedPosition(transform.position);


        if (objectTag == "Box")
        {
            gridPosition += Vector3.down * gridSize;
             //transform.position = FindNonOverlappingPosition(gridPosition);
        }
        
        transform.position = gridPosition;
        Debug.Log($"{gameObject.name} snapped to: {transform.position}");
    }

    private Vector3 GetGridAlignedPosition(Vector3 position)
    {

        return new Vector3(
        Mathf.Round(position.x / gridSize) * gridSize,
        Mathf.Round(position.y / gridSize) * gridSize,
        position.z
        );


    }


    public void SetMovementLocked(bool locked) => movementLocked = locked;
    public bool IsFalling() => isFalling;
    public bool IsInAbyss() => isInAbyss;
    public void ForceParentingCheck() => CheckChunkParentingStatus();
    public void SetObjectTag(string newTag) => objectTag = newTag;

    private void OnDrawGizmos()
    {
        if (isFalling)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, Vector3.down * (currentFallVelocity * 0.1f));

            Gizmos.color = isInAbyss ? Color.red : Color.yellow;
            Gizmos.DrawRay(transform.position, Vector3.down * boundaryCheckDistance);
        }

        if (isInAbyss)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}



