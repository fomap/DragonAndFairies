using UnityEngine;

public class BoxPhysics : MonoBehaviour
{
    private void Awake()
    {
        // Ensure box has a Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
        }
        
        // Register with gravity controller
        if (GravityController.Instance != null)
        {
            GravityController.Instance.RegisterObject(transform);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up from gravity controller if needed
    }
}