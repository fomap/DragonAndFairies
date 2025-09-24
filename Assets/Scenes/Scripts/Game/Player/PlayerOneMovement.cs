using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
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
    // private Rigidbody2D pointRB;
    private bool isMoving = false;
    private Vector2 targetPos;
    private Vector2 movementInput;
    private Transform currentParent;
    private int Transform;


    private void Awake()
    {
        playerOneControls = new PlayerControls();
        // pointRB = GetComponent<Rigidbody2D>();
    
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

    private void Update()
    {

        // if (isMoving)
        // {
        //     pointRB.position = Vector2.MoveTowards(
        //         pointRB.position,
        //         targetPos,
        //         moveSpeed * Time.deltaTime
        //     );

        //     if (Vector2.Distance(pointRB.position, targetPos) < snapThreshold)
        //     {
        //         pointRB.position = targetPos;
        //         isMoving = false;

        //         OnDragonMovementCompleted?.Invoke();
        //     }
        // }

    }

    public void OnMovementPerformed(InputAction.CallbackContext context)
    {
        if (isMoving) return;

        movementInput = context.ReadValue<Vector2>();
        // Vector2 moveDirection = GetPrimaryDirection(movementInput);

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



