using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Store tile information
  class Tile 
  {
        public GameObject theTile;
        public float creationTime;

        public Tile(GameObject t, float ct) {
            theTile = t;
            creationTime = ct;
        }
}

public class GenerateInfinity : MonoBehaviour
{
    public GameObject plane;
    public GameObject player;
    int planeSize = 10;
    int halfTilesX = 10;
    int halfTilesZ = 10;
    Vector3 startPosition;  
    Hashtable tiles = new Hashtable();

    void Start()
    {
        
        if (player != null)
        {
            int playerX = (int)(Mathf.Floor(player.transform.position.x / planeSize) * planeSize);
            int playerZ = (int)(Mathf.Floor(player.transform.position.z / planeSize) * planeSize);
            startPosition = new Vector3(playerX, 0, playerZ);
        }
        else
        {
            startPosition = Vector3.zero;
        }

        float updateTime= Time.realtimeSinceStartup;
        for (int x = -halfTilesX; x <= halfTilesX; x++)
        {
            for (int z = -halfTilesZ; z <= halfTilesZ; z++)
            {
                Vector3 pos = new Vector3((x * planeSize + startPosition.x), 0, (z * planeSize + startPosition.z));
                GameObject t = (GameObject) Instantiate(plane, pos, Quaternion.identity);
                string tilename = "Tile_" + ((int)pos.x).ToString() + "_" + ((int)pos.z).ToString();
                t.name = tilename;
                Tile tile = new Tile(t, updateTime);
                tiles.Add(tilename, tile);
            }
        }
    }

    void Update()
    {
        // 1. 计算玩家相对于上一次中心点的位移
        int xMove = (int)(player.transform.position.x-startPosition.x);
        int zMove = (int)(player.transform.position.z-startPosition.z);

        // 2. 判断是否移动超过了一个地块的距离
        // Mathf.Abs 是取绝对值。只要在X轴或Z轴任意方向走远了，就触发更新。
        if(Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize)
        {
            float updateTime = Time.realtimeSinceStartup;

            int playerX= (int)(Mathf.Floor(player.transform.position.x/planeSize)*planeSize);
            int playerZ= (int)(Mathf.Floor(player.transform.position.z/planeSize)*planeSize);
            
            // 4. 扫描玩家周围的新区域
            for (int x = -halfTilesX; x <= halfTilesX; x++)
            {
                for (int z = -halfTilesZ; z <= halfTilesZ; z++)
                {
                    // 计算这一格的世界坐标
                    Vector3 pos = new Vector3((x * planeSize + playerX), 0, (z * planeSize + playerZ));
                    string tilename = "Tile_" + ((int)pos.x).ToString() + "_" + ((int)pos.z).ToString();
                    
                    // 5. 核心判断：这个位置有地块吗？
                    if (!tiles.ContainsKey(tilename))
                    {
                        // 没有 -> 生成新的
                        GameObject t = (GameObject)Instantiate(plane, pos, Quaternion.identity);
                        t.name = tilename;
                        Tile tile = new Tile(t, updateTime);
                        tiles.Add(tilename, tile);
                    }
                    else
                    {
                       
                        (tiles[tilename] as Tile).creationTime = updateTime;
                    }
                }
            }
            

            Hashtable newTerrains = new Hashtable();
            foreach (Tile tls in tiles.Values)
            {
                // 清理老地块
                if (tls.creationTime == updateTime)
                {
                    // 仍然活跃的地块，保留
                    newTerrains.Add(tls.theTile.name, tls);
                }
                else
                {
                    // 不活跃的地块，销毁
                    Destroy(tls.theTile);
                }
            }
            tiles = newTerrains;
            startPosition = player.transform.position;
        }
    }
}
