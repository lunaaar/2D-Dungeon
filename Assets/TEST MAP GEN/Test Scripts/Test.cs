using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Test : MonoBehaviour
{
    public List<GameObject> prefabTests;

    public RaycastHit2D[] results = new RaycastHit2D[10];


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GameObject t2 = Instantiate(prefabTests[1], this.transform.position, new Quaternion(0, 0, 0, 1), this.transform);
            Debug.Log(t2.transform.Find("Walls").GetComponent<TilemapCollider2D>().bounds);
        }
        
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject t3 = Instantiate(prefabTests[2], this.transform.position, new Quaternion(0, 0, 0, 1), this.transform);

            BoxCollider2D test = t3.GetComponent<BoxCollider2D>();

            Debug.Log(test.bounds);

            //Physics2D.OverlapBox(test.bounds.center, test.bounds.size, 0);

            if(Physics2D.OverlapBox(test.bounds.center, test.bounds.size, 0) != null)
            {
                Debug.Log("TEST");
            }


            /**

            TilemapCollider2D test = t3.transform.Find("Walls").GetComponent<TilemapCollider2D>();

            Collider2D[] results = new Collider2D[1];
            ContactFilter2D contactFilter = new ContactFilter2D();

            if (test.OverlapCollider(contactFilter, results) != 0)
            {
                Debug.Log("TRY THE PHYSICS BOX YOU BITCH");
                Debug.Log("TEST");
            }
            else
            {
                Debug.Log("TEST2");
            }

            */

        }
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            //OG object, Vector3 position, Quaternion Rotation, Transform Parent

            GameObject test = Instantiate(prefabTests[0], this.transform.position, new Quaternion(0,0,0,1), this.transform);
            GameObject test2 = Instantiate(prefabTests[0], this.transform.position + new Vector3(1,1,0), new Quaternion(0, 0, 0, 1), this.transform);

            Tilemap t1 = test.transform.Find("Walls").GetComponent<Tilemap>();
            Tilemap t2 = test2.transform.Find("Walls").GetComponent<Tilemap>();

            Debug.Log(t1.cellBounds);
            Debug.Log(t2.cellBounds);


            if (!((t1.cellBounds.yMax < t2.cellBounds.yMin || t1.cellBounds.yMin > t2.cellBounds.yMax) ||
                (t1.cellBounds.xMax < t2.cellBounds.xMin || t1.cellBounds.xMin > t2.cellBounds.xMax)))
            {
                Debug.Log("TEST");
            }


            //if(testTilemap.cellBounds.xMin < testTilemap2.cellBounds.xMax && testTilemap.cellBounds.xMax > testTilemap2.cellBounds.xMin &&
            //   testTilemap.cellBounds.yMax > testTilemap2.cellBounds.yMin && testTilemap.cellBounds.yMax < testTilemap2.cellBounds.yMin)
            //{
            //    Debug.Log("TEST");
            //}

            //Debug.Log(testTilemap.cellBounds);
            //Debug.Log(testTilemap.cellBounds.max);
           // Debug.Log(testTilemap.cellBounds.min);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject test = Instantiate(prefabTests[1]);
            test.transform.parent = this.transform;

            Debug.Log(test.transform.rotation);
        }
    }
}
