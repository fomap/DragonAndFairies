using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FeyNewControl : MonoBehaviour
{
    [Header("Movement Settings")]
    private float gridSize = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float boxMoveSpeed = 2f;
    private float snapThreshold = 0.01f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    private string lastDirection = "Down";

    [Header("Win Settings")]


    [SerializeField] public int minBoxesNumber = 1;
    // [SerializeField] private string nextLevel = "";
   
    private AsyncOperation asyncLoadOperation;

    // Components
    private Skyfall skyfallObject;
    private PlayerControls controls;
    private Rigidbody2D characterRB;

    // Movement state
    private Vector2 movementInput;
    private Vector3 targetPosition;
    private bool isSmoothingMovement = false;
    private bool isMoving = false;
    private bool isPushing = false;
    private bool movementEnabled = true;
    public static int currentBoxes;

    #region Initialization
    private void Awake()
    {
        controls = new PlayerControls();
        characterRB = GetComponent<Rigidbody2D>();
        currentBoxes = 0;

        InitializeComponents();
        SetupSkyfallEvents();
        SnapToGrid();

    }

    private void InitializeComponents()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

       
        skyfallObject = GetComponent<Skyfall>();
        if (skyfallObject == null)
        {
            skyfallObject = gameObject.AddComponent<Skyfall>();
            skyfallObject.SetObjectTag("Player");
        }
    }

    private void SetupSkyfallEvents()
    {
        skyfallObject.OnStartFalling += OnStartFalling;
        skyfallObject.OnStopFalling += OnStopFalling;
        skyfallObject.OnEnterAbyss += OnEnterAbyss;
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
    #endregion

    #region Event Subscription
    private void OnEnable()
    {
        controls.Enable();
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        controls.Disable();
        UnsubscribeFromEvents();

        if (skyfallObject != null)
        {
            skyfallObject.OnStartFalling -= OnStartFalling;
            skyfallObject.OnStopFalling -= OnStopFalling;
            skyfallObject.OnEnterAbyss -= OnEnterAbyss;
        }
    }

    private void SubscribeToEvents()
    {
        PlayerOneMovement.OnDragonMovementStarted += DisableFairyMovement;
        PlayerOneMovement.OnDragonMovementCompleted += EnableFairyMovement;

        GlobalSkyfallEventManager.OnAnyObjectStartFalling += DisableFairyMovement;
        GlobalSkyfallEventManager.OnAnyObjectStopFalling += EnableFairyMovement;

         GlobalSkyfallEventManager.OnGamePaused += DisableFairyMovement;
        GlobalSkyfallEventManager.OnGameResumed += EnableFairyMovement;  
    }

    private void UnsubscribeFromEvents()
    {
        PlayerOneMovement.OnDragonMovementStarted -= DisableFairyMovement;
        PlayerOneMovement.OnDragonMovementCompleted -= EnableFairyMovement;

        GlobalSkyfallEventManager.OnAnyObjectStartFalling -= DisableFairyMovement;
        GlobalSkyfallEventManager.OnAnyObjectStopFalling -= EnableFairyMovement;

        GlobalSkyfallEventManager.OnGamePaused -= DisableFairyMovement;
        GlobalSkyfallEventManager.OnGameResumed -= EnableFairyMovement;    
    }
    #endregion

    #region Skyfall Event Handlers
    private void OnStartFalling()
    {
        movementEnabled = false;

        if (isSmoothingMovement)
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
        }

        if (animator != null)
            animator.Play("Falling");
    }

    private void OnStopFalling()
    {
        movementEnabled = true;
        SnapToGrid();
    }

    private void OnEnterAbyss()
    {
        movementEnabled = false;
        enabled = false;
        
        if (animator != null)
        {
            animator.Play("Falling");
        }
    }
    #endregion

    #region Movement Core
    private void FixedUpdate()
    {

        if (!movementEnabled || skyfallObject.IsFalling()) return;

        if (!skyfallObject.IsFalling() && movementInput != Vector2.zero && !isSmoothingMovement)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }

        HandleAnimations();
        CheckWin();
    }

    private void CheckWin()
    {
        if (currentBoxes == minBoxesNumber)
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            if (Dialogue.Instance != null && Dialogue.Instance.IsDialogueActive)
            {
                Dialogue.Instance.QueueLevelLoad(); // wait for dialogue to finish
            }
            else
            {
                // load immediately if no dialogue is showing
                SceneManager.LoadScene(sceneIndex + 1);
            }


        

        }
    }





    private void TryMove(Vector2 direction)
    {
        Vector2 newPos = GetGridAlignedPosition((Vector2)transform.position + (direction * gridSize));

        if (IsWallCollision(newPos))
        {
            isMoving = false;
            return;
        }

        Collider2D box = GetBoxAtPosition(newPos);
        if (box != null)
        {
            TryPushBox(direction, newPos, box);
            return;
        }

        // Regular movement without box
        targetPosition = newPos;
        StartCoroutine(SmoothMovement());
        UpdateLastDirection(direction);
    }

    private bool IsWallCollision(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, 0.2f, wallLayer);
    }

    private Collider2D GetBoxAtPosition(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, 0.2f, boxLayer);
    }

    private void TryPushBox(Vector2 direction, Vector2 feyPosition, Collider2D box)
    {
        Vector2 newBoxPos = GetGridAlignedPosition(feyPosition + (direction * gridSize));
        
        if (IsWallOrBoxCollision(newBoxPos))
        {
            isMoving = false;
            return;
        }

        StartCoroutine(PushBoxAndMove(box.transform, feyPosition, newBoxPos));
        UpdateLastDirection(direction);
    }

    private bool IsWallOrBoxCollision(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, 0.2f, wallLayer | boxLayer);
    }
    #endregion

    #region Movement Coroutines

    private IEnumerator PushBoxAndMove(Transform box, Vector2 feyTarget, Vector2 boxTarget)
    {
        isSmoothingMovement = true;
        isMoving = true;
        isPushing = true;
        StartMovement();

        Skyfall boxSkyfall = box.GetComponent<Skyfall>();
        if (boxSkyfall != null)
        {
            boxSkyfall.SetMovementLocked(true);
        }

        Coroutine boxRoutine = StartCoroutine(MoveBoxSmoothly(box, boxTarget));

        Vector3 startPosition = transform.position;
        float journeyLength = Vector3.Distance(startPosition, feyTarget);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, feyTarget) > snapThreshold && movementEnabled)
        {
            // Check if Fey started falling during pushing
            if (skyfallObject.IsFalling())
            {
                Debug.Log("Fey started falling while pushing box - aborting push");
                break;
            }

            // Check if box fell into abyss during pushing
            if (boxSkyfall != null && boxSkyfall.IsInAbyss())
            {
                break;
            }

            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, feyTarget, fractionOfJourney);
            yield return null;
        }

        if (movementEnabled && !skyfallObject.IsFalling())
        {
            transform.position = feyTarget;
            SnapToGrid();
        }

        if (skyfallObject.IsFalling())
        {
            StopCoroutine(boxRoutine);
            if (boxSkyfall != null)
            {
                boxSkyfall.SetMovementLocked(false);
            }
        }
        else
        {
            yield return boxRoutine;
        }


        yield return new WaitForEndOfFrame();

     
        if (ChunkManager.Instance != null)
        {
            ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
            ChunkManager.Instance.ValidateObjectParenting(box, "Box");
        }

        // Only notify box if it's still alive
        if (boxSkyfall != null && !boxSkyfall.IsInAbyss() && !boxSkyfall.IsFalling())
        {
            boxSkyfall.SetMovementLocked(false);
        }

        isSmoothingMovement = false;
        isMoving = false;
        isPushing = false; 
        StopMovement();

        GlobalSkyfallEventManager.Instance?.IncrementBoxMoveCount();

        // Continue movement if input is still active
        if (movementEnabled && !skyfallObject.IsFalling() && movementInput != Vector2.zero)
        // if (movementEnabled && movementInput != Vector2.zero)
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
            // box.position = alignedTarget;
            box.position = GetGridAlignedPosition(box.position);

            Chunk correctChunk = ChunkManager.Instance.FindChunkContainingPosition(box.position);
            // Ensure box is properly parented after movement
            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ForceParentObject(box, correctChunk);
            }
        }
    }

    private IEnumerator SmoothMovement()
    {
        isSmoothingMovement = true;
        isMoving = true;
        StartMovement();

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
                StopMovement();
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

            yield return new WaitForEndOfFrame();

            // Validate parenting after movement
            if (ChunkManager.Instance != null)
            {
                ChunkManager.Instance.ValidateObjectParenting(transform, "Player");
            }
        }

        isSmoothingMovement = false;
        isMoving = false;
        StopMovement();

        // Continue movement if input is still active
        if (movementEnabled && movementInput != Vector2.zero)
        {
            Vector2 moveDirection = GetPrimaryDirection(movementInput);
            if (moveDirection != Vector2.zero)
            {
                TryMove(moveDirection);
            }
        }
    }
    #endregion

    #region Animation
    private void HandleAnimations()
    {
        if (skyfallObject.IsFalling() || skyfallObject.IsInAbyss())
    {
        animator.Play("Falling");
    }
    else if (isPushing)
    {
        animator.Play("Pushing" + lastDirection);
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
    #endregion

    #region Utility Methods
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
    #endregion

    #region Movement Control
    private void StartMovement()
    {
        GlobalSkyfallEventManager.Instance?.NotifyFeyStartMoving();
    }

    private void StopMovement()
    {
        GlobalSkyfallEventManager.Instance?.NotifyFeyStopMoving();
    }

    private void DisableFairyMovement()
    {
        movementEnabled = false;
        movementInput = Vector2.zero;
        isMoving = false;

        if (isSmoothingMovement && !skyfallObject.IsFalling())
        {
            StopAllCoroutines();
            isSmoothingMovement = false;
            SnapToGrid();
        }
    }

    private void EnableFairyMovement()
    {
        if (!skyfallObject.IsFalling() && !GlobalSkyfallEventManager.IsGamePaused)
        {
            movementEnabled = true;
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;

        if (!enabled)
        {
            movementInput = Vector2.zero;
            isMoving = false;

            if (isSmoothingMovement)
            {
                StopAllCoroutines();
                isSmoothingMovement = false;
            }

            if (characterRB != null)
            {
                characterRB.velocity = Vector2.zero;
            }
        }
    }
    #endregion

    #region Public API
    public bool IsMovingBox(Transform box)
    {
        return isSmoothingMovement && box != null && 
               Vector3.Distance(box.position, GetGridAlignedPosition(box.position)) > snapThreshold;
    }

    public bool IsFalling()
    {
        return skyfallObject != null && skyfallObject.IsFalling();
    }

    public bool IsInAbyss()
    {
        return skyfallObject != null && skyfallObject.IsInAbyss();
    }

    public bool IsMoving()
    {
        return isMoving || isSmoothingMovement;
    }
    #endregion

    #region Properties
    // Public properties for external access
    public string CurrentDirection => lastDirection;
    public Vector2 MovementInput => movementInput;
    public bool MovementEnabled => movementEnabled;
    #endregion
}





