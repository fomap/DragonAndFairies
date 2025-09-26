using UnityEngine;
using System.Collections.Generic;

public class GravityController : MonoBehaviour
{
    // public static GravityController Instance;
    
    // [Header("Gravity Settings")]
    // [SerializeField] private float gravityStrength = 15f;
    // [SerializeField] private float maxFallSpeed = 20f;
    // [SerializeField] private LayerMask groundLayerMask;
    // [SerializeField] private float fallActivationDelay = 0.1f;
    
    // private HashSet<Transform> fallingObjects = new HashSet<Transform>();
    // private Dictionary<Transform, Rigidbody2D> objectRigidbodies = new Dictionary<Transform, Rigidbody2D>();
    // private Dictionary<Transform, Vector2> fallVelocities = new Dictionary<Transform, Vector2>();
    
    // public System.Action<Transform> OnObjectStartedFalling;
    // public System.Action<Transform> OnObjectStoppedFalling;
    
    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }
    
    // private void Update()
    // {
    //     // Check all tracked objects for chunk parenting status
    //     CheckObjectsChunkStatus();
        
    //     // Apply gravity to falling objects
    //     ApplyGravity();
        
    //     // Check for ground collisions
    //     CheckGroundCollisions();
    // }
    
    // public void RegisterObject(Transform obj)
    // {
    //     if (obj == null) return;
        
    //     Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
    //     if (rb == null) return;
            
    //     objectRigidbodies[obj] = rb;
    //     // Store initial state
    //     rb.gravityScale = 0f;
    //     rb.velocity = Vector2.zero;
    // }
    
    // private void CheckObjectsChunkStatus()
    // {
    //     List<Transform> objectsToCheck = new List<Transform>(objectRigidbodies.Keys);
        
    //     foreach (Transform obj in objectsToCheck)
    //     {
    //         if (obj == null) continue;
            
    //         bool isInChunk = Chunk.IsObjectInAnyChunk(obj);
    //         bool isFalling = fallingObjects.Contains(obj);
            
    //         if (!isInChunk && !isFalling)
    //         {
    //             // Start falling after a delay to avoid false triggers
    //             StartCoroutine(StartFallingAfterDelay(obj));
    //         }
    //         else if (isInChunk && isFalling)
    //         {
    //             StopFalling(obj);
    //         }
    //     }
    // }
    
    // private System.Collections.IEnumerator StartFallingAfterDelay(Transform obj)
    // {
    //     yield return new WaitForSeconds(fallActivationDelay);
        
    //     // Double-check if still not in a chunk
    //     if (obj != null && !Chunk.IsObjectInAnyChunk(obj) && objectRigidbodies.ContainsKey(obj))
    //     {
    //         StartFalling(obj);
    //     }
    // }
    
    // private void StartFalling(Transform obj)
    // {
    //     if (obj == null || fallingObjects.Contains(obj)) return;
        
    //     fallingObjects.Add(obj);
    //     fallVelocities[obj] = Vector2.zero;
        
    //     Rigidbody2D rb = objectRigidbodies[obj];
    //     if (rb != null)
    //     {
    //         rb.gravityScale = 1f; // Enable Unity's physics gravity
    //     }
        
    //     // Disable controls if it's Fey
    //     if (obj.CompareTag("Player"))
    //     {
    //         DisableCharacterControls();
    //     }
        
    //     OnObjectStartedFalling?.Invoke(obj);
    //     Debug.Log($"{obj.name} started falling!");
    // }
    
    // private void StopFalling(Transform obj)
    // {
    //     if (obj == null || !fallingObjects.Contains(obj)) return;
        
    //     fallingObjects.Remove(obj);
    //     fallVelocities.Remove(obj);
        
    //     Rigidbody2D rb = objectRigidbodies[obj];
    //     if (rb != null)
    //     {
    //         rb.gravityScale = 0f;
    //         rb.velocity = Vector2.zero;
    //     }
        
    //     // Re-enable controls if it's Fey and reached safe ground
    //     if (obj.CompareTag("Player") && Chunk.IsObjectInAnyChunk(obj))
    //     {
    //         EnableCharacterControls();
    //     }
        
    //     OnObjectStoppedFalling?.Invoke(obj);
    //     Debug.Log($"{obj.name} stopped falling!");
    // }
    
    // private void ApplyGravity()
    // {
    //     foreach (Transform obj in new HashSet<Transform>(fallingObjects))
    //     {
    //         if (obj == null) continue;
            
    //         // Apply custom gravity for more control
    //         Vector2 currentVelocity = fallVelocities.ContainsKey(obj) ? fallVelocities[obj] : Vector2.zero;
    //         currentVelocity.y -= gravityStrength * Time.deltaTime;
    //         currentVelocity.y = Mathf.Max(currentVelocity.y, -maxFallSpeed);
    //         fallVelocities[obj] = currentVelocity;
            
    //         // Apply movement
    //         obj.position += (Vector3)currentVelocity * Time.deltaTime;
    //     }
    // }
    
    // private void CheckGroundCollisions()
    // {
    //     foreach (Transform obj in new HashSet<Transform>(fallingObjects))
    //     {
    //         if (obj == null) continue;
            
    //         // Check if object hit the ground
    //         RaycastHit2D hit = Physics2D.Raycast(obj.position, Vector2.down, 0.5f, groundLayerMask);
    //         if (hit.collider != null)
    //         {
    //             OnObjectHitGround(obj);
    //         }
    //     }
    // }
    
    // private void OnObjectHitGround(Transform obj)
    // {
    //     Debug.Log($"{obj.name} hit the ground!");
        
    //     // Implement game over or reset logic here
    //     if (obj.CompareTag("Player"))
    //     {
    //         // Game over - reload scene or show game over screen
    //          UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    //     }
    //     else if (obj.CompareTag("Box"))
    //     {
    //         // Box hit ground - you might want to leave it there or destroy it
    //         // Destroy(obj.gameObject);
    //     }
        
    //     StopFalling(obj);
    // }
    
    // private void DisableCharacterControls()
    // {
    //     // Disable Fey controls
    //     FeyNewControl feyControl = FindObjectOfType<FeyNewControl>();
    //     if (feyControl != null)
    //     {
    //         feyControl.SetMovementEnabled(false);
    //     }
        
    //     // Disable Dragon controls using your existing event system
    //     // PlayerOneMovement.NotifyDragonMovementStarted();
    // }
    
    // private void EnableCharacterControls()
    // {
    //     // Enable Fey controls
    //     FeyNewControl feyControl = FindObjectOfType<FeyNewControl>();
    //     if (feyControl != null)
    //     {
    //         feyControl.SetMovementEnabled(true);
    //     }
        
    //     // Enable Dragon controls
    //      //PlayerOneMovement.NotifyDragonMovementStarted();
    // }
    
    // public bool IsObjectFalling(Transform obj)
    // {
    //     return fallingObjects.Contains(obj);
    // }
}