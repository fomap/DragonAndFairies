using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls playerControls = null;
    private Vector2 moveVector = Vector2.zero;
    private Rigidbody2D playerRB;


    [Header("MovementSettings")]
    [SerializeField] private float gridSize = 1f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private float moveSpeed = 5f;


    private bool isMoving = false;
    private Vector2 targetPos;
    private Vector2 lastMovedDir;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerRB = GetComponent<Rigidbody2D>();

    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Player.Movement.performed += OnMovementPerformed;
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerControls.Player.Movement.performed -= OnMovementPerformed;
    }

    private void Update()
    {
        if (isMoving)
        {
            playerRB.position = Vector2.MoveTowards(
                playerRB.position,
                targetPos,
                moveSpeed /* *Time.deltaTime*/
            );


            if (Vector2.Distance(playerRB.position, targetPos) < 0.01f)
            {
                playerRB.position = targetPos;
                isMoving = false;
            }
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        if (isMoving) return;
        moveVector = value.ReadValue<Vector2>();

        Vector2 moveDirection = Vector2.zero;
        if (Mathf.Abs(moveVector.x) > 0.5f)
        {
            moveDirection.x = Mathf.Sign(moveVector.x);
        }
        else
        {
            moveDirection.y = Mathf.Sign(moveVector.y);
        }

        TryToMove(moveDirection * gridSize);

    }

    private void TryToMove(Vector2 direction)
    {
        Vector2 newPos = playerRB.position + direction;
        if (Physics2D.OverlapCircle(newPos, 0.01f, wallLayer))
        {
            return; 
        }

        Collider2D box = Physics2D.OverlapCircle(newPos, 0.01f, boxLayer);
        if (box != null)
        {
            Vector2 newBoxPos = newPos + direction;
            if (!Physics2D.OverlapCircle(newBoxPos, 0.01f, wallLayer | boxLayer))
            {
                box.transform.position = newBoxPos;
            }
            else
            {
                return;
            }


        }

        targetPos = newPos;
        isMoving = true;
        lastMovedDir = direction;

    }
 
    
}
