using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    [Header("References")]
    public Transform muzzle; // The point where the raycast originates
    public Transform cameraTransform; // To determine recoil direction
    public Rigidbody playerRb; // To apply recoil force
    public PlayerChainsaw playerChainsaw; // Reference to the chainsaw script

    [Header("Slug Settings (Left Click)")]
    public float slugRange = 100f;
    public string creatureTag = "creature";

    [Header("Buckshot Settings (Right Click)")]
    public float buckshotRange = 20f;
    public string itemTag = "item";
    public float recoilForce = 20f;
    public int maxBuckshotAmmo = 5;
    private int currentBuckshotAmmo;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip fireSound;

    [Header("UI References")]
    public kills killsScript;

    // Expose ammo for UI
    public int CurrentBuckshotAmmo { get { return currentBuckshotAmmo; } }

    void Start()
    {
        currentBuckshotAmmo = maxBuckshotAmmo;

        if (killsScript == null)
            killsScript = FindObjectOfType<kills>();

        if (playerRb == null)
            playerRb = GetComponentInParent<Rigidbody>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
            
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Weapon Switching: Press 1 to switch to Chainsaw
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToChainsaw();
            return; // Exit update to prevent firing on the same frame
        }

        // Left Click: Slug Fire
        if (Input.GetButtonDown("Fire1"))
        {
            UnityEngine.Debug.Log("Firing Slug");
            FireSlug();
        }

        // Right Click: Buckshot Fire
        if (Input.GetButtonDown("Fire2"))
        {
            UnityEngine.Debug.Log("Firing Buckshot");
            FireBuckshot();
        }
    }

    void SwitchToChainsaw()
    {
        if (playerChainsaw != null)
        {
            playerChainsaw.enabled = true;
            playerChainsaw.gameObject.SetActive(true);

            this.enabled = false;
            
            // Only disable this gameObject if the scripts are on different objects
            if (playerChainsaw.gameObject != this.gameObject)
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PlayerChainsaw reference is not assigned in PlayerFire!");
        }
    }

    void FireSlug()
    {
        if (muzzle == null) return;

        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        RaycastHit hit;
        // Raycast from muzzle forward
        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, slugRange))
        {
            UnityEngine.Debug.Log("Slug hit: " + hit.collider.name);
            if (hit.collider.CompareTag(creatureTag))
            {
                if (killsScript != null)
                {
                    killsScript.AddKill();
                }
                Destroy(hit.collider.gameObject);
            }
        }
    }

    void FireBuckshot()
    {
        if (currentBuckshotAmmo <= 0)
        {
            UnityEngine.Debug.Log("Out of Buckshot Ammo!");
            return;
        }

        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        currentBuckshotAmmo--;
        UnityEngine.Debug.Log("Buckshot Ammo Left: " + currentBuckshotAmmo);

        if (muzzle == null) return;

        // 1. Raycast logic
        RaycastHit hit;
        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, buckshotRange))
        {
            UnityEngine.Debug.Log("Buckshot hit: " + hit.collider.name);
            if (hit.collider.CompareTag(itemTag))
            {
                Destroy(hit.collider.gameObject);
            }
        }

        // 2. Recoil logic
        if (playerRb != null && cameraTransform != null)
        {
            // Apply force in the opposite direction of the camera's forward vector
            // We use cameraTransform.forward because muzzle might be slightly offset
            Vector3 recoilDir = -cameraTransform.forward;
            playerRb.AddForce(recoilDir * recoilForce, ForceMode.Impulse);
        }
    }
}
