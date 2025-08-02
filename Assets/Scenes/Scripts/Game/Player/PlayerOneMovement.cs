using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerOneMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gridSize = 8f;
    [SerializeField] private float snapThreshold = 0.01f;

    private PlayerControls playerOneControls;
    private Rigidbody2D rb;
    private bool isMoving = false;
    private Vector2 targetPos;
    private Vector2 movementInput;


    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxLayer;



    private void Awake()
    {
        playerOneControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        playerOneControls.Enable();
        playerOneControls.Player.Player1.performed += OnMovementPerformed;
    }

    private void OnDisable()
    {
        playerOneControls.Disable();
        playerOneControls.Player.Player1.performed -= OnMovementPerformed;
    }

    private void Update()
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

    public void OnMovementPerformed(InputAction.CallbackContext context)
    {
        if (isMoving) return;
        
        movementInput = context.ReadValue<Vector2>();
        Vector2 moveDirection = GetPrimaryDirection(movementInput);

        if (moveDirection != Vector2.zero)
        {

            transform.position += (Vector3)moveDirection * gridSize;
            Debug.Log(transform.position);
            // TryToMove(moveDirection * gridSize);
        }
 
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



}