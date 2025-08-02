using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class SokobanControls : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private LayerMask Obstacles;
    [SerializeField] private LayerMask ObjectsToPush;
    [SerializeField] private float raycast = 1f;
    [SerializeField] private Rigidbody2D playerRB;


    // // bool isFacingRight = true;
    // // [SerializeField] private Rigidbody2D playerRB;

    // private PlayerControls playerControls;
    // // private PlayerInput playerInput;
    // private Vector2 direction;
    // private Vector3 targetPosition;
    // private bool isMoving;

    private InputAction playerControls;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private Vector3 targetPosition;
    private bool isMoving;


    private Vector2 moveDirection = Vector2.zero;

    private void Onable()
    {
        playerControls.Enable();

    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    void Awake()
    {



    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveInput = playerControls.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
    
    }
}
