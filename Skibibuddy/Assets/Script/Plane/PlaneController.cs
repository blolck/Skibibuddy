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

    // 【新增】公开方法：根据世界坐标计算地形高度
    // 这样 itemSpawn 就不需要用射线检测了，直接算出来高度是多少
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
            // 1. 获取该顶点的世界坐标
            float worldX = vertices[v].x + this.transform.position.x;
            float worldZ = vertices[v].z + this.transform.position.z;

            // 2. 直接调用上面的公式方法，保证逻辑统一
            vertices[v].y = GetHeightAt(worldX, worldZ);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // 重新生成碰撞体
        MeshCollider oldCollider = GetComponent<MeshCollider>();
        if (oldCollider != null) Destroy(oldCollider);
        
        MeshCollider newCollider = this.gameObject.AddComponent<MeshCollider>();

        // 强制同步物理变换，确保射线检测能立即检测到新生成的碰撞体
        Physics.SyncTransforms();

        // 主动调用 itemSpawn 生成物体
        // 不需要传 collider 了，因为我们改用了数学计算
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
