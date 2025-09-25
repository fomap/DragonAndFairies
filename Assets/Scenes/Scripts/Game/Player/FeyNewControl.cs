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

    [Header("Falling Settings")]
    [SerializeField] private float fallSpeed = 8f;
    private bool insideChunk = false;
    // [SerializeField] private float fallCheckInterval = 0.2f;
    // [SerializeField] private LayerMask chunkLayer; // Layer for chunks

    public static int currentBoxes;

    private PlayerControls controls;
    private Rigidbody2D characterRB;

    // Movement state
    private Vector2 movementInput;
    private Vector3 targetPosition;
    private bool isSmoothingMovement = false;
    private bool isMoving = false;
    private bool movementEnabled = true;
    private bool isFalling = false;

    
    

    [Header("SmoothDamp Settings")]
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float maxSpeed = 15f;

    [SerializeField] private float normalGravity = 1f;
    [SerializeField] private float noGravity = 0f;

    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        controls = new PlayerControls();
        characterRB = GetComponent<Rigidbody2D>();
        currentBoxes = 0;

        if (animator == null)
            animator = GetComponent<Animator>();

        SnapToGrid();
    }

    private void Start()
    {
        controls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Fey.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            isMoving = false;
        };

        // Start checking for falling
        // StartCoroutine(CheckForFalling());
    }

    private void Update()
    {
        if (!isSmoothingMovement && !isMoving && movementEnabled && !isFalling)
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

        //  if (!insideChunk && characterRB.gravityScale == 0)
        // {
        //     EnableGravity();
        // }

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

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            // Stop any current movement
            movementInput = Vector2.zero;
            isMoving = false;

            if (isSmoothingMovement)
            {
                StopAllCoroutines();
                isSmoothingMovement = false;
            }

            // Reset velocity
            if (characterRB != null)
            {
                characterRB.velocity = Vector2.zero;
            }
        }
    }

    void FixedUpdate()
    {
        if (!movementEnabled || isFalling) return;

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

    // private IEnumerator CheckForFalling()
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(fallCheckInterval);

    //         if (movementEnabled && !isFalling && !isSmoothingMovement && !isMoving)
    //         {
    //             CheckIfShouldFall();
    //         }
    //     }
    // }

    // private void CheckIfShouldFall()
    // {
        
    //     if (transform.parent != null && transform.parent.CompareTag("Chunk"))
    //     {
            
    //         return;
    //     }

        
    //     Vector2 checkPosition = GetGridAlignedPosition(transform.position + Vector3.down * gridSize);
    //     Collider2D chunkBelow = Physics2D.OverlapCircle(checkPosition, 0.2f, chunkLayer);

    //     if (chunkBelow != null)
    //     {
    
    //         StartCoroutine(FallToChunk(chunkBelow.transform));
    //     }
    //     else
    //     {
           
    //         CheckForContinuousFall();
    //     }
    // }

    // private void CheckForContinuousFall()
    // {
      
    //     if (transform.parent == null || !transform.parent.CompareTag("Chunk"))
    //     {
    //         StartCoroutine(ContinuousFall());
    //     }
    // }

    // private IEnumerator FallToChunk(Transform chunkTransform)
    // {
    //     isFalling = true;
    //     movementEnabled = false;

    //     Vector3 targetPosition = GetGridAlignedPosition(chunkTransform.position);
    //     Vector3 startPosition = transform.position;
    //     float journeyLength = Vector3.Distance(startPosition, targetPosition);
    //     float startTime = Time.time;

    //     while (Vector3.Distance(transform.position, targetPosition) > snapThreshold)
    //     {
    //         float distanceCovered = (Time.time - startTime) * fallSpeed;
    //         float fractionOfJourney = distanceCovered / journeyLength;
    //         transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
    //         yield return null;
    //     }

      
    //     transform.position = targetPosition;
    //     transform.SetParent(chunkTransform);

    //     isFalling = false;
    //     movementEnabled = true;

     
    //     HandleAnimations();
    // }

    // private IEnumerator ContinuousFall()
    // {
    //     isFalling = true;
    //     movementEnabled = false;

       
    //     if (animator != null)
    //     {
    //         animator.Play("Falling"); 
    //     }

    //     float fallDistance = 10f; 
    //     Vector3 fallTarget = transform.position + Vector3.down * fallDistance;
    //     Vector3 startPosition = transform.position;
    //     float startTime = Time.time;

    //     while (Vector3.Distance(transform.position, fallTarget) > snapThreshold && isFalling)
    //     {
           
    //         Vector2 checkPosition = GetGridAlignedPosition(transform.position + Vector3.down * gridSize);
    //         Collider2D chunkBelow = Physics2D.OverlapCircle(checkPosition, 0.2f, chunkLayer);

    //         if (chunkBelow != null)
    //         {
                
    //             transform.position = GetGridAlignedPosition(chunkBelow.transform.position);
    //             transform.SetParent(chunkBelow.transform);
    //             isFalling = false;
    //             movementEnabled = true;
    //             yield break;
    //         }

          
    //         float distanceCovered = (Time.time - startTime) * fallSpeed;
    //         float fractionOfJourney = distanceCovered / fallDistance;
    //         transform.position = Vector3.Lerp(startPosition, fallTarget, fractionOfJourney);
    //         yield return null;
    //     }

        
     
    //     Debug.LogWarning("Fairy fell out of the world!");

       
    //     isFalling = false;
    //     movementEnabled = true;
    // }

    private void DisableFairyMovement()
    {
        movementEnabled = false;
        movementInput = Vector2.zero;
        isMoving = false;

        if (isSmoothingMovement)
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
            SnapToGrid();
        }

        if (isFalling)
        {
            StopAllCoroutines();
            isFalling = false;
            SnapToGrid();
        }
    }

    private void EnableFairyMovement()
    {
        movementEnabled = true;
    }

    private void TryMove(Vector2 direction)
    {
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

            StartCoroutine(PushBoxAndMove(box.transform, newPos, newBoxPos));
            UpdateLastDirection(direction);
            return;
        }

        targetPosition = newPos;
        StartCoroutine(SmoothMovement());
        UpdateLastDirection(direction);
    }

    private IEnumerator PushBoxAndMove(Transform box, Vector2 feyTarget, Vector2 boxTarget)
    {
        isSmoothingMovement = true;
        isMoving = true;

        Coroutine boxRoutine = StartCoroutine(MoveBoxSmoothly(box, boxTarget));

        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, feyTarget);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, feyTarget) > snapThreshold && movementEnabled)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, feyTarget, fractionOfJourney);

            yield return null;
        }

        transform.position = feyTarget;
        SnapToGrid();

        yield return boxRoutine;

        isSmoothingMovement = false;
        isMoving = false;

        if (movementInput != Vector2.zero)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }
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
                SnapToGrid();
                yield break;
            }

            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            journeyLength = Vector3.Distance(transform.position, targetPosition);
            yield return null;
        }

        if (movementEnabled)
        {
            transform.position = targetPosition;
            SnapToGrid();

            isSmoothingMovement = false;
            isMoving = false;

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

        if (isFalling)
        {
            animator.Play("Falling");
        }
        else
        {
            string animationName = isMoving ? "Walking" : "Idle";
            animator.Play(animationName + lastDirection);
        }
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
            box.position = GetGridAlignedPosition(box.position);
        }
    }

    // private void CheckChunkParentingOLD()
    // {
    //     if (transform.parent == null)
    //     {
    //         if (ChunkManager.Instance != null)
    //         {
    //             ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
    //         }
    //     }
    //     else
    //     {
    //         if (ChunkManager.Instance != null)
    //         {
    //             ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
    //         }
    //     }
    // }


    private void CheckChunkParenting()
{
    bool inChunk = transform.parent != null && transform.parent.GetComponent<Chunk>() != null;

    if (inChunk && isFalling)
    {
        DisableGravity(); 
    }
    else if (!inChunk && !isFalling)
    {
        EnableGravity(); 
    }

   
    if (ChunkManager.Instance != null)
    {
        ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
    }
}

private void EnableGravity()
    {
        isFalling = true;
        movementEnabled = false;

        if (characterRB != null)
        {
            characterRB.gravityScale = normalGravity; 
            characterRB.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.Play("Falling");
        }
    }

private void DisableGravity()
{
    isFalling = false;
    movementEnabled = true;

    if (characterRB != null)
    {
        characterRB.gravityScale = noGravity; 
        characterRB.velocity = Vector2.zero;
    }
}


}




