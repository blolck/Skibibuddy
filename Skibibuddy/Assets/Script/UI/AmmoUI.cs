using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoUI : MonoBehaviour
{
    [Header("References")]
    public PlayerFire playerFire;
    private TextMeshProUGUI ammoText;

    void Start()
    {
        ammoText = GetComponent<TextMeshProUGUI>();

        if (playerFire == null)
        {
            // Try to find PlayerFire in the scene (e.g., on the player or weapon)
            playerFire = FindObjectOfType<PlayerFire>();
        }
    }

    void Update()
    {
        if (playerFire != null && ammoText != null)
        {
            ammoText.text = "Ammo: " + playerFire.CurrentBuckshotAmmo + " / " + playerFire.maxBuckshotAmmo;
        }
    }
}
