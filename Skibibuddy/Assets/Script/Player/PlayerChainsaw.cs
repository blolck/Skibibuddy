using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChainsaw : MonoBehaviour
{
    [Header("References")]
    public GameObject attackRange; // The trigger object for the attack
    public PlayerFire playerFire;  // Reference to the shotgun script

    [Header("Settings")]
    public float attackCooldown = 1.0f;
    public float attackDuration = 0.2f;
    [Header("Sound")]

    private float currentCooldownTimer = 0f;
    [SerializeField] private AudioSource chainsawAudioSource;
    [SerializeField] private AudioClip chainsawStartSound;

    // Start is called before the first frame update
    void Start()
    {
        // Ensure attack range is initially disabled
        if (attackRange != null)
            attackRange.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Cooldown Timer Logic
        if (currentCooldownTimer > 0)
        {
            currentCooldownTimer -= Time.deltaTime;
        }

        // Weapon Switching: Press 2 to switch to Shotgun
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToShotgun();
        }
        if (Input.GetButtonDown("Fire1") && currentCooldownTimer <= 0)
        {
            if (chainsawAudioSource != null && chainsawStartSound != null)
            {
                chainsawAudioSource.PlayOneShot(chainsawStartSound);
            }
            Attack();
        }
    }

    void SwitchToShotgun()
    {
        if (playerFire != null)
        {
            playerFire.enabled = true;
            // Also enable the gameobject if they are separate objects
            playerFire.gameObject.SetActive(true);
            
            this.enabled = false;
            
            if (playerFire.gameObject != this.gameObject)
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("PlayerFire reference is not assigned in PlayerChainsaw!");
        }
    }

    void Attack()
    {
        currentCooldownTimer = attackCooldown;
        StartCoroutine(PerformAttack());
    }

    IEnumerator PerformAttack()
    {
        if (attackRange != null)
        {
            attackRange.SetActive(true);
            yield return new WaitForSeconds(attackDuration);
            attackRange.SetActive(false);
        }
    }
}
