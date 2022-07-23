using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMap : MonoBehaviour
{
    public Vector2Int size;
    public GameObject floorTile, wallTile;
    public string[,] mapArray;

    // Start is called before the first frame update
    void Awake()
    {
        // createMap();
    }

    void createMap()
    {
        mapArray = new string[size.x, size.y];

        for (int i=0; i<size.x; i++)
        {
            for (int j=0; j<size.y; j++)
            {
                var r = Random.Range(0,10f);
                if (r<8f)
                {
                    var tile = Instantiate(floorTile, new Vector3(i,0,j), Quaternion.identity, transform);
                    tile.tag = "Floor";
                    mapArray[i,j] = "F";
                }
                else
                {
                    var tile = Instantiate(wallTile, new Vector3(i,0,j), Quaternion.identity, transform);
                    tile.tag = "Wall";
                    mapArray[i,j] = "M";
                }
            }
        }
    }

}
