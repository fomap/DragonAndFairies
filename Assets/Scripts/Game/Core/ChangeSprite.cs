
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeSprite : MonoBehaviour
{
    [Header("Sprite Settings")]
    [SerializeField] private Sprite rightSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite upLeftSprite;
    [SerializeField] private Sprite upRightSprite;
    [SerializeField] private Sprite downLeftSprite;
    [SerializeField] private Sprite downRightSprite;

    private PlayerControls playerControls;
    private Vector2 movementInput;
    private string currentDirection = "Left";
    private string previousMoveDirection = "Left"; 
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
            string previousDirection = currentDirection;
            UpdateCurrentDirection(direction);
            ChangeSnakeSprite(currentDirection, previousDirection);
            previousMoveDirection = currentDirection;
        }
    }

    private void UpdateCurrentDirection(Vector2 direction)
    {
        if (direction.x > 0) 
            currentDirection = "Right";
        else if (direction.x < 0) 
            currentDirection = "Left";
        else if (direction.y > 0) 
            currentDirection = "Up";
        else if (direction.y < 0) 
            currentDirection = "Down";
    }

    private void ChangeSnakeSprite(string currentDirection, string previousDirection)
    {
        switch (currentDirection)
        {
            case "Right":
                spriteRenderer.sprite = rightSprite;
                break;
            case "Left":
                spriteRenderer.sprite = leftSprite;
                break;
            case "Up":
                if (previousDirection == "Left")
                    spriteRenderer.sprite = upLeftSprite;
                else if (previousDirection == "Right")
                    spriteRenderer.sprite = upRightSprite;
                break;
            case "Down":
                if (previousDirection == "Left")
                    spriteRenderer.sprite = downLeftSprite;
                else if (previousDirection == "Right")
                    spriteRenderer.sprite = downRightSprite;
                break;
        }
        
       //Debug.Log($"Current: {currentDirection}, Previous: {previousDirection}, Sprite: {spriteRenderer.sprite.name}");
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