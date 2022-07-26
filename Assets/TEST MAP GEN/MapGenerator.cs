using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Graphs;


public class MapGenerator : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        Hallway
    }

    public int numRooms;

    public Vector2Int sizeOfGrid;

    Grid2D<CellType> grid;

    public List<GameObject> roomPrefabs;

    public GameObject hallwayPrefab;

    List<GameObject> rooms;

    DelaunayTriangulation delaunay;

    HashSet<Prim.Edge> selectedEdges;

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
        rooms = new List<GameObject>();

        placeRooms();
        delaunayTriangulation();
        minimumSpanningTree();
        pathfindHallways();
    }

    private void placeRooms()
    {
        //ADD IN HERE TO PLACE THE STARTING ROOM AND BOSS ROOM SEPARATE
        
        
        for (int i = 0; i < numRooms; i++)
        {
            Vector3 center = new Vector3(Random.Range(-sizeOfGrid.x, sizeOfGrid.x), Random.Range(-sizeOfGrid.y, sizeOfGrid.y), 0);

            //Randomly pick a room from list of prefabs.
            GameObject room = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], center, new Quaternion(0, 0, 0, 1), this.transform);
            //Establish it as a room object with a random location

            BoxCollider2D boxCollider = room.GetComponent<BoxCollider2D>();

            if (Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0).Length > 2)
            {
                Destroy(room);
                i--;
            }
            else
            {
                rooms.Add(room);

                //foreach(var pos in room.GetComponent<BoxCollider2D>().bounds.)
            } 
        }

    }

    private void delaunayTriangulation()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach(var room in rooms)
        {
            vertices.Add(new Vertex<GameObject>((Vector2)room.GetComponent<BoxCollider2D>().bounds.center, room));
        }

        delaunay = DelaunayTriangulation.Triangulate(vertices);

    }
    private void minimumSpanningTree()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (Random.value < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }
    void pathfindHallways()
    {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(sizeOfGrid);

        foreach(var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<GameObject>).Item;
            var endRoom = (edge.V as Vertex<GameObject>).Item;

            var startPosf = startRoom.GetComponent<BoxCollider2D>().bounds.center;
            var endPosf = endRoom.GetComponent<BoxCollider2D>().bounds.center;
            
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);
            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost += 10;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost += 5;
                }
                else if (grid[b.Position] == CellType.Hallway)
                {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0)
                    {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

                foreach (var pos in path)
                {
                    if (grid[pos] == CellType.Hallway)
                    {
                        //PlaceHallway(pos);

                        Instantiate(hallwayPrefab, new Vector3(pos.x, pos.y), new Quaternion(0, 0, 0, 1), this.transform);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if(delaunay != null)
        {
            foreach (DelaunayTriangulation.Edge v in delaunay.Edges)
            {
                Gizmos.DrawLine(v.U.Position, v.V.Position);
            }

        }

        Gizmos.color = Color.blue;

        if (selectedEdges != null)
        {
            foreach (Prim.Edge v in selectedEdges)
            {
                Gizmos.DrawLine(v.U.Position, v.V.Position);
            }

        }

    }
}

