using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(PlayerInput))]
public class DragonNewControls : MonoBehaviour
{
    [Header("Tilemap References")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    
    [Header("Dragon Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private float snapThreshold = 0.01f;
    
    [Header("Body Chunks")]
    [SerializeField] private List<Transform> chunks = new List<Transform>();
    [SerializeField] private List<Vector3> positionHistory = new List<Vector3>();
    
    private PlayerControls controls;
    private Animator animator;
    private Vector2 movementInput = Vector2.zero;
    private string lastDirection = "Right";
    private bool isMoving = false;
    private bool isSmoothingMovement = false;
    private Vector3 targetPosition;
    
    // Movement variables
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;

    private void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        
        // Initialize chunks
        foreach (Transform child in transform)
        {
            if (child != transform) // Avoid adding self as a chunk
            {
                chunks.Add(child);
                positionHistory.Add(child.position);
            }
        }
        
        // Ensure head is first in list
        if (chunks.Count > 0 && chunks[0] != transform)
        {
            chunks.Insert(0, transform);
            positionHistory.Insert(0, transform.position);
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    
    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        controls.Player.Dragon.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Dragon.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            isMoving = false;
        };
        
        // Initialize current grid position
        currentGridPosition = groundTilemap.WorldToCell(transform.position);
    }

    void FixedUpdate()
    {
        isMoving = movementInput != Vector2.zero;
        // && CanMove(movementInput) && !isSmoothingMovement;

        Move(movementInput);
         Debug.Log(movementInput);

        // if (movementInput != Vector2.zero && !isSmoothingMovement)
        // {
        //     Move(movementInput);
        // }

        // HandleAnimations();
    }

    private void Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            Vector2 moveDirection = GetPrimaryDirection(direction);
            targetGridPosition = currentGridPosition + new Vector3Int((int)moveDirection.x, (int)moveDirection.y, 0);
            targetPosition = groundTilemap.GetCellCenterWorld(targetGridPosition);
            
            StartCoroutine(SmoothMovement());
            UpdateLastDirection(moveDirection);
        }
        else
        {
            isMoving = false;
        }
    }

    private IEnumerator SmoothMovement()
    {
        isSmoothingMovement = true;
        isMoving = true;
        
        // Update position history with new head position
        positionHistory.Insert(0, targetPosition);
        
        // If we haven't grown, remove the last position to maintain length
        if (positionHistory.Count > chunks.Count)
        {
            positionHistory.RemoveAt(positionHistory.Count - 1);
        }
        
        float remainingDistance = Vector3.Distance(transform.position, targetPosition);
        
        while (remainingDistance > float.Epsilon)
        {
            // Move head
            transform.position = Vector3.MoveTowards(
                transform.position, 
                targetPosition, 
                moveSpeed * Time.deltaTime
            );
            
            // Update body chunks to follow position history
            UpdateBodyChunks();
            
            remainingDistance = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }
        
        transform.position = targetPosition;
        currentGridPosition = targetGridPosition;
        isSmoothingMovement = false;
        
        // Final update of body chunks
        UpdateBodyChunks();
        
        if (movementInput != Vector2.zero && CanMove(movementInput))
        {
            Move(movementInput);
        }
        else
        {
            isMoving = false;
        }
    }

    private void UpdateBodyChunks()
    {
        for (int i = 1; i < chunks.Count; i++)
        {
            if (i < positionHistory.Count)
            {
                chunks[i].position = positionHistory[i];
            }
        }
    }

    private void HandleAnimations()
    {
        if (animator == null) return;
        string animationName = isMoving ? "Walking" : "Idle";
        animator.Play(animationName + lastDirection);
    }

    private void UpdateLastDirection(Vector2 direction)
    {
        if (direction.x > 0) lastDirection = "Right";
        else if (direction.x < 0) lastDirection = "Left";
        else if (direction.y > 0) lastDirection = "Up";
        else if (direction.y < 0) lastDirection = "Down";
    }

    private bool CanMove(Vector2 direction)
    {
        Vector2 primaryDirection = GetPrimaryDirection(direction);
        Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)primaryDirection);
        
        // Check if target tile is valid
        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
            return false;
        
        // Check if dragon would intersect itself
        if (WouldIntersectSelf(gridPosition))
            return false;
            
        return true;
    }

    private bool WouldIntersectSelf(Vector3Int newHeadGridPosition)
    {
        Vector3 newHeadWorldPosition = groundTilemap.GetCellCenterWorld(newHeadGridPosition);
        
        // Check if new position would intersect with any body chunk
        for (int i = 1; i < chunks.Count; i++)
        {
            if (Vector3.Distance(newHeadWorldPosition, chunks[i].position) < 0.4f * gridSize)
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetPrimaryDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            return new Vector2(Mathf.Sign(input.x), 0);
        }
        else if (input.y != 0)
        {
            return new Vector2(0, Mathf.Sign(input.y));
        }
        return Vector2.zero;
    }

    // Public method to check if a position is inside the dragon's body
    public bool IsPositionInsideDragon(Vector3 worldPosition)
    {
        foreach (Transform chunk in chunks)
        {
            if (Vector3.Distance(worldPosition, chunk.position) < 0.3f * gridSize)
            {
                return true;
            }
        }
        return false;
    }

    // Public method to get all dragon chunk positions (for fairy movement validation)
    public List<Vector3> GetDragonChunkPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform chunk in chunks)
        {
            positions.Add(chunk.position);
        }
        return positions;
    }

    // Method to add new chunk when dragon grows
    public void AddChunk(Transform newChunk)
    {
        chunks.Add(newChunk);
        positionHistory.Add(newChunk.position);
    }
}



//     public event Action<Vector2, bool> OnMovementAttempted;

//     [Header("Movement Settings")]
//     [SerializeField] private float moveSpeed = 5f;
//     [SerializeField] private float gridSize = 8f;
//     [SerializeField] private float snapThreshold = 0.01f;



//     [Header("Tilemap References")]
//     [SerializeField] private Tilemap groundTilemap;
//     // [SerializeField] private Tilemap collisionTilemap;
//     // [SerializeField] private Tilemap dragonTilemap;
//     // [SerializeField] private TileBase headTile;
//     // [SerializeField] private TileBase bodyTile;

//     [Header("Body Chunks")]
//     [SerializeField] private List<Transform> Chunks = new List<Transform>();
//     [SerializeField] private List<Vector2> positionHistory = new List<Vector2>();
   
//      private PlayerControls controls;
//     private Animator animator;
//     private Vector2 movementInput = Vector2.zero;
//     private string lastDirection = "Down";
//     private bool isMoving = false;

//     private Vector3 targetPosition;
//     private bool isSmoothingMovement = false;
//     private int Transform;

//     // private PlayerControls playerControls;
//     // private bool isMoving = false;
//     // private Vector3 targetWorldPos;
//     // private Vector2 movementInput;
//     // private Vector3Int currentHeadCell;
//     // private List<Vector3Int> bodyCells = new List<Vector3Int>();
//     // private List<Vector3Int> positionHistory = new List<Vector3Int>();

//     // private void Awake()
//     // {
//     //     playerControls = new PlayerControls();
//     //     animator = GetComponent<Animator>();



//     //     // Initialize position history for all chunks
//     //     foreach (Transform child in transform)
//     //     {
//     //         Chunks.Add(child);
//     //         positionHistory.Add(child.position);
//     //     }

//     //     Transform = Chunks.Count;

//     // }

//     private void Awake()
//     {
//         controls = new PlayerControls();
//         animator = GetComponent<Animator>();

//         foreach (Transform child in transform)
//         {

//             Chunks.Add(child);
//             positionHistory.Add(child.position);
//         }

//        Transform = Chunks.Count;
     
//     }

//     private void OnEnable()
//     {
//         controls.Enable();
//     }
    
//     private void OnDisable()
//     {
//         controls.Disable();
//     }


//    void Start()
//     {
        
//         controls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
//         controls.Player.Fey.canceled += ctx =>
//         {
//             movementInput = Vector2.zero;
//             isMoving = false;
//         };
//     }

//     void FixedUpdate()
//     {
//         isMoving = movementInput != Vector2.zero && CanMove(movementInput);

//         if (movementInput != Vector2.zero && !isSmoothingMovement)
//         {
//             Move(movementInput);
//             Debug.Log(movementInput);
//         }
        
//        // HandleAnimations();
//     }

//     private void Move(Vector2 direction)
//     {
//         if (CanMove(direction))
//         {
//             Vector2 newHeadPosition = (Vector2)transform.position + direction * gridSize;
            
//             if (WouldIntersectSelf(newHeadPosition))
//             {
//                 OnMovementAttempted?.Invoke(direction, false);
//                 return;
//             }

//             targetPosition = newHeadPosition;
//           //  StartCoroutine(SmoothMovement(direction));
//             UpdateLastDirection(direction);
//         }
//         else
//         {
//             isMoving = false;
//             OnMovementAttempted?.Invoke(direction, false);
//         }
//     }


//      private IEnumerator SmoothMovement(Vector2 direction)
//     {
//         isSmoothingMovement = true;
//         isMoving = true;
        
//         // Store the starting position for all chunks
//         List<Vector2> startPositions = new List<Vector2>();
//         foreach (var chunk in Chunks)
//         {
//             startPositions.Add(chunk.position);
//         }
        
//         float remainingDistance = Vector3.Distance(transform.position, targetPosition);
        
//         while (remainingDistance > float.Epsilon)
//         {
//             // Move the head
//             transform.position = Vector3.MoveTowards(
//                 transform.position, 
//                 targetPosition, 
//                 moveSpeed * Time.deltaTime
//             );
            
//             // Update body positions based on history (like snake movement)
//             UpdateBodyPositions();
            
//             remainingDistance = Vector3.Distance(transform.position, targetPosition);
//             yield return null;
//         }
        
//         // Final position snap
//         transform.position = targetPosition;
//         UpdateBodyPositions();
        
//         isSmoothingMovement = false;
        
//         OnMovementAttempted?.Invoke(direction, true);
//     }



//       private void UpdateBodyPositions()
//     {
//         // Add current head position to history
//         if (positionHistory.Count == 0 || Vector2.Distance(positionHistory[0], transform.position) > 0.1f)
//         {
//             positionHistory.Insert(0, transform.position);
            
//             // Keep history length matching chunk count
//             if (positionHistory.Count > Chunks.Count)
//             {
//                 positionHistory.RemoveAt(positionHistory.Count - 1);
//             }
//         }
        
//         // Update each body chunk position
//         for (int i = 1; i < Chunks.Count; i++)
//         {
//             if (i < positionHistory.Count)
//             {
//                 Chunks[i].position = positionHistory[i];
//             }
//         }
//     }

//      private void UpdateLastDirection(Vector2 direction)
//     {
//         if (direction.x > 0) lastDirection = "Right";
//         else if (direction.x < 0) lastDirection = "Left";
//         else if (direction.y > 0) lastDirection = "Up";
//         else if (direction.y < 0) lastDirection = "Down";
//     }

//     private bool CanMove(Vector2 direction)
//     {
//         // return true;
//         Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)(direction * gridSize));
//         //     if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
//         //         return false;
//             return true;
//     }

//     private Vector2 GetPrimaryDirection(Vector2 input)
//     {
//         if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
//         {
//             return new Vector2(Mathf.Sign(input.x), 0);
//         }
//         else if (input.y != 0)
//         {
//             return new Vector2(0, Mathf.Sign(input.y));
//         }
//         return Vector2.zero;
//     }

//     private bool WouldIntersectSelf(Vector2 newHeadPosition)
//     {
//         for (int i = 1; i < Chunks.Count; i++)
//         {
//             if (Vector2.Distance(newHeadPosition, Chunks[i].position) < 0.4f * gridSize)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     // private void Update()
//     // {
//     //     if (isMoving)
//     //     {
//     //         // Smooth movement towards target position
//     //         transform.position = Vector3.MoveTowards(
//     //             transform.position,
//     //             targetWorldPos,
//     //             moveSpeed * Time.deltaTime
//     //         );

//     //         // Check if we've reached the target
//     //         if (Vector3.Distance(transform.position, targetWorldPos) < snapThreshold)
//     //         {
//     //             transform.position = targetWorldPos;
//     //             isMoving = false;

//     //             // Update the dragon visualization
//     //             UpdateDragonVisualization();
//     //         }
//     //     }
//     // }

//     // public void OnMovementPerformed(InputAction.CallbackContext context)
//     // {
//     //     if (isMoving) return;

//     //     movementInput = context.ReadValue<Vector2>();
//     //     Vector2 moveDirection = GetPrimaryDirection(movementInput);

//     //     if (moveDirection != Vector2.zero)
//     //     {
//     //         TryToMove(moveDirection);
//     //     }
//     // }

//     // private bool TryToMove(Vector2 direction)
//     // {
//     //     // Convert direction to cell movement
//     //     Vector3Int moveDirection = new Vector3Int((int)direction.x, (int)direction.y, 0);
//     //     Vector3Int newHeadCell = currentHeadCell + moveDirection;

//     //     // Check if the new position is valid
//     //     if (!IsValidPosition(newHeadCell))
//     //     {
//     //         OnMovementAttempted?.Invoke(direction, false);
//     //         return false;
//     //     }

//     //     // Check if the new position would intersect with the body
//     //     if (WouldIntersectSelf(newHeadCell))
//     //     {
//     //         OnMovementAttempted?.Invoke(direction, false);
//     //         return false;
//     //     }

//     //     // Start movement
//     //     currentHeadCell = newHeadCell;
//     //     targetWorldPos = groundTilemap.GetCellCenterWorld(currentHeadCell);
//     //     isMoving = true;
        
//     //     // Add to position history
//     //     positionHistory.Insert(0, currentHeadCell);
        
//     //     // Keep position history the same length as body parts
//     //     if (positionHistory.Count > bodyCells.Count)
//     //     {
//     //         positionHistory.RemoveAt(positionHistory.Count - 1);
//     //     }

//     //     OnMovementAttempted?.Invoke(direction, true);
//     //     return true;
//     // }

//     // private void UpdateDragonVisualization()
//     // {
//     //     // Clear all dragon tiles
//     //     foreach (Vector3Int cell in bodyCells)
//     //     {
//     //         dragonTilemap.SetTile(cell, null);
//     //     }
        
//     //     // Update body cells from position history
//     //     for (int i = 0; i < bodyCells.Count; i++)
//     //     {
//     //         if (i < positionHistory.Count)
//     //         {
//     //             bodyCells[i] = positionHistory[i];
//     //         }
//     //     }
        
//     //     // Set tiles for the dragon
//     //     for (int i = 0; i < bodyCells.Count; i++)
//     //     {
//     //         if (i == 0)
//     //         {
//     //             dragonTilemap.SetTile(bodyCells[i], headTile);
//     //         }
//     //         else
//     //         {
//     //             dragonTilemap.SetTile(bodyCells[i], bodyTile);
//     //         }
//     //     }
//     // }

//     // private bool IsValidPosition(Vector3Int cellPosition)
//     // {
//     //     // Check if the cell has ground and no collision
//     //     return groundTilemap.HasTile(cellPosition) && !collisionTilemap.HasTile(cellPosition);
//     // }

//     // private Vector2 GetPrimaryDirection(Vector2 input)
//     // {
//     //     if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
//     //     {
//     //         return new Vector2(Mathf.Sign(input.x), 0);
//     //     }
//     //     else if (input.y != 0)
//     //     {
//     //         return new Vector2(0, Mathf.Sign(input.y));
//     //     }
//     //     return Vector2.zero;
//     // }

//     // private bool WouldIntersectSelf(Vector3Int newHeadCell)
//     // {
//     //     // Check if the new position would intersect with any body part
//     //     // Skip the first few cells to allow movement through the tail
//     //     for (int i = 1; i < bodyCells.Count - 1; i++)
//     //     {
//     //         if (newHeadCell == bodyCells[i])
//     //         {
//     //             return true;
//     //         }
//     //     }
//     //     return false;
//     // }
    
//     // // Call this when you want to grow the dragon
//     // public void GrowDragon()
//     // {
//     //     Vector3Int newBodyCell = positionHistory[positionHistory.Count - 1];
//     //     bodyCells.Add(newBodyCell);
//     //     positionHistory.Add(newBodyCell);
        
//     //     // Update the visualization
//     //     UpdateDragonVisualization();
//     // }
    
//     // // Helper method to visualize the dragon in the editor
//     // private void OnDrawGizmosSelected()
//     // {
//     //     if (bodyCells == null) return;
        
//     //     Gizmos.color = Color.red;
//     //     foreach (Vector3Int cell in bodyCells)
//     //     {
//     //         Vector3 worldPos = groundTilemap.GetCellCenterWorld(cell);
//     //         Gizmos.DrawWireCube(worldPos, groundTilemap.cellSize);
//     //     }
//     // }
// }