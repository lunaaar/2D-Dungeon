using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TEST2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        Tilemap tilemap = this.GetComponent<Tilemap>();
        Debug.Log(tilemap.cellBounds);
        Debug.Log("-------");

        foreach (var point in tilemap.cellBounds.allPositionsWithin)
        {
            Debug.Log(point.ToString());
            Debug.Log(tilemap.CellToWorld(point));
            Debug.Log("-------");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
