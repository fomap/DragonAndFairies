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


    private Skyfall skyfallObject;
    private bool isFalling = false;

    public static int currentBoxes;
    private PlayerControls controls;
    private Rigidbody2D characterRB;

    // Movement state
    private Vector2 movementInput;
    private Vector3 targetPosition;
    private bool isSmoothingMovement = false;
    private bool isMoving = false;
    private bool movementEnabled = true;


    private void Awake()
    {
        controls = new PlayerControls();
        characterRB = GetComponent<Rigidbody2D>();
        currentBoxes = 0;

        if (animator == null)
            animator = GetComponent<Animator>();

        // Get or add ChunkAwareObject component
        skyfallObject = GetComponent<Skyfall>();
        if (skyfallObject == null)
        {
            skyfallObject = gameObject.AddComponent<Skyfall>();
            skyfallObject.SetObjectTag("Player");
        }

        // Listen to falling events
        skyfallObject.OnStartFalling += OnStartFalling;
        skyfallObject.OnStopFalling += OnStopFalling;
             skyfallObject.OnEnterAbyss += OnEnterAbyss;


        SnapToGrid();

    }

       private void OnStartFalling()
    {
        isFalling = true;
        movementEnabled = false;

        Debug.Log("Fey started falling - no chunk parent!");

        // Stop any active movement coroutines
        if (isSmoothingMovement)
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
        }
    }

    private void OnStopFalling()
    {
        isFalling = false;
        movementEnabled = true;
        Debug.Log("Fey stopped falling - found chunk parent!");
    }

  private void OnEnterAbyss()
    {
        Debug.Log("Fey fell into the abyss! Game over!");
        
        // Handle player death/restart logic
        // Example: reload scene, show game over screen, etc.
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
        // For now, just disable the controller
        movementEnabled = false;
        enabled = false;
        
        // Play death animation
        if (animator != null)
        {
            animator.Play("Falling");
        }
    }

    private void Start()
    {
        controls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Fey.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            isMoving = false;
        };
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

    private void Update()
    {
        // CheckChunkParentingStatus();
        // HandleFalling();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        if (!movementEnabled) return;

        if (!isFalling && movementInput != Vector2.zero && !isSmoothingMovement)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }
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

   
    //  private IEnumerator PushBoxAndMove(Transform box, Vector2 feyTarget, Vector2 boxTarget)
    // {
    //     isSmoothingMovement = true;
    //     isMoving = true;

    //     // Notify the box that it's being pushed
    //     Skyfall skyfallobj = box.GetComponent<Skyfall>();
    //     if (skyfallobj != null)
    //     {
    //         skyfallobj.SetMovementLocked(true);
    //     }

    //     Coroutine boxRoutine = StartCoroutine(MoveBoxSmoothly(box, boxTarget));

    //     Vector3 startPosition = transform.position;
    //     float journeyLength = Vector3.Distance(startPosition, feyTarget);
    //     float startTime = Time.time;

    //     while (Vector3.Distance(transform.position, feyTarget) > snapThreshold && movementEnabled)
    //     {
    //         float distanceCovered = (Time.time - startTime) * moveSpeed;
    //         float fractionOfJourney = distanceCovered / journeyLength;
    //         transform.position = Vector3.Lerp(startPosition, feyTarget, fractionOfJourney);

    //         yield return null;
    //     }

    //     transform.position = feyTarget;
    //     SnapToGrid();

    //     yield return boxRoutine;

    //     // Notify the box that pushing is complete
    //     if (skyfallobj != null)
    //     {
    //         skyfallobj.SetMovementLocked(false);
    //     }

    //     isSmoothingMovement = false;
    //     isMoving = false;

    //     if (movementInput != Vector2.zero && movementEnabled)
    //     {
    //         Vector2 moveDirection = GetPrimaryDirection(movementInput);
    //         if (moveDirection != Vector2.zero)
    //         {
    //             TryMove(moveDirection);
    //         }
    //     }
    // }

   private IEnumerator PushBoxAndMove(Transform box, Vector2 feyTarget, Vector2 boxTarget)
    {
        isSmoothingMovement = true;
        isMoving = true;

        // Notify the box that it's being pushed
        Skyfall boxChunkAware = box.GetComponent<Skyfall>();
        if (boxChunkAware != null)
        {
            boxChunkAware.SetMovementLocked(true);
        }

        Coroutine boxRoutine = StartCoroutine(MoveBoxSmoothly(box, boxTarget));

        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, feyTarget);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, feyTarget) > snapThreshold && movementEnabled)
        {
            // Check if box fell into abyss during pushing
            if (boxChunkAware != null && boxChunkAware.IsInAbyss())
            {
                Debug.Log("Box fell into abyss during push!");
                break;
            }

            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, feyTarget, fractionOfJourney);

            yield return null;
        }

        transform.position = feyTarget;
        SnapToGrid();

        yield return boxRoutine;

        // Only notify box if it's still alive
        if (boxChunkAware != null && !boxChunkAware.IsInAbyss())
        {
            boxChunkAware.SetMovementLocked(false);
        }

        isSmoothingMovement = false;
        isMoving = false;

        if (movementInput != Vector2.zero && movementEnabled)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }
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

            // Ensure box is properly parented after movement
            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ValidateObjectParenting(box, "Box");
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

            // Validate parenting after movement
            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
            }

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

    private void DisableFairyMovement()
    {
        movementEnabled = false;
        movementInput = Vector2.zero;
        isMoving = false;

        if (isSmoothingMovement && !isFalling) // Don't interrupt falling
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
            SnapToGrid();
        }
    }

    private void EnableFairyMovement()
    {
        // Only enable movement if we're not falling
        if (!isFalling)
        {
            movementEnabled = true;
        }
    }


    private void UpdateLastDirection(Vector2 direction)
    {
        if (direction.x > 0) lastDirection = "Right";
        else if (direction.x < 0) lastDirection = "Left";
        else if (direction.y > 0) lastDirection = "Up";
        else if (direction.y < 0) lastDirection = "Down";
    }

}





/*



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


    private Skyfall skyfallObject;
    private bool isFalling = false;

    public static int currentBoxes;
    private PlayerControls controls;
    private Rigidbody2D characterRB;

    // Movement state
    private Vector2 movementInput;
    private Vector3 targetPosition;
    private bool isSmoothingMovement = false;
    private bool isMoving = false;
    private bool movementEnabled = true;


    private void Awake()
    {
        controls = new PlayerControls();
        characterRB = GetComponent<Rigidbody2D>();
        currentBoxes = 0;

        if (animator == null)
            animator = GetComponent<Animator>();

        // Get or add ChunkAwareObject component
        skyfallObject = GetComponent<Skyfall>();
        if (skyfallObject == null)
        {
            skyfallObject = gameObject.AddComponent<Skyfall>();
            skyfallObject.SetObjectTag("Player");
        }

        // Listen to falling events
        skyfallObject.OnStartFalling += OnStartFalling;
        skyfallObject.OnStopFalling += OnStopFalling;

        SnapToGrid();

    }

       private void OnStartFalling()
    {
        isFalling = true;
        movementEnabled = false;

        Debug.Log("Fey started falling - no chunk parent!");

        // Stop any active movement coroutines
        if (isSmoothingMovement)
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
        }
    }

    private void OnStopFalling()
    {
        isFalling = false;
        movementEnabled = true;
        Debug.Log("Fey stopped falling - found chunk parent!");
    }



    private void Start()
    {
        controls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Fey.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            isMoving = false;
        };
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

    private void Update()
    {
        // CheckChunkParentingStatus();
        // HandleFalling();
        HandleAnimations();
    }

    void FixedUpdate()
    {
        if (!movementEnabled) return;

        if (!isFalling && movementInput != Vector2.zero && !isSmoothingMovement)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }
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

        // Notify the box that it's being pushed
        Skyfall skyfallobj = box.GetComponent<Skyfall>();
        if (skyfallobj != null)
        {
            skyfallobj.SetMovementLocked(true);
        }

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

        // Notify the box that pushing is complete
        if (skyfallobj != null)
        {
            skyfallobj.SetMovementLocked(false);
        }

        isSmoothingMovement = false;
        isMoving = false;

        if (movementInput != Vector2.zero && movementEnabled)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }
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

            // Ensure box is properly parented after movement
            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ValidateObjectParenting(box, "Box");
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

            // Validate parenting after movement
            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
            }

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

    private void DisableFairyMovement()
    {
        movementEnabled = false;
        movementInput = Vector2.zero;
        isMoving = false;

        if (isSmoothingMovement && !isFalling) // Don't interrupt falling
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
            SnapToGrid();
        }
    }

    private void EnableFairyMovement()
    {
        // Only enable movement if we're not falling
        if (!isFalling)
        {
            movementEnabled = true;
        }
    }


    private void UpdateLastDirection(Vector2 direction)
    {
        if (direction.x > 0) lastDirection = "Right";
        else if (direction.x < 0) lastDirection = "Left";
        else if (direction.y > 0) lastDirection = "Up";
        else if (direction.y < 0) lastDirection = "Down";
    }

}




















*/