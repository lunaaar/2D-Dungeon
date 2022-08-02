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
        
        //Debug.Log(tilemap.cellBounds);
        // Debug.Log("-------");

        foreach (Vector3Int point in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(point))
            {
                //Debug.Log("TEST");
                Debug.Log(tilemap.CellToWorld(point));
                tilemap.SetTileFlags(point, TileFlags.None);
                tilemap.SetColor(point, Color.blue);
            }
        }

        //tilemap.RefreshAllTiles();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("TEST");
            Tilemap tilemap = this.GetComponent<Tilemap>();
            Vector3Int point = new Vector3Int(1, 1, 0);
            tilemap.SetTileFlags(point, TileFlags.None);
            tilemap.SetColor(point, Color.green);
        }
    }
}
