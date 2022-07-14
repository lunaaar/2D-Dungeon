using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class MapGenerator : MonoBehaviour
{
    enum Type {
        Nothing,
        Room,
        Hallway
    }


    class Room
    {
        public BoundsInt bounds;

        public Room(BoundsInt b)
        {
            bounds = b;
        }
    }


    public int numRooms;
    //public Grid grid;

    public Vector2Int sizeOfGrid;

    public List<GameObject> roomPrefabs;

    List<GameObject> rooms;

    public ContactFilter2D contactFilter;

    // THIS GENERATION IS WIP and is based on a form of the Tiny Keep Generation Method

    /** STEPS TO DO
     * 1. Place Rooms Randomly such that they do not intersect
     * 2. Create Delaunay Triangulation (possibly use Bowyer-Watson Algorithm)
     * 3. Find Minimum Spanning Tree (MST) (use Prim's Algorithm)
     * 4. Randomly Choose Edges
     * 5. Pathfind Hallways (use A* algorithm on every hallway)
     */

    // Start is called before the first frame update
    void Start()
    {
        //grid = new Grid();

        rooms = new List<GameObject>();

        
        placeRooms();
        delaunayTriangulation();
        minimumSpanningTree();
        pathfindHallways();
    }

    private void placeRooms()
    {
        bool goodRoom = true;

        for (int i = 0; i < numRooms; i++)
        {

            //Randomly pick a room from list of prefabs.
            GameObject room = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
            //Establish it as a room object with a random location

            goodRoom = true;

            Vector3 center = new Vector3(Random.Range(-sizeOfGrid.x, sizeOfGrid.x), Random.Range(-sizeOfGrid.y, sizeOfGrid.y), 0);

            

            Collider2D[] results = new Collider2D[10];
            
            

            foreach (GameObject r in rooms)
            {
                Debug.Log("TEST");
                TilemapCollider2D t = r.transform.Find("Walls").GetComponent<TilemapCollider2D>();

                Debug.Log(t.OverlapCollider(contactFilter, results));

                if(t.OverlapCollider(contactFilter, results) != 0)
                {

                    Debug.Log("TRY THE PHYSICS BOX YOU BITCH");
                    
                    Debug.Log("collide");

                    goodRoom = false;
                    break;
                }
            }


            if (goodRoom)
            {
                rooms.Add(room);
                Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], center, new Quaternion(0, 0, 0, 1), this.transform);
            }

            

        }
    }

    private void delaunayTriangulation()
    {

    }
    private void minimumSpanningTree()
    {

    }
    private void pathfindHallways()
    {

    }
}

