using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class FairyMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float snapThreshold = 0.01f;
    
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float boxMoveSpeed = 8f;
    
    private PlayerControls playerControls;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Vector2 targetPos;
    private string lastDirection = "Down";
    private Vector2 movementInput;
    
    
    [Header("Win Settings")]
    [SerializeField] public int minBoxesNumber = 1;
    [SerializeField] public static int currentBoxes;
    [SerializeField] private string nextLevel = "";


    public static event Action CameraZoomOut;

    public void DoSomethingAndNotify()
    {
        
        CameraZoomOut?.Invoke(); 
    }

    private void Awake()

    {
       
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        currentBoxes = 0;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Player2.performed += OnMovementPerformed;
        
    }
    
    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.Player2.performed -= OnMovementPerformed;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleAnimations();
        CheckWin();
    }

    private void CheckWin()
    {
        if (currentBoxes >= minBoxesNumber)
        {

            DoSomethingAndNotify();
            SceneManager.LoadScene(nextLevel);
            
        }
    }

    private void HandleAnimations()
    {
        if (animator == null) return;
        string animationName = isMoving ? "Walking" : "Idle";
        animator.Play(animationName + lastDirection);
    }
    
    private void HandleMovement()
    {
        if (isMoving)
        {
            rb.position = Vector2.MoveTowards(
                rb.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(rb.position, targetPos) < snapThreshold)
            {
                rb.position = targetPos;
                isMoving = false;
            }
        }
    }
    
    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        if (isMoving) return;
        
        movementInput = context.ReadValue<Vector2>();
        Vector2 moveDirection = GetPrimaryDirection(movementInput);
        
        if (moveDirection != Vector2.zero)
        {
            UpdateLastDirection(moveDirection);
            TryToMove(moveDirection * gridSize);
        }
    }
    
    private void TryToMove(Vector2 direction)
    {
        Vector2 newPos = GetGridAlignedPosition(rb.position + direction);
        if (Physics2D.OverlapCircle(newPos, 0.45f, wallLayer)) return;

        Collider2D box = Physics2D.OverlapCircle(newPos, 0.45f, boxLayer);
        if (box != null)
        {
            Vector2 newBoxPos = GetGridAlignedPosition(newPos + direction);
            if (Physics2D.OverlapCircle(newBoxPos, 0.45f, wallLayer | boxLayer)) return;

            StartCoroutine(MoveBoxSmoothly(box.transform, newBoxPos));
        }

        targetPos = newPos;
        isMoving = true;
    }
    
   
    private Vector2 GetGridAlignedPosition(Vector2 position)
    {
        return new Vector2(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize
        );
    }
    
    private IEnumerator MoveBoxSmoothly(Transform box, Vector2 targetPosition)
    {
        Vector2 startPosition = box.position;
        float journeyLength = Vector2.Distance(startPosition, targetPosition);
        float startTime = Time.time;
        
        while (Vector2.Distance(box.position, targetPosition) > snapThreshold)
        {
            float distanceCovered = (Time.time - startTime) * boxMoveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            box.position = Vector2.Lerp(startPosition, targetPosition, fractionOfJourney);
            yield return null;
        }
        
        box.position = targetPosition;
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
    
    private void UpdateLastDirection(Vector2 direction)
    {
        if (direction.x > 0) lastDirection = "Right";
        else if (direction.x < 0) lastDirection = "Left";
        else if (direction.y > 0) lastDirection = "Up";
        else if (direction.y < 0) lastDirection = "Down";
    }
    
    
}