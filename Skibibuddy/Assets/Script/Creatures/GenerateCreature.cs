using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCreature : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject creaturePrefab;
    public float spawnRange = 20f;
    public float spawnInterval = 5f; // Time between spawns
    public float spawnHeightOffset = 2f;
    public float destroyDistance = 150f; // Distance to destroy creature

    [Header("Ground Check")]
    public LayerMask whatIsGround;

    private Transform playerTransform;

    private void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnCreature();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void SpawnCreature()
    {
        if (playerTransform == null || creaturePrefab == null) return;

        // Calculate random position within range
        Vector2 randomCircle = Random.insideUnitCircle * spawnRange;
        Vector3 spawnPos = playerTransform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
        
        // Raycast to find ground height
        if (Physics.Raycast(spawnPos + Vector3.up * 50f, Vector3.down, out RaycastHit hit, 100f, whatIsGround))
        {
            spawnPos.y = hit.point.y + spawnHeightOffset;
        }
        else
        {
            spawnPos.y = playerTransform.position.y + spawnHeightOffset; // Fallback
        }

        GameObject creatureObj = Instantiate(creaturePrefab, spawnPos, Quaternion.identity);
        
        // Initialize creature with destruction settings
        Creature creatureScript = creatureObj.GetComponent<Creature>();
        if (creatureScript != null)
        {
            creatureScript.Init(playerTransform, destroyDistance);
        }
    }
}
