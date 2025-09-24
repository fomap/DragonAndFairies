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
    [SerializeField] private float boxMoveSpeed = 8f;
    [SerializeField] private float snapThreshold = 0.01f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    private string lastDirection = "Down";

    [Header("Win Settings")]
    [SerializeField] public int minBoxesNumber = 1;
    [SerializeField] private string nextLevel = "";

    public static int currentBoxes;
    public static event Action CameraZoomOut;

    private PlayerControls playerControls;
    private Rigidbody2D rb;

    // Movement state
    private Vector2 movementInput;
    private Vector3 targetPos;
    private bool isSmoothingMovement = false;
    private bool isMoving = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        currentBoxes = 0;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Fey.canceled += ctx =>
        {
            movementInput = Vector2.zero;
            isMoving = false;
        };
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void FixedUpdate()
    {
        if (!isSmoothingMovement && movementInput != Vector2.zero)
        {
            TryMove(movementInput);
        }

        HandleAnimations();
        CheckWin();
    }

    private void CheckWin()
    {
        if (currentBoxes >= minBoxesNumber)
        {
            CameraZoomOut?.Invoke();
            SceneManager.LoadScene(nextLevel);
        }
    }

    private void HandleAnimations()
    {
        if (animator == null) return;
        string animationName = isMoving ? "Walking" : "Idle";
        animator.Play(animationName + lastDirection);
    }

    private void TryMove(Vector2 direction)
    {
        Vector2 newPos = GetGridAlignedPosition(rb.position + direction);

        // Wall check
        if (Physics2D.OverlapCircle(newPos, 0.45f, wallLayer)) return;

        // Box pushing check
        Collider2D box = Physics2D.OverlapCircle(newPos, 0.45f, boxLayer);
        if (box != null)
        {
            Vector2 newBoxPos = GetGridAlignedPosition(newPos + direction);
            if (Physics2D.OverlapCircle(newBoxPos, 0.45f, wallLayer | boxLayer)) return;

            StartCoroutine(MoveBoxSmoothly(box.transform, newBoxPos));
        }

        // Start smooth movement
        targetPos = newPos;
        StartCoroutine(SmoothMovement(targetPos));

        UpdateLastDirection(direction);
    }

    private IEnumerator SmoothMovement(Vector3 targetPosition)
    {
        isSmoothingMovement = true;
        isMoving = true;

        float remainingDistance = Vector3.Distance(transform.position, targetPosition);

        while (remainingDistance > snapThreshold)
        {
            rb.position = Vector3.MoveTowards(
                rb.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            remainingDistance = Vector3.Distance(rb.position, targetPosition);
            yield return null;
        }

        rb.position = targetPosition;
        isSmoothingMovement = false;
        isMoving = false;

        // Auto-continue if holding input
        if (movementInput != Vector2.zero)
        {
            TryMove(movementInput);
        }
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

    private Vector2 GetGridAlignedPosition(Vector2 position)
    {
        return new Vector2(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize
        );
    }

    private void UpdateLastDirection(Vector2 direction)
    {
        if (direction.x > 0) lastDirection = "Right";
        else if (direction.x < 0) lastDirection = "Left";
        else if (direction.y > 0) lastDirection = "Up";
        else if (direction.y < 0) lastDirection = "Down";
    }
}

