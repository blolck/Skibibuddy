using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    [Header("Terrain Settings")]
    [SerializeField] int heightScale = 5;       // 地形起伏高度
    [SerializeField] float detailScale = 5.0f;  // 地形细节（越小越尖锐）
    
    [Header("Slope Settings")]
    [SerializeField] float slopeScale = 0.3f;   // 坡度系数：值越大，坡越陡

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        
        for (int v=0; v < vertices.Length; v++)
        {
            // 1. 获取该顶点的世界坐标
            float worldX = vertices[v].x + this.transform.position.x;
            float worldZ = vertices[v].z + this.transform.position.z;

            // 2. 计算柏林噪声（原本的起伏）
            float noiseY = Mathf.PerlinNoise(worldX / detailScale, worldZ / detailScale) * heightScale;

            // 3. 计算下坡偏移
            // 逻辑：Z 轴坐标越大（向前走），高度 Y 越低。
            // 负号 (-) 制造了下坡效果。
            float slopeY = -worldZ * slopeScale;

            // 4. 最终高度 = 起伏 + 下坡
            vertices[v].y = noiseY + slopeY;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        // 重新生成碰撞体，确保物理形状和视觉一致
        MeshCollider oldCollider = GetComponent<MeshCollider>();
        if (oldCollider != null) Destroy(oldCollider);
        this.gameObject.AddComponent<MeshCollider>();
    }

    void Update()
    {
        
    }
}
