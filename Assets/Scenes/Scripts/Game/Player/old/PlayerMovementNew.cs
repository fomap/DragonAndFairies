using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementOld : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movementDirection;
    private Vector2 currentInput;



    [Header("Animations")]
    [SerializeField] private Animator animator;
    [SerializeField] private string lastDirection = "Down";


    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // private void Update()
    // {
    //     HandleAnimations();
    // }

    private void FixedUpdate()
    {
        HandleAnimations();
        rb.velocity = movementDirection * moveSpeed;
    }

    private void HandleAnimations()
    {
        if (animator == null) return;

        string animationName = "";

        if (movementDirection == Vector2.zero)
            animationName = "Idle";
        else
            animationName = "Walking";

        animator.Play(animationName + lastDirection);
    }


    private Vector3 GetDirection(Vector3 input)
    {
        Vector3 finalDirection = Vector3.zero;
        if (input.y > 0.01f)
        {
            lastDirection = "Up";
            finalDirection = new Vector2(0, 1);
        }
        else if (input.y < -0.01f)
        {
            lastDirection = "Down";
            finalDirection = new Vector2(0, -1);
        }
        else if (input.x > 0.01f)
        {
            lastDirection = "Right";
            finalDirection = new Vector2(1, 0);
        }
        else if (input.x < -0.01f)
        {
            lastDirection = "Left";
            finalDirection = new Vector2(-1, 0);
        }
        else
            finalDirection = Vector2.zero;

        return finalDirection;
    }
    private void OnMovement(InputValue value)
    {
        currentInput = value.Get<Vector2>().normalized;
        movementDirection = GetDirection(currentInput);
    }

   

}



