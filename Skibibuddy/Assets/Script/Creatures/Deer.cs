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
        // 1. Boost jump height and speed more
        pc.jumpForce *= 1.5f; 
        
        // Boost speed further for Deer
        pc.maxSpeed *= 1.2f;
        pc.maxNormalSpeed *= 1.2f;
        moveSpeed *= 1.2f;
    }

    public override bool HandleMountSwitch(Creature newMount)
    {
        // 2. Prevent switching to Penguin, absorb it instead
        if (newMount is Penguin)
        {
            if (!hasPenguin)
            {
                hasPenguin = true;
                Debug.Log("Deer absorbed Penguin!");
                // Destroy the penguin object as it is now "carried"
                Destroy(newMount.gameObject);
            }
            return false; // Block switch
        }
        return true; // Allow switch for other creatures
    }

    protected override void PerformCrash()
    {
        // 3. Crash logic
        if (hasPenguin)
        {
            hasPenguin = false;
            Debug.Log("Lost Penguin in crash!");
            // Optional: Add visual feedback or sound here
        }
        else
        {
            base.PerformCrash();
        }
    }
}
