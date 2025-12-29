
using System;
using UnityEngine;

public class TailDirection : MonoBehaviour
{
    [Header("Tail Sprites")]
    [SerializeField] private Sprite tailRight;
    [SerializeField] private Sprite tailLeft;
    [SerializeField] private Sprite tailUpLeft;
    [SerializeField] private Sprite tailUpRight;
    [SerializeField] private Sprite tailDownLeft;
    [SerializeField] private Sprite tailDownRight;

    [Header("References")]
    [SerializeField] private Transform previousSegment;
    [SerializeField] private SpriteRenderer tailRenderer;

    private string currentDirection = "Left";
    private string previousDirection = "Left";

    private void Update()
    {
        UpdateTailDirection();
    }

    private void UpdateTailDirection()
    {
        if (previousSegment == null) return;
        Vector3 direction = (previousSegment.position - transform.position).normalized;

        string newDirection;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            newDirection = direction.x > 0 ? "Left" : "Right";
        }
        else
        {
            newDirection = direction.y > 0 ? "Down" : "Up";
        }
        

        if (newDirection != currentDirection)
        {
            previousDirection = currentDirection;
            currentDirection = newDirection;
            UpdateTailSprite();
        }
    }

    private void UpdateTailSprite()
    {
        switch (currentDirection)
        {
            case "Right":
                tailRenderer.sprite = tailRight;
                break;
            case "Left":
                tailRenderer.sprite = tailLeft;
                break;
            case "Up":
                if (previousDirection == "Left")
                    tailRenderer.sprite = tailUpLeft;
                else
                    tailRenderer.sprite = tailUpRight;
                break;
            case "Down":
                if (previousDirection == "Left")
                    tailRenderer.sprite = tailDownLeft;
                else
                    tailRenderer.sprite = tailDownRight;
                break;
        }
    }
}