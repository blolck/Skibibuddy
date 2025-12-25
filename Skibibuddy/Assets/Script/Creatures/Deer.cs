using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Creature
{
    private bool hasPenguin = false;

    protected override void FollowPlayer()
    {
        if (mountedPlayerController == null) return;
        base.FollowPlayer();
    }

    protected override void MountPlayer(GameObject player, bool isSwitching = false)
    {
        base.MountPlayer(player, isSwitching);
        transform.rotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 0f);
    }

    protected override void ApplyRideEffects(PlayerController pc)
    {
        base.ApplyRideEffects(pc);
        
        // Deer Speed Boost (High)
        float speedMultiplier = 1.5f;

        pc.moveSpeed *= speedMultiplier;       
        pc.maxNormalSpeed *= speedMultiplier;  
        pc.maxSpeed *= speedMultiplier;       

        // Deer Jump Boost
   
        pc.jumpForce *= 1.5f; 
        
    }

    public override bool HandleMountSwitch(Creature newMount)
    {
        // Special interaction with Penguin
        if (newMount is Penguin)
        {
            if (!hasPenguin)
            {
                hasPenguin = true;
                Debug.Log("Deer absorbed Penguin!");
                // Destroy the penguin object
                Destroy(newMount.gameObject);
            }
            return false; 
        }
        return true; // Allow switch for other creatures
    }

    protected override void PerformCrash()
    {

        if (hasPenguin)
        {
            hasPenguin = false;
            Debug.Log("Lost Penguin in crash!");
        }
        else
        {
            base.PerformCrash();
        }
    }
}
