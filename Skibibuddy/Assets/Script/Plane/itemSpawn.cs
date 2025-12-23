using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] prefabs; 
    public int minCount = 1;  
    public int maxCount = 5;     

    [Header("Difficulty Progression")]
    public int absoluteMaxCount = 4;
    public float timeToReachMax = 20f;
    public float verticalOffset = -0.1f; 
    
    public float planeSize = 10f; 

    [Header("Noise Settings")]
    public float itemNoiseScale = 0.1f; 
    [Range(0f, 1f)]
    public float itemDensityThreshold = 0.5f; 

    public void SpawnItems()
    {
        if (prefabs == null || prefabs.Length == 0) return;

        PlaneController pc = GetComponent<PlaneController>();
        if (pc == null)
        {
             return;
        }

        // Calculate dynamic max count
        float progress = Mathf.Clamp01(Time.timeSinceLevelLoad / timeToReachMax);
        int currentMaxCount = Mathf.RoundToInt(Mathf.Lerp(maxCount, absoluteMaxCount, progress));

        int attemptCount = currentMaxCount * 2; 

        for (int i = 0; i < attemptCount; i++)
        {
    
            float margin = 1.0f;
            float halfSize = (planeSize / 2f) - margin;

            float randX = Random.Range(-halfSize, halfSize);
            float randZ = Random.Range(-halfSize, halfSize);

            float worldX = transform.position.x + randX;
            float worldZ = transform.position.z + randZ;

            float noiseVal = Mathf.PerlinNoise(worldX * itemNoiseScale, worldZ * itemNoiseScale);

            if (noiseVal < itemDensityThreshold) continue;

            float y = pc.GetHeightAt(worldX, worldZ);

      
            GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
            
 
            Vector3 spawnPos = new Vector3(worldX, y + verticalOffset, worldZ);
            

            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360), 0);
            

            GameObject instance = Instantiate(prefabToSpawn, spawnPos, randomRot);
            
            instance.transform.SetParent(this.transform);
        }
    }

    void Update()
    {
        
    }
}
