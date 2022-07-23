using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    MapLoader mapLoader;
    int[] rows, cols, rows_empty, cols_empty;

    void Start()
    {
        mapLoader = GetComponent<MapLoader>();
        rows_empty = new int[]{};
        cols_empty = new int[]{};
        rows = new int[]{2,3,4};
        cols = new int[]{1,2};
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            mapLoader.scrollMapSegment(0, 1, rows_empty, cols);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            mapLoader.scrollMapSegment(0, -1, rows_empty, cols);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            mapLoader.scrollMapSegment(-1, 0, rows, cols_empty);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            mapLoader.scrollMapSegment(1, 0, rows, cols_empty);
        }
    }
}
