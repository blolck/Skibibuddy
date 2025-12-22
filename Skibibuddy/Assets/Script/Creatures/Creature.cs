using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float groundDrag = 5f;
    public float airMultiplier = 0.4f;
    
    [Header("Speed Limits")]
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float maxNormalSpeed = 20f;

    [Header("Ground Check")]
    public float creatureHeight = 2f;
    public LayerMask whatIsGround;
    protected bool grounded;

    protected Rigidbody rb;
    protected bool isRidden = false;
    protected PlayerController mountedPlayerController = null;

    [Header("Riding Settings")]
    public float rideSpeedMultiplier = 1.5f;
    public float mountCooldown = 1.0f; // Cooldown to prevent immediate remounting
    private float lastUnmountTime = -100f;

    [Header("Crash Settings")]
    public float minCrashSpeed = 12f; // Speed required to trigger crash
    private Vector3 lastFrameVelocity;

    // store original player speed values to restore on unmount
    private float origPlayerMinSpeed;
    private float origPlayerMaxSpeed;
    private float origPlayerMaxNormalSpeed;
    private float origCreatureMoveSpeed;
    private int origLayer; // Store original layer
    
    // Input variables for ridden state
    float horizontalInput;
    float verticalInput;

   
    private Transform playerTransform;
    private float destroyDistance = -1f; 

    public virtual void Init(Transform player, float distance)
    {
        playerTransform = player;
        destroyDistance = distance;
    }

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // Generic creature starts upright or however it is placed in scene
    }

    protected virtual void Update()
    {
        // Check for destruction
        if (destroyDistance > 0 && playerTransform != null && !isRidden)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) > destroyDistance)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (isRidden && mountedPlayerController != null)
        {
            FollowPlayer();
            return; 
        }

        // Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, creatureHeight * 0.5f + 0.2f, whatIsGround);

        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (isRidden)
        {
            
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > maxNormalSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * maxNormalSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    protected virtual void FollowPlayer()
    {
        //creature FOLLOW
        Vector3 targetPos = mountedPlayerController.transform.position;
        
        float playerHalfHeight = mountedPlayerController.playerHeight * 0.5f;
        targetPos.y -= playerHalfHeight; 

        transform.position = targetPos;

        Vector3 targetRot = mountedPlayerController.transform.eulerAngles;
        // Generic: Sync Y with player, keep X/Z 0 (upright)
        transform.rotation = Quaternion.Euler(0f, targetRot.y, 0f);
    }

    protected virtual void FixedUpdate()
    {
        if (isRidden)
        {
           CheckForCrash();
        }
        else
        {
            MoveAuto();
        }
    }

    protected void CheckForCrash()
    {
        if (mountedPlayerController == null) return;
        
        Rigidbody playerRb = mountedPlayerController.GetComponent<Rigidbody>();
        if (playerRb == null) return;

        Vector3 currentVelocity = playerRb.velocity;
        
        // Check if we were moving fast enough in the last frame
        if (lastFrameVelocity.magnitude > minCrashSpeed)
        {
            // Check if we stopped suddenly (collision)
            // If speed drops to near zero instantly
            if (currentVelocity.magnitude < 0.5f) 
            {
                Debug.Log("Crash detected! Speed dropped from " + lastFrameVelocity.magnitude + " to " + currentVelocity.magnitude);
                PerformCrash();
                return;
            }
        }
        
        lastFrameVelocity = currentVelocity;
    }

    protected void PerformCrash()
    {
        if (mountedPlayerController != null)
        {
            GameObject player = mountedPlayerController.gameObject;
            UnmountPlayer(player);
            Destroy(gameObject);
        }
    }

    protected virtual void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    protected virtual void MoveAuto()
    {
        // Generic forward movement
        Vector3 flatForward = transform.forward;
        
        if (grounded)
            rb.AddForce(flatForward * moveSpeed * 3f, ForceMode.Force);
        else
            rb.AddForce(flatForward * moveSpeed * 3f * airMultiplier, ForceMode.Force);
    }

    protected virtual void MoveWithPlayer()
    {
        Vector3 moveDirectionX = transform.right * horizontalInput;
        Vector3 moveDirectionZ = transform.forward;

        // Apply forces
        // 1. Left/Right
        if (grounded)
            rb.AddForce(moveDirectionX.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirectionX.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // 2. Forward/Backward
        float zForceMultiplier = 1f;
        if (verticalInput > 0) zForceMultiplier = 1.5f;
        else if (verticalInput < 0) zForceMultiplier = 0.5f;

        if (grounded)
            rb.AddForce(moveDirectionZ.normalized * moveSpeed * 10f * zForceMultiplier, ForceMode.Force);
        else
            rb.AddForce(moveDirectionZ.normalized * moveSpeed * 10f * airMultiplier * zForceMultiplier, ForceMode.Force);
    }

    protected virtual void SpeedControl()
    {
       
        Vector3 localVel = transform.InverseTransformDirection(rb.velocity);

        // Limit left/right speed
        if (Mathf.Abs(localVel.x) > moveSpeed)
        {
            localVel.x = Mathf.Sign(localVel.x) * moveSpeed;
        }

        // Limit forward speed
        float currentMaxZSpeed = maxNormalSpeed;

        if (verticalInput > 0)
        {
            currentMaxZSpeed = maxSpeed;
        }
        else if (verticalInput < 0)
        {
            currentMaxZSpeed = minSpeed;
        }

        if (localVel.z > currentMaxZSpeed)
        {
            localVel.z = currentMaxZSpeed;
        }

        rb.velocity = transform.TransformDirection(localVel);
    }
// Apply speed boosts to the player's PlayerController while riding
    protected virtual void ApplyRideEffects(PlayerController pc)
    {
        // store originals
        origPlayerMinSpeed = pc.minSpeed;
        origPlayerMaxSpeed = pc.maxSpeed;
        origPlayerMaxNormalSpeed = pc.maxNormalSpeed;
        origCreatureMoveSpeed = moveSpeed;

        // apply multiplier
        pc.minSpeed = pc.minSpeed * rideSpeedMultiplier;
        pc.maxSpeed = pc.maxSpeed * rideSpeedMultiplier;
        pc.maxNormalSpeed = pc.maxNormalSpeed * rideSpeedMultiplier;
        
        // Boost creature speed as well since it carries the player
        moveSpeed = moveSpeed * rideSpeedMultiplier;
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the player
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        
        // If not player directly, check if it's a creature being ridden by a player
        if (player == null)
        {
            Creature otherCreature = collision.gameObject.GetComponent<Creature>();
            if (otherCreature != null && otherCreature.isRidden)
            {
                player = otherCreature.mountedPlayerController;
            }
            
            // Or check parent
            if (player == null)
            {
                player = collision.gameObject.GetComponentInParent<PlayerController>();
            }
        }

        if (player != null)
        {
            // Check cooldown
            if (Time.time < lastUnmountTime + mountCooldown) return;

            bool isSwitching = false;
            // If the player is already riding another creature, unmount from it first
            if (player.currentMount != null && player.currentMount != this)
            {
                player.currentMount.UnmountPlayer(player.gameObject);
                isSwitching = true;
                Debug.Log("Switching mounts");
            }

            // If not already ridden by this player (or anyone else), mount
            if (!isRidden)
            {
                MountPlayer(player.gameObject, isSwitching);
            }
        }
    }

    // MountPlayer positions the player and sets up the riding state
    protected virtual void MountPlayer(GameObject player, bool isSwitching = false)
    {
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc == null) return;

        // 1. Set state
        isRidden = true;
        mountedPlayerController = pc;
        pc.currentMount = this;

        // 2. Physics & Parenting
        // Disable creature physics so it doesn't fight player
        // Fix: Set velocity to zero BEFORE setting kinematic to avoid "Setting linear velocity of a kinematic body is not supported" error
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;
 
        rb.detectCollisions = false;
        
        // Change layer to Ignore Raycast (usually layer 2) to prevent infinite jump
        origLayer = gameObject.layer;
        gameObject.layer = 2; 
        
        // DO NOT Parent Creature to Player (Creature stays independent but follows in Update)
        // transform.SetParent(player.transform); 
        
        // 3. Position & Rotation
        // Initial placement
        Vector3 targetPos = player.transform.position;
        
        float playerHalfHeight = pc.playerHeight * 0.5f;
        targetPos.y -= playerHalfHeight; 
        
        transform.position = targetPos;
        // Generic: Keep X rotation at 0, sync Y with player
        transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 0f);

        pc.SetRidingState(true, creatureHeight);
        
        // Move Player UP in world space so they don't snap into ground
        if (!isSwitching)
        {
            player.transform.position += Vector3.up * creatureHeight;
        }

        // 5. Apply stats (speed boost, etc.)
        ApplyRideEffects(pc);

        // Initialize crash detection
        if (pc.GetComponent<Rigidbody>() != null)
        {
            lastFrameVelocity = pc.GetComponent<Rigidbody>().velocity;
        }
    }

    

    // Public: unmount player and restore speeds
    public void UnmountPlayer(GameObject player)
    {
        if (mountedPlayerController != null)
        {
            // Restore speeds
            mountedPlayerController.minSpeed = origPlayerMinSpeed;
            mountedPlayerController.maxSpeed = origPlayerMaxSpeed;
            mountedPlayerController.maxNormalSpeed = origPlayerMaxNormalSpeed;
            
            // Restore Player Physics (Collider)
            mountedPlayerController.SetRidingState(false, 0);
            
            // Clear mount reference on player
            mountedPlayerController.currentMount = null;
            
            mountedPlayerController = null;
        }
        
        // Restore creature speed
        moveSpeed = origCreatureMoveSpeed;

        // Restore physics and parent
        // transform.SetParent(null); // No longer parented
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.detectCollisions = true;
        
        // Restore layer
        gameObject.layer = origLayer;
        
        // Stop ridden state
        isRidden = false;
        lastUnmountTime = Time.time;
    }
}
