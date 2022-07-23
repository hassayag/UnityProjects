using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject mapObj;
    public int dupeLength = 3;
    public GameObject mapColliderObj;
    public GameObject player;
    public GameObject floorTile, wallTile, waterTile;

    bool allowInput;
    GameObject mapParent;
    PlayerController playerController;
    BoxCollider mapCollider;
    TileMap map;

    public string[,] mapArray;
    public Transform[,] tileArray;

    void Awake()
    {
        allowInput = true;

        mapCollider = mapColliderObj.GetComponent<BoxCollider>();
        playerController = player.GetComponent<PlayerController>();
        
        loadMap(mapObj);
        renderMap();
    }

    void loadMap(GameObject mapObj)
    {
        map = mapObj.GetComponent<TileMap>();
        (mapArray, tileArray) = buildMapData(map);
    }

    (string[,], Transform[,]) buildMapData(TileMap _map)
    {
        string[,] _mapArray = new string[_map.size.x, _map.size.y];
        Transform[,] _tileArray = new Transform[_map.size.x, _map.size.y];

        foreach (Transform tile in _map.transform)
        {   
            Vector2Int _tilePos = new Vector2Int ((int) tile.localPosition.x, (int) tile.localPosition.z);
            
            if (tile.gameObject.tag == "Floor")
            {
                _mapArray[_tilePos.x, _tilePos.y] = "F";
            }
            else if (tile.gameObject.tag == "Wall")
            {
                _mapArray[_tilePos.x, _tilePos.y] = "W";
            }
            else if (tile.gameObject.tag == "Water")
            {
                _mapArray[_tilePos.x, _tilePos.y] = "A";
            }

            _tileArray[_tilePos.x, _tilePos.y] = tile;
        }

        return (_mapArray, _tileArray);
    }

    void Update()
    {
        // limitMapDisplayArea();
        // debugMapDisplay();
    }

    void limitMapDisplayArea()
    {
        foreach (Transform tileGroup in transform)
        {   
            foreach (Transform tile in tileGroup)
            {
                if (mapCollider.bounds.Contains(tile.position))
                {
                    tile.gameObject.SetActive(true);
                }
                else
                {
                    tile.gameObject.SetActive(false);
                }
            }
        }
    }

    public void scrollMapSegment(int x, int y, int[] rows, int[] cols)
    {        
        if (!allowInput)
        {
            return;
        }

        allowInput = false;

        // Scrolls map data
        string[,] arrayOut = new string[map.size.x, map.size.y];

        List<Vector2Int> movedTilePositions = new List<Vector2Int>();
        
        for (int i=0; i < map.size.x; i++)
        {
            for (int j=0; j < map.size.y; j++)
            {
                // Check if the tile is in the row/column allowed for this movement
                if (Array.Exists(cols, val => val == i) || Array.Exists(rows, val => val == j))
                {
                    // wrap position around the 2D array
                    var _newPos = new Vector2Int (wrapIndex(i, x, map.size.x),wrapIndex(j, y, map.size.y));

                    // update map data and tile positions
                    arrayOut[_newPos.x, _newPos.y] = mapArray[i,j];

                    // store the tiles which have been moved so we can animate them later
                    movedTilePositions.Add(new Vector2Int(i,j));
                }
                // If not, maintain the same tile pos
                else
                {
                    arrayOut[i,j] = mapArray[i,j];
                }
            }
        }

        // Ensure new map state leaves the player on a floor tile
        Vector2Int playerPos = playerController.tilePos;
        string tileType = arrayOut[playerPos.x, playerPos.y];

        if (tileType != "F")
        {
            allowInput = true;

            return;
        }

        mapArray = arrayOut;

        StartCoroutine(animateMovement(movedTilePositions, new Vector2Int(x,y), 0.3f));
    }    

    void renderMap()
    {
        if (mapParent != null)
        {
            Destroy(mapParent);
        }

        mapParent = new GameObject("MapParent");

        for (int i=0; i < map.size.x; i++)
        {
            for (int j=0; j < map.size.y; j++)
            {
                string tileType = mapArray[i, j];

                GameObject tile = null;

                if (tileType == "F")
                {
                    tile = Instantiate(floorTile, new Vector3 (i,0,j), Quaternion.identity, mapParent.transform);
                }
                else if (tileType == "W")
                {
                    tile = Instantiate(wallTile, new Vector3 (i,0,j), Quaternion.identity, mapParent.transform);
                }
                else if (tileType == "A")
                {
                    tile = Instantiate(waterTile, new Vector3 (i,0,j), Quaternion.identity, mapParent.transform);
                }

                if (tile != null)
                {
                    tileArray[i,j] = tile.transform;
                }
            }
        }
    }

    int wrapIndex(int index, int delta, int length)
    {
        int indexOut = index + delta;
        if (indexOut < 0)
        {
            indexOut = length-1;
        }
        else if (indexOut > length-1)
        {
            indexOut = 0;
        }

        return indexOut;
    }

    IEnumerator animateMovement(List<Vector2Int> tilePosList, Vector2Int dir, float lerpDuration)
    {
        foreach (Vector2Int tilePos in tilePosList)
        {
            StartCoroutine(smoothTranslate(tilePos, dir, lerpDuration));
        }
        yield return new WaitForSeconds(lerpDuration);

        renderMap();

        // Allow input after we have rendered the new tile positions
        allowInput = true;
    }

    IEnumerator smoothTranslate(Vector2Int pos, Vector2Int dir, float lerpDuration)
    {
        Vector3 startPos = new Vector3(pos.x, 0, pos.y);
        Vector3 finalPos = new Vector3(pos.x + dir.x, 0, pos.y + dir.y);

        float timeElapsed = 0;

        Transform tile = tileArray[pos.x, pos.y];

        if (tile != null)
        {
            while (timeElapsed < lerpDuration)
            {   
                tile.position = Vector3.Lerp(startPos, finalPos, timeElapsed/lerpDuration);

                timeElapsed += Time.deltaTime;   

                yield return null;
            }
        }

        tile.position = new Vector3Int((int) finalPos.x, 0, (int) finalPos.y);
    }

    // void debugMapDisplay()
    // {
    //     if (debugParent != null)
    //     {
    //         Destroy(debugParent);
    //     }
    //     debugParent = new GameObject("DebugParent");

    //     for (int i=0; i<map.size.x; i++)
    //     {
    //         for (int j=0; j<map.size.y; j++)
    //         {
    //             string tileType = mapArray[i, j];
    //             if (tileType == "F")
    //             {
    //                 Instantiate(floorTile, new Vector3 (i,10,j), Quaternion.identity, debugParent.transform);
    //             }
    //             else if (tileType == "W")
    //             {
    //                 Instantiate(wallTile, new Vector3 (i,10,j), Quaternion.identity, debugParent.transform);
    //             }
    //         }
    //     }
    // }
}
