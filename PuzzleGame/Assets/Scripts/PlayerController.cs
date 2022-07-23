using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject tileMapLoader;
    MapLoader tileMap;

    public Vector2Int tilePos;
    Vector2Int lastTilePos;


    // Start is called before the first frame update
    void Start()
    {
        tileMap = tileMapLoader.GetComponent<MapLoader>();
        tilePos = new Vector2Int(0,0);
        lastTilePos = tilePos;
    }

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetKeyDown("up"))
        {
            tilePos.y += 1;
        }
        else if (Input.GetKeyDown("down"))
        {
            tilePos.y -= 1;
        }
        else if (Input.GetKeyDown("left"))
        {
            tilePos.x -= 1;
        }
        else if (Input.GetKeyDown("right"))
        {
            tilePos.x += 1;
        }

        // only set pos if player has moved
        if (tilePos != lastTilePos)
        {
            string tileType = null;

            try
            {
                tileType = tileMap.mapArray[tilePos.x, tilePos.y];
            }
            catch
            {
                tilePos = lastTilePos;
            }

            // only allow movement to floor tiles
            if (tileType != null && tileType == "F")
            {
                transform.position = tileToWorldPos(tilePos);
            }
            // move failed, reset tile pos
            else
            {
                tilePos = lastTilePos;
            }
        }

        lastTilePos = tilePos;
    }

    Vector3 tileToWorldPos(Vector2 pos)
    {
        return new Vector3 (pos.x, 0.5f, pos.y);
    }
}
