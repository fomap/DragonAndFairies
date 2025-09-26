using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerOneMovement : MonoBehaviour
{
    public static event Action OnDragonMovementStarted;
    public static event Action OnDragonMovementCompleted;
    public event Action<Vector2, bool> OnMovementAttempted;

    [Header("Movement Settings")]
    [SerializeField] private float gridSize = 8f;
    [SerializeField] private LayerMask layerChecks;

    [Header("Body Chunks")]
    [SerializeField] private List<Transform> chunks = new List<Transform>();
    [SerializeField] private List<Vector2> positionHistory = new List<Vector2>();

    private PlayerControls playerControls;
    private bool isMoving = false;
    private bool dragonMovementEnabled = true;
    private Vector2 movementInput;
    private int chunkCount;
 

    private void Awake()
    {
        playerControls = new PlayerControls();
        InitializeChunks();
    }

    private void InitializeChunks()
    {
        foreach (Transform child in transform)
        {
            chunks.Add(child);
            positionHistory.Add(child.position);
        }
        chunkCount = chunks.Count;
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Dragon.performed += OnMovementPerformed;
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.Dragon.performed -= OnMovementPerformed;
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GlobalSkyfallEventManager.OnAnyObjectStartFalling += DisableDragonMovement;
        GlobalSkyfallEventManager.OnAnyObjectStopFalling += EnableDragonMovement;


        GlobalSkyfallEventManager.OnFeyStartMoving += DisableDragonMovement;
        GlobalSkyfallEventManager.OnFeyStopMoving += EnableDragonMovement;
    }

    private void UnsubscribeFromEvents()
    {
        GlobalSkyfallEventManager.OnAnyObjectStartFalling -= DisableDragonMovement;
        GlobalSkyfallEventManager.OnAnyObjectStopFalling -= EnableDragonMovement;

        GlobalSkyfallEventManager.OnFeyStartMoving -= DisableDragonMovement;
        GlobalSkyfallEventManager.OnFeyStopMoving -= EnableDragonMovement;
    }

    public void OnMovementPerformed(InputAction.CallbackContext context)
    {
        if (isMoving || !dragonMovementEnabled) return;

        movementInput = context.ReadValue<Vector2>();
        if (movementInput != Vector2.zero)
        {
            OnDragonMovementStarted?.Invoke();
            TryToMove(movementInput * gridSize);
        }
    }

    private bool TryToMove(Vector2 direction)
    {
        Vector2 newHeadPosition = (Vector2)transform.position + direction;

        if (WouldIntersectSelf(newHeadPosition) || IsCollisionAtPosition(newHeadPosition))
        {
            OnMovementAttempted?.Invoke(direction, false);
            OnDragonMovementCompleted?.Invoke();
            return false;
        }

        ExecuteMovement(newHeadPosition, direction);
        return true;
    }

    private bool IsCollisionAtPosition(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, 0.45f, layerChecks);
    }

    private bool WouldIntersectSelf(Vector2 newHeadPosition)
    {
        for (int i = 1; i < chunks.Count; i++)
        {
            if (Vector2.Distance(newHeadPosition, chunks[i].position) < 0.4f * gridSize)
            {
                return true;
            }
        }
        return false;
    }

    private void ExecuteMovement(Vector2 newHeadPosition, Vector2 direction)
    {
        transform.position = newHeadPosition;
        positionHistory.Insert(0, transform.position);

        UpdateBodyPositions();
        OnMovementAttempted?.Invoke(direction, true);
        OnDragonMovementCompleted?.Invoke();
    }

    private void UpdateBodyPositions()
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            Vector2 point = positionHistory[Mathf.Min(i, positionHistory.Count - 1)];
            chunks[i].position = point;
        }
    }


    private void DisableDragonMovement()
    {
        dragonMovementEnabled = false;
        movementInput = Vector2.zero;
        isMoving = false;

    }

    private void EnableDragonMovement ()
    {
        
        dragonMovementEnabled = true;
        
    }

}


