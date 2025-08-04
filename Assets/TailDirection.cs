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

    private void Start()
    {
        lastPosition = transform.position;
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
                // tailRenderer.flipX = false;
                break;
            case "Left":
                tailRenderer.sprite = tailLeft; // Same sprite as right but flipped
                // tailRenderer.flipX = true;
                break;
            case "Up":
                tailRenderer.sprite = tailUp;
                break;
            case "Down":
                tailRenderer.sprite = tailDown;
                break;
        }
    }
}