using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Penguin : Creature
{
    protected override void Start()
    {
        base.Start();
        transform.rotation = Quaternion.Euler(90f, transform.eulerAngles.y, 0f);
    }

    protected override void FollowPlayer()
    {
        if (mountedPlayerController == null) return;
        base.FollowPlayer();

        Vector3 targetRot = mountedPlayerController.transform.eulerAngles;
        transform.rotation = Quaternion.Euler(90f, targetRot.y, 0f);
    }

    protected override void MoveAuto()
    {
        Vector3 flatForward = Quaternion.Euler(0, transform.eulerAngles.z, 0) * Vector3.forward;
        if (grounded)
            rb.AddForce(flatForward * moveSpeed * 3f, ForceMode.Force);
        else
            rb.AddForce(flatForward * moveSpeed * 3f * airMultiplier, ForceMode.Force);
    }

    protected override void MountPlayer(GameObject player, bool isSwitching = false)
    {
        base.MountPlayer(player, isSwitching);
        transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
    }

    protected override void ApplyRideEffects(PlayerController pc)
    {
        base.ApplyRideEffects(pc);
        
        // Penguin Speed Boost
        float speedMultiplier = 1.2f;

        pc.moveSpeed *= speedMultiplier;       
        pc.maxNormalSpeed *= speedMultiplier;  
        pc.maxSpeed *= speedMultiplier;        
        
    }
}
