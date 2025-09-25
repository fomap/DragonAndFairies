using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



[RequireComponent(typeof(PlayerInput))]
public class PlayerOneMovement : MonoBehaviour
{
    public static event Action OnDragonMovementStarted;
    public static event Action OnDragonMovementCompleted;

    public event Action<Vector2, bool> OnMovementAttempted;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 8f;
    [SerializeField] private float snapThreshold = 0.01f;
    [SerializeField] private LayerMask layerChecks;

    [Header("Body Chunks")]
    [SerializeField] private List<Transform> Chunks = new List<Transform>();
    [SerializeField] private List<Vector2> positionHistory = new List<Vector2>();

    private PlayerControls playerOneControls;
   
    private bool isMoving = false;
    private Vector2 targetPos;
    private Vector2 movementInput;
    private Transform currentParent;
    private int Transform;


    private void Awake()
    {
        playerOneControls = new PlayerControls();
       
    
        foreach (Transform child in transform)
        {
            Chunks.Add(child);
            positionHistory.Add(child.position);
        }

        Transform = Chunks.Count;

    }

    private void OnEnable()
    {
        playerOneControls.Enable();
        playerOneControls.Player.Dragon.performed += OnMovementPerformed;
    }

    private void OnDisable()
    {
        playerOneControls.Disable();
        playerOneControls.Player.Dragon.performed -= OnMovementPerformed;
    }

    public void OnMovementPerformed(InputAction.CallbackContext context)
    {
        if (isMoving) return;

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

        if (WouldIntersectSelf(newHeadPosition))
        {
            OnMovementAttempted?.Invoke(direction, false);
            OnDragonMovementCompleted?.Invoke();
            return false;
        }

        if (Physics2D.OverlapCircle(newHeadPosition, 0.45f, layerChecks))
        {
            OnMovementAttempted?.Invoke(direction, false);
            OnDragonMovementCompleted?.Invoke();
            return false;
        }

        transform.position = newHeadPosition;
        positionHistory.Insert(0, transform.position);
        
        int index = 0;
        foreach (var body in Chunks)
        {
            Vector2 point = positionHistory[Mathf.Min(index, positionHistory.Count - 1)];
            body.transform.position = point;
            index++;
        }

        OnMovementAttempted?.Invoke(direction, true);
        OnDragonMovementCompleted?.Invoke();

        return true;
    }

    private bool WouldIntersectSelf(Vector2 newHeadPosition)
    {
        for (int i = 1; i < Chunks.Count; i++)
        {
            if (Vector2.Distance(newHeadPosition, Chunks[i].position) < 0.4f * gridSize)
            {
                return true;
            }
        }
        return false;
    }
    
}



