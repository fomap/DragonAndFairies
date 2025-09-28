using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TailDirection : MonoBehaviour
{
    [Header("Tail Sprites")]
    [SerializeField] private Sprite tailRight;
    [SerializeField] private Sprite tailLeft;
    [SerializeField] private Sprite tailUp;
    [SerializeField] private Sprite tailDown;

    [Header("References")]
    [SerializeField] private Transform previousSegment;
    [SerializeField] private SpriteRenderer tailRenderer;

    private Vector3 lastPosition;
    private string currentDirection = "Down";

    public Direction initialDir;
    public enum TailDir
    {
        Down,
        Up,
        Left,
        Right
    }


    private void Start()
    {
        lastPosition = transform.position;
        //   currentDirection = initialDir;
     
    }

    private void Update()
    {
        UpdateTailDirection();
    }

    private void UpdateTailDirection()
    {
        if (previousSegment == null) return;
        
        // Calculate direction based on position relative to previous segment
        Vector3 direction = (previousSegment.position - transform.position).normalized;
        
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            currentDirection = direction.x > 0 ? "Left" : "Right";
        }
        else
        {
            currentDirection = direction.y > 0 ? "Down" : "Up";
        }
        
        UpdateTailSprite();
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
                tailRenderer.flipX = true;
                break;
            case "Up":
                tailRenderer.sprite = tailLeft;
                tailRenderer.flipY = true;
                break;
            case "Down":
                tailRenderer.sprite = tailLeft;
                tailRenderer.flipY = false;
                break;
        }
    }
}