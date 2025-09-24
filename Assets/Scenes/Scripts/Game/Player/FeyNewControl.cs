using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeyNewControl : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float boxMoveSpeed = 8f;
    [SerializeField] private float snapThreshold = 0.01f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    private string lastDirection = "Down";

    [Header("Win Settings")]
    [SerializeField] public int minBoxesNumber = 1;
    [SerializeField] private string nextLevel = "";

    public static int currentBoxes;

    private PlayerControls controls;
    private Rigidbody2D rb;

    // Movement state
    private Vector2 movementInput;
    private Vector3 targetPosition;
    private bool isSmoothingMovement = false;
    private bool isMoving = false;
    private bool movementEnabled = true;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        currentBoxes = 0;

        if (animator == null)
            animator = GetComponent<Animator>();

        // Snap to grid on start
        SnapToGrid();
    }

    private void Update()
    {
        // Safety net: occasionally snap to grid if not moving
        if (!isSmoothingMovement && !isMoving && movementEnabled)
        {
            if (Vector2.Distance(transform.position, GetGridAlignedPosition(transform.position)) > 0.01f)
            {
                SnapToGrid();
            }
        }

        if (Time.frameCount % 30 == 0) // Check every 30 frames
    {
        CheckChunkParenting();
    }
    }

    private void OnEnable()
    {
        controls.Enable();
        PlayerOneMovement.OnDragonMovementStarted += DisableFairyMovement;
        PlayerOneMovement.OnDragonMovementCompleted += EnableFairyMovement;
    }

    private void OnDisable()
    {
        controls.Disable();
        PlayerOneMovement.OnDragonMovementStarted -= DisableFairyMovement;
        PlayerOneMovement.OnDragonMovementCompleted -= EnableFairyMovement;
    }

    void Start()
    {
        controls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Fey.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            isMoving = false;
        };
    }

    void FixedUpdate()
    {
        if (!movementEnabled) return;

        if (movementInput != Vector2.zero && !isSmoothingMovement)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }

        HandleAnimations();
    }

    private void DisableFairyMovement()
    {
        movementEnabled = false;
        movementInput = Vector2.zero;
        isMoving = false;

        if (isSmoothingMovement)
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
            // Snap to grid when movement is interrupted
            SnapToGrid();
        }
    }

    
    private void EnableFairyMovement()
    {
        movementEnabled = true;
    }

    private void TryMove(Vector2 direction)
    {
        // Use grid-aligned position calculation
        Vector2 newPos = GetGridAlignedPosition((Vector2)transform.position + (direction * gridSize));

        if (Physics2D.OverlapCircle(newPos, 0.2f, wallLayer))
        {
            isMoving = false;
            return;
        }

        Collider2D box = Physics2D.OverlapCircle(newPos, 0.2f, boxLayer);
        if (box != null)
        {
            Vector2 newBoxPos = GetGridAlignedPosition(newPos + (direction * gridSize));
            if (Physics2D.OverlapCircle(newBoxPos, 0.2f, wallLayer | boxLayer))
            {
                isMoving = false;
                return;
            }
            StartCoroutine(MoveBoxSmoothly(box.transform, newBoxPos));
        }

        targetPosition = newPos;
        StartCoroutine(SmoothMovement());
        UpdateLastDirection(direction);
    }

    private IEnumerator SmoothMovement()
    {
        isSmoothingMovement = true;
        isMoving = true;

        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, targetPosition);
        float startTime = Time.time;

        while (journeyLength > snapThreshold && movementEnabled)
        {
            if (!movementEnabled)
            {
                isSmoothingMovement = false;
                isMoving = false;
                SnapToGrid(); // Snap when interrupted
                yield break;
            }

            // Use Lerp for smoother movement that always ends at exact position
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            journeyLength = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }

        if (movementEnabled)
        {
            // Ensure exact grid position at the end
            transform.position = targetPosition;
            SnapToGrid();

            isSmoothingMovement = false;
            isMoving = false;

            //ChunkManager.Instance?.ValidateObjectParenting(transform, "Player");

            // Check for continuous input
            if (movementInput != Vector2.zero)
            {
                Vector2 moveDirection = GetPrimaryDirection(movementInput);
                if (moveDirection != Vector2.zero)
                {
                    TryMove(moveDirection);
                }
            }
        }
        else
        {
            SnapToGrid();
            isSmoothingMovement = false;
            isMoving = false;
        }
    }

    // Grid snapping functions from your old code
    private Vector2 GetGridAlignedPosition(Vector2 position)
    {
        return new Vector2(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize
        );
    }

    private void SnapToGrid()
    {
        transform.position = GetGridAlignedPosition(transform.position);
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

    private IEnumerator MoveBoxSmoothly(Transform box, Vector2 targetPosition)
    {
        Vector2 startPosition = box.position;
        Vector2 alignedTarget = GetGridAlignedPosition(targetPosition);
        float journeyLength = Vector2.Distance(startPosition, alignedTarget);
        float startTime = Time.time;

        while (Vector2.Distance(box.position, alignedTarget) > snapThreshold && movementEnabled)
        {
            float distanceCovered = (Time.time - startTime) * boxMoveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            box.position = Vector2.Lerp(startPosition, alignedTarget, fractionOfJourney);
            yield return null;
        }

        if (movementEnabled)
        {
            box.position = alignedTarget;
            // Snap box to grid as well
            box.position = GetGridAlignedPosition(box.position);
        }
    }
    

    private void CheckChunkParenting()
{
    // Check if Fey thinks she's not parented but should be
    if (transform.parent == null)
    {
        // Force a validation check
        //ChunkManager.Instance?.ValidateObjectParenting(transform, "Player");
    }
}
}






// using System.Collections;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class FeyNewControl : MonoBehaviour
// {
//     [Header("Movement Settings")]
//     [SerializeField] private float gridSize = 1f;
//     [SerializeField] private LayerMask wallLayer;
//     [SerializeField] private LayerMask boxLayer;
//     [SerializeField] private float moveSpeed = 5f;
//     [SerializeField] private float boxMoveSpeed = 8f;
//     [SerializeField] private float snapThreshold = 0.01f;

//     [Header("Animation Settings")]
//     [SerializeField] private Animator animator;
//     private string lastDirection = "Down";

//     [Header("Win Settings")]
//     [SerializeField] public int minBoxesNumber = 1;
//     [SerializeField] private string nextLevel = "";

//     public static int currentBoxes;

//     private PlayerControls controls;
//     private Rigidbody2D rb;

//     // Movement state
//     private Vector2 movementInput;
//     private Vector3 targetPosition;
//     private bool isSmoothingMovement = false;
//     private bool isMoving = false;
//     private bool movementEnabled = true; 

//     private void Awake()
//     {
//         controls = new PlayerControls();
//         rb = GetComponent<Rigidbody2D>();
//         currentBoxes = 0;
        
//         if (animator == null)
//             animator = GetComponent<Animator>();
//     }

//     private void OnEnable()
//     {
//         controls.Enable();
  
//         PlayerOneMovement.OnDragonMovementStarted += DisableFairyMovement;
//         PlayerOneMovement.OnDragonMovementCompleted += EnableFairyMovement;
//     }

//     private void OnDisable()
//     {
//         controls.Disable();

//         PlayerOneMovement.OnDragonMovementStarted -= DisableFairyMovement;
//         PlayerOneMovement.OnDragonMovementCompleted -= EnableFairyMovement;
//     }

//     void Start()
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
//         if (!movementEnabled) return;

//         if (movementInput != Vector2.zero && !isSmoothingMovement)
//         {
//             Vector2 moveDirection = GetPrimaryDirection(movementInput);
//             if (moveDirection != Vector2.zero)
//             {
//                 TryMove(moveDirection);
//             }
//         }
        
//         HandleAnimations();
//     }

//     private void DisableFairyMovement()
//     {
//         movementEnabled = false;
//         movementInput = Vector2.zero; 
//         isMoving = false;
        
//         if (isSmoothingMovement)
//         {
//             StopAllCoroutines();
//             isSmoothingMovement = false;
//         }
//     }

//     private void EnableFairyMovement()
//     {
//         movementEnabled = true;
//     }

//     private void TryMove(Vector2 direction)
//     {
//         Vector2 newPos = (Vector2)transform.position + (direction * gridSize);

//         if (Physics2D.OverlapCircle(newPos, 0.2f, wallLayer))
//         {
//             isMoving = false;
//             return;
//         }

//         Collider2D box = Physics2D.OverlapCircle(newPos, 0.2f, boxLayer);
//         if (box != null)
//         {
//             Vector2 newBoxPos = newPos + (direction * gridSize);
//             if (Physics2D.OverlapCircle(newBoxPos, 0.2f, wallLayer | boxLayer))
//             {
//                 isMoving = false;
//                 return;
//             }
//             StartCoroutine(MoveBoxSmoothly(box.transform, newBoxPos));
//         }

//         targetPosition = newPos;
//         StartCoroutine(SmoothMovement());
//         UpdateLastDirection(direction);
//     }

//     private IEnumerator SmoothMovement()
//     {
//         isSmoothingMovement = true;
//         isMoving = true;
        
//         float remainingDistance = Vector3.Distance(transform.position, targetPosition);
        
//         while (remainingDistance > snapThreshold && movementEnabled)
//         {

//             if (!movementEnabled) 
//             {
//                 isSmoothingMovement = false;
//                 isMoving = false;
//                 yield break; 
//             }
            
//             transform.position = Vector3.MoveTowards(
//                 transform.position, 
//                 targetPosition, 
//                 moveSpeed * Time.deltaTime
//             );
            
//             remainingDistance = Vector3.Distance(transform.position, targetPosition);
//             yield return null;
//         }
        
//         if (movementEnabled)
//         {
//             transform.position = targetPosition;
//             isSmoothingMovement = false;
//             isMoving = false;

            

//             if (movementInput != Vector2.zero)
//             {
//                 // Vector2 moveDirection = GetPrimaryDirection(movementInput);
//                 if (movementInput != Vector2.zero)
//                 {
//                     TryMove(movementInput);
//                 }
//             }
//         }
//         else
//         {
//             isSmoothingMovement = false;
//             isMoving = false;
//         }
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

//     private void HandleAnimations()
//     {
//         if (animator == null) return;
//         string animationName = isMoving ? "Walking" : "Idle";
//         animator.Play(animationName + lastDirection);
//     }

//     private void UpdateLastDirection(Vector2 direction)
//     {
//         if (direction.x > 0) lastDirection = "Right";
//         else if (direction.x < 0) lastDirection = "Left";
//         else if (direction.y > 0) lastDirection = "Up";
//         else if (direction.y < 0) lastDirection = "Down";
//     }

//     private IEnumerator MoveBoxSmoothly(Transform box, Vector2 targetPosition)
//     {
//         Vector2 startPosition = box.position;
//         float journeyLength = Vector2.Distance(startPosition, targetPosition);
//         float startTime = Time.time;

//         while (Vector2.Distance(box.position, targetPosition) > snapThreshold && movementEnabled)
//         {
//             float distanceCovered = (Time.time - startTime) * boxMoveSpeed;
//             float fractionOfJourney = distanceCovered / journeyLength;
//             box.position = Vector2.Lerp(startPosition, targetPosition, fractionOfJourney);
//             yield return null;
//         }

//         if (movementEnabled)
//         {
//             box.position = targetPosition;
//         }
//     }
// }