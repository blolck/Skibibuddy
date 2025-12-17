using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed; 
    public float maxSlopeSpeed = 40f; 
    
    [Header("Slope Physics")]
    public float slopeForce = 20f; 
    public float crashDamping = 0.5f; 

    public float xFriction = 10f; 

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public float groundCheckOffset = 0.1f; 
    public LayerMask whatIsGround;
    bool grounded;
    RaycastHit slopeHit; 

    public Transform orientation;

    float horizontalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        readyToJump = true;
    }

    private void Update()
    {
        
        float rayLength = playerHeight * 0.5f + groundCheckOffset;
        
        // 使用OutSlopeHit 获取碰撞点详细信息
        grounded = Physics.Raycast(transform.position, Vector3.down, out slopeHit, rayLength, whatIsGround);

        Debug.DrawRay(transform.position, Vector3.down * rayLength, grounded ? Color.green : Color.red);

        MyInput();
        SpeedControl();

        transform.rotation = Quaternion.Euler(0, orientation.eulerAngles.y, 0);
        rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
        ApplySlopeForce(); 
        ApplyXAxisFriction(); 
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            Jump();
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        
        moveDirection = Vector3.right * horizontalInput;

        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }


    private void ApplySlopeForce()
    {
        if (grounded)
        {
            Vector3 slopeMoveDir = Vector3.ProjectOnPlane(Vector3.forward, slopeHit.normal).normalized;
            rb.AddForce(slopeMoveDir * slopeForce, ForceMode.Acceleration);
        }
    }

    private void ApplyXAxisFriction()
    {
        if (grounded && horizontalInput == 0)
        {
            Vector3 xVel = new Vector3(rb.velocity.x, 0, 0);
            rb.AddForce(-xVel * xFriction, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
  
        Vector3 xVelocity = new Vector3(rb.velocity.x, 0f, 0f);
        if(xVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedX = xVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedX.x, rb.velocity.y, rb.velocity.z);
        }
        if(rb.velocity.z > maxSlopeSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, maxSlopeSpeed);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground") || collision.relativeVelocity.magnitude > 5f)
        {
           
            rb.velocity = rb.velocity * crashDamping;
            Debug.Log("Crashed");
        }
    }
}