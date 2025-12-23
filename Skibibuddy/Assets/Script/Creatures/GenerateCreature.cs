using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCreature : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] PlayerController player;
    [SerializeField] GameObject creaturePrefab1;
    [SerializeField] GameObject creaturePrefab2;
    [SerializeField] Score scoreRef;
    public float spawnRange = 20f;
    public float spawnInterval = 5f; // Time between spawns
    public float spawnHeightOffset = 2f;
    public float destroyDistance = 150f; // Distance to destroy creature
    
    [Header("Score Thresholds")]
    public float spawnSecondPrefabScore = 6000f;

    [Header("Ground Check")]
    public LayerMask whatIsGround;

    private Transform playerTransform;

    private void Start()
    {
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
        if (playerTransform == null) return;

        bool spawnSecondCreature = false;
        if (scoreRef != null)
        {
            if(scoreRef.CurrentScore >= spawnSecondPrefabScore)
            {
                 spawnSecondCreature = true;
            }
        }

        if (creaturePrefab1 != null)
        {
            SpawnOne(creaturePrefab1);
        }

        if (spawnSecondCreature && creaturePrefab2 != null)
        {
            SpawnOne(creaturePrefab2);
        }
    }

    private void SpawnOne(GameObject prefab)
    {
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

        GameObject creatureObj = Instantiate(prefab, spawnPos, Quaternion.identity);
        
        // Initialize creature with destruction settings
        Creature creatureScript = creatureObj.GetComponent<Creature>();
        if (creatureScript != null)
        {
            creatureScript.Init(playerTransform, destroyDistance);
        }
    }
}
