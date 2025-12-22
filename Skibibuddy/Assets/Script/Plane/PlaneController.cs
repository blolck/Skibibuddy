using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] int heightScale = 5;       
    [SerializeField] float detailScale = 5.0f;  
    [Header("Slope Settings")]
    [SerializeField] float slopeScale = 0.3f;   

    public float GetHeightAt(float worldX, float worldZ)
    {
        float noiseY = Mathf.PerlinNoise(worldX / detailScale, worldZ / detailScale) * heightScale;
        float slopeY = -worldZ * slopeScale;
        return noiseY + slopeY;
    }

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        
        for (int v=0; v < vertices.Length; v++)
        {
            float worldX = vertices[v].x + this.transform.position.x;
            float worldZ = vertices[v].z + this.transform.position.z;

            vertices[v].y = GetHeightAt(worldX, worldZ);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        MeshCollider oldCollider = GetComponent<MeshCollider>();
        if (oldCollider != null) Destroy(oldCollider);
        
        MeshCollider newCollider = this.gameObject.AddComponent<MeshCollider>();
        Physics.SyncTransforms();

        itemSpawn spawner = GetComponent<itemSpawn>();
        if (spawner != null)
        {
            spawner.SpawnItems();
        }
    }
    void Update()
    {
        
    }
}
