using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class ChangeSprite : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite upSprite;
    [SerializeField] private Sprite downSprite;

  

    private PlayerControls playerControls;
    private Vector2 movementInput;
    private string lastDirection = "Down";
    private SpriteRenderer spriteRenderer;
   [SerializeField] private PlayerOneMovement snakeMovement; 

    private void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

        

 
    private void OnEnable()
    {
        snakeMovement.OnMovementAttempted += HandleMovementAttempt;
    }

    private void OnDisable()
    {
        snakeMovement.OnMovementAttempted -= HandleMovementAttempt;
    }

       private void HandleMovementAttempt(Vector2 direction, bool success)
    {
        if (success)
        {
            UpdateLastDirection(direction);
            ChangeSnakeSprite(lastDirection);
        }
       
    }

    private void OnMovementPerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        Vector2 moveDirection = GetPrimaryDirection(movementInput);

        if (moveDirection != Vector2.zero)
        {
            UpdateLastDirection(moveDirection);
            ChangeSnakeSprite(lastDirection);
        }
    }

    private void ChangeSnakeSprite(string direction)
    {
        switch (direction)
        {
            case "Right":
                spriteRenderer.sprite = rightSprite;
                spriteRenderer.flipX = false; // Reset flip if needed
                break;
            case "Left":
                spriteRenderer.sprite = rightSprite; // Use same sprite as right
                spriteRenderer.flipX = true; // Flip horizontally
                break;
            case "Up":
                spriteRenderer.sprite = upSprite;
                break;
            case "Down":
                spriteRenderer.sprite = downSprite;
                break;
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
    
    private void UpdateLastDirection(Vector2 direction)
    {
        if (direction.x > 0) lastDirection = "Right";
        else if (direction.x < 0) lastDirection = "Left";
        else if (direction.y > 0) lastDirection = "Up";
        else if (direction.y < 0) lastDirection = "Down";
    }
}