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

    [Header("Slug Settings (Left Click)")]
    public float slugRange = 100f;
    public string creatureTag = "creature";

    [Header("Buckshot Settings (Right Click)")]
    public float buckshotRange = 20f;
    public string itemTag = "item";
    public float recoilForce = 20f;
    public int maxBuckshotAmmo = 5;
    private int currentBuckshotAmmo;

    // Expose ammo for UI
    public int CurrentBuckshotAmmo { get { return currentBuckshotAmmo; } }

    void Start()
    {
        currentBuckshotAmmo = maxBuckshotAmmo;

        if (playerRb == null)
            playerRb = GetComponentInParent<Rigidbody>();
        
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
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

    void FireSlug()
    {
        if (muzzle == null) return;

        RaycastHit hit;
        // Raycast from muzzle forward
        if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, slugRange))
        {
            UnityEngine.Debug.Log("Slug hit: " + hit.collider.name);
            if (hit.collider.CompareTag(creatureTag))
            {
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
