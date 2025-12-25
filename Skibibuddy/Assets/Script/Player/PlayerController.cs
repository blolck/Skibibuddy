using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;


    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    [Header("Speed Limits")]
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float maxNormalSpeed = 20f;
    
    [Header("Acceleration Settings")]
    public float initialSpeed = 20f;
    public float acceleration = 0.5f; // How much maxNormalSpeed increases per second
    public float absoluteMaxSpeed = 50f;   // The hard cap for maxNormalSpeed

    public Transform orientation;
    
    // Track the creature currently being ridden
    public Creature currentMount;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;
    CapsuleCollider col;
    float originalColHeight;
    Vector3 originalColCenter;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        col = GetComponent<CapsuleCollider>();
        if (col != null)
        {
            originalColHeight = col.height;
            originalColCenter = col.center;
        }

        readyToJump = true;
        maxNormalSpeed = initialSpeed;
    }

    public void SetRidingState(bool riding, float mountHeight)
    {
        if (col == null) return;

        if (riding)
        {
            // Extend collider downwards
            col.height = originalColHeight + mountHeight;
            col.center = originalColCenter - new Vector3(0, mountHeight / 2f, 0);
        }
        else
        {
            // Restore collider
            col.height = originalColHeight;
            col.center = originalColCenter;
        }
    }

    private void Update()
    {
        // Increase maxNormalSpeed over time
        if (maxNormalSpeed < absoluteMaxSpeed)
        {
            maxNormalSpeed += acceleration * Time.deltaTime;
            // Also increase moveSpeed proportionally if needed, or just let SpeedControl handle the cap
            moveSpeed += (acceleration * 0.5f) * Time.deltaTime; 
        }

        MyInput();
        SpeedControl();
        
        // ground check
        float rayLength = playerHeight * 0.5f + 0.3f;
        grounded = Physics.Raycast(transform.position, Vector3.down, rayLength, whatIsGround);
        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            Jump();
            readyToJump = false;
            Debug.Log("Jumped");
            

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //left and right movement
        Vector3 moveDirectionX = orientation.right * horizontalInput;
        
        if (grounded)
            rb.AddForce(moveDirectionX.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirectionX.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        //forward and backward movement 
        Vector3 moveDirectionZ = orientation.forward;
        
        float zForceMultiplier = 1f;
        if (verticalInput > 0) zForceMultiplier = 1.5f;      
        else if (verticalInput < 0) zForceMultiplier = 0.5f; 
        
        if (grounded)
            rb.AddForce(moveDirectionZ.normalized * moveSpeed * 10f * zForceMultiplier, ForceMode.Force);
        else
            rb.AddForce(moveDirectionZ.normalized * moveSpeed * 10f * airMultiplier * zForceMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        // 1. Component-based clamping (Local Space)
        Vector3 localVel = orientation.InverseTransformDirection(rb.velocity);

        // limit left right speed
        if (Mathf.Abs(localVel.x) > moveSpeed)
        {
            localVel.x = Mathf.Sign(localVel.x) * moveSpeed;
        }

        //limit forward speed
        float currentMaxZSpeed = maxNormalSpeed; 

        // Only use minSpeed when moving backwards
        if (verticalInput < 0)
        {
            currentMaxZSpeed = minSpeed;
        }
        
        // When moving forward (verticalInput > 0) or idle, use maxNormalSpeed 
        // which is dynamically increased in Update()

        if (localVel.z > currentMaxZSpeed)
        {
            localVel.z = currentMaxZSpeed;
        }

        rb.velocity = orientation.TransformDirection(localVel);

        // 2. Magnitude-based clamping (World Space)
        // Fix: Prevent diagonal acceleration (strafe jumping) by clamping total horizontal magnitude
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > maxNormalSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxNormalSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}