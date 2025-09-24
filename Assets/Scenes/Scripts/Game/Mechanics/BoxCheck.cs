using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCheck : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            //  spriteRenderer.color = Color.green;
            FairyMovement.currentBoxes++;
            Debug.Log(FairyMovement.currentBoxes);
        }

    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Point"))
        {
            // spriteRenderer.color = Color.red;
            FairyMovement.currentBoxes--;
            Debug.Log(FairyMovement.currentBoxes);
        }

    }


}




// {
//     private PlayerControls controls;
//     [SerializeField] private Tilemap groundTilemap;
//     // [SerializeField] private Tilemap collisionTilemap;

//     private Animator animator;
//     private SpriteRenderer spriteRenderer;
//     private Vector2 movementInput = Vector2.zero;
//     private string lastDirection = "Down";
//     private bool isMoving = false;
    
//     // Movement variables
//     private Vector3 targetPosition;
//     private bool isSmoothingMovement = false;
//     public float moveSpeed = 5f;

//     private void Awake()
//     {
//         controls = new PlayerControls();
//         animator = GetComponent<Animator>();
//         spriteRenderer = GetComponent<SpriteRenderer>();
//     }

//     private void OnEnable()
//     {
//         controls.Enable();
//     }
    
//     private void OnDisable()
//     {
//         controls.Disable();
//     }

//     void Start()
//     {
        
//         controls.Player.Fey.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
//         controls.Player.Fey.canceled += ctx =>
//         {
//             movementInput = Vector2.zero;
//             isMoving = false;
//         };
//     }

//     void FixedUpdate()
//     {

//         isMoving = movementInput != Vector2.zero && CanMove(movementInput);

//         if (movementInput != Vector2.zero && !isSmoothingMovement)
//         {
//             Move(movementInput);
//             // Debug.Log(movementInput);
//         }
        
//         HandleAnimations();
//     }

//     private void Move(Vector2 direction)
//     {
//         if (CanMove(direction))
//         {
//             targetPosition = transform.position + (Vector3)direction;
//             StartCoroutine(SmoothMovement());
//             UpdateLastDirection(direction);
//         }
//         else
//         {
//             isMoving = false;
//         }
//     }

//     private IEnumerator SmoothMovement()
//     {
//         isSmoothingMovement = true;
//         isMoving = true;
        
//         float remainingDistance = Vector3.Distance(transform.position, targetPosition);
        
//         while (remainingDistance > float.Epsilon)
//         {
//             transform.position = Vector3.MoveTowards(
//                 transform.position, 
//                 targetPosition, 
//                 moveSpeed * Time.deltaTime
//             );
            
//             remainingDistance = Vector3.Distance(transform.position, targetPosition);
//             yield return null;
//         }
        
//         transform.position = targetPosition;
//         isSmoothingMovement = false;
        
//         if (movementInput != Vector2.zero && CanMove(movementInput))
//         {
//             Move(movementInput);
//         }
//         else
//         {
//             isMoving = false;
//         }
//     }

//     private void HandleAnimations()
//     {
//         if (animator == null) return;
//         string animationName = isMoving ? "Walking" : "Idle";
//         animator.Play(animationName + lastDirection);
//     }

//     private void UpdateLastDirection(Vector2 direction)
//     {
//         if (direction.x > 0) lastDirection = "Right";
//         else if (direction.x < 0) lastDirection = "Left";
//         else if (direction.y > 0) lastDirection = "Up";
//         else if (direction.y < 0) lastDirection = "Down";
//     }

//     private bool CanMove(Vector2 direction)
//     {
//         Vector3Int gridPosition = groundTilemap.WorldToCell(transform.position + (Vector3)direction);
//         if (!groundTilemap.HasTile(gridPosition)
//         // || collisionTilemap.HasTile(gridPosition)
        
//         )
//             return false;
//         return true;
//     }
// }