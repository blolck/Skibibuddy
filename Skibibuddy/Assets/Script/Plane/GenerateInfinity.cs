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
        int xMove = (int)(player.transform.position.x-startPosition.x);
        int zMove = (int)(player.transform.position.z-startPosition.z);

        if(Mathf.Abs(xMove) >= planeSize || Mathf.Abs(zMove) >= planeSize)
        {
            float updateTime = Time.realtimeSinceStartup;

            int playerX= (int)(Mathf.Floor(player.transform.position.x/planeSize)*planeSize);
            int playerZ= (int)(Mathf.Floor(player.transform.position.z/planeSize)*planeSize);
            
            for (int x = -halfTilesX; x <= halfTilesX; x++)
            {
                for (int z = -halfTilesZ; z <= halfTilesZ; z++)
                {
                    Vector3 pos = new Vector3((x * planeSize + playerX), 0, (z * planeSize + playerZ));
                    string tilename = "Tile_" + ((int)pos.x).ToString() + "_" + ((int)pos.z).ToString();

                    if (!tiles.ContainsKey(tilename))
                    {
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
                if (tls.creationTime == updateTime)
                {
                    newTerrains.Add(tls.theTile.name, tls);
                }
                else
                {
                    Destroy(tls.theTile);
                }
            }
            tiles = newTerrains;
            startPosition = player.transform.position;
        }
    }
}
