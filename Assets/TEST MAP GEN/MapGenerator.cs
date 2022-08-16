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
        Hallway,
        Wall
    }

    [Header("Size Stuff")]
    public int numRooms;

    public Vector2Int sizeOfGrid;

    Grid2D<CellType> grid;
    GameObject[] hallwayInstances;
    GameObject[] wallInstances;
    [Header("Rooms")]
    public List<GameObject> roomPrefabs;

    [Header("Tilemap Stuff")]
    public GameObject hallwayPrefab;
    public GameObject roomPrefab;
    public GameObject testPrefab;
    public GameObject wallPrefab;
    [Space]
    public Tilemap wallTilemap;
    public RuleTile wallTile;
    [Space]
    public Tilemap floorTilemap;
    public RuleTile floorTile;

    public List<GameObject> rooms;

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
        grid = new Grid2D<CellType>(sizeOfGrid, Vector2Int.zero);
        hallwayInstances = new GameObject[grid.Size.x * grid.Size.y];
        wallInstances = new GameObject[grid.Size.x * grid.Size.y];

        placeRooms();
        delaunayTriangulation();
        minimumSpanningTree();
        pruneRooms();
        pathfindHallways();
        pruneRooms();
        cleanUp();
        pruneRooms();
        cleanUp();
        cleanUp();
        cleanUp();
        GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position;
    }


    private bool _placeRooms()
    {
        for (int i = 0; i < 2; i++)
        {
            var success = false;
            while (!success)
            {
                Vector3 center = new Vector3(Random.Range(10, sizeOfGrid.x - 10), Random.Range(10, sizeOfGrid.y - 10), 0);

                //Randomly pick a room from list of prefabs.
                GameObject room = Instantiate(roomPrefabs[Random.Range(2, roomPrefabs.Count)], center, new Quaternion(0, 0, 0, 1), this.transform);
                //Establish it as a room object with a random location

                BoxCollider2D boxCollider = room.GetComponent<BoxCollider2D>();

                if (Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0).Length > 2)
                {
                    Destroy(room);
                    success = false;
                }
                else
                {
                    rooms.Add(room);

                    Tilemap wallTilemap = room.transform.Find("Walls").GetComponent<Tilemap>();

                    foreach (Vector3Int point in wallTilemap.cellBounds.allPositionsWithin)
                    {
                        if (wallTilemap.HasTile(point))
                        {
                            Vector2Int pos = new Vector2Int((int)wallTilemap.CellToWorld(point).x, (int)wallTilemap.CellToWorld(point).y);
                            if (!grid.InBounds(pos))
                                return false;

                            grid[pos] = CellType.Room;
                            if (grid[pos] == CellType.Room)
                            {
                                //Instantiate(roomPrefab, new Vector3(pos.x, pos.y), new Quaternion(0, 0, 0, 1), this.transform);
                            }
                        }
                    }
                    success = true;
                }
            }
            
        }
        bool spawnSpawned = false;
        bool exitSpawned = false;
        while (!spawnSpawned || !exitSpawned)
        {
            if (!spawnSpawned)
            {
                Vector3 center = new Vector3(Random.Range(10, sizeOfGrid.x - 10), Random.Range(10, sizeOfGrid.y - 10), 0);

                //Randomly pick a room from list of prefabs.
                GameObject room = Instantiate(roomPrefabs[0], center, new Quaternion(0, 0, 0, 1), this.transform);
                //Establish it as a room object with a random location

                BoxCollider2D boxCollider = room.GetComponent<BoxCollider2D>();

                if (Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0).Length > 2)
                {
                    Destroy(room);
                }
                else
                {
                    spawnSpawned = true;
                    rooms.Add(room);

                    Tilemap wallTilemap = room.transform.Find("Walls").GetComponent<Tilemap>();
                    Tilemap floorTilemap = room.transform.Find("Floor").GetComponent<Tilemap>();

                    foreach (Vector3Int point in wallTilemap.cellBounds.allPositionsWithin)
                    {
                        if (wallTilemap.HasTile(point))
                        {
                            Vector2Int pos = new Vector2Int((int)wallTilemap.CellToWorld(point).x, (int)wallTilemap.CellToWorld(point).y);
                            if (!grid.InBounds(pos))
                                return false;

                            grid[pos] = CellType.Room;
                            if (grid[pos] == CellType.Room)
                            {
                                //Instantiate(roomPrefab, new Vector3(pos.x, pos.y), new Quaternion(0, 0, 0, 1), this.transform);
                            }
                        }
                    }

                    foreach (Vector3Int point in floorTilemap.cellBounds.allPositionsWithin)
                    {
                        if (floorTilemap.HasTile(point))
                        {
                            Vector2Int pos = new Vector2Int((int)floorTilemap.CellToWorld(point).x, (int)floorTilemap.CellToWorld(point).y);
                            if (!grid.InBounds(pos))
                                return false;

                            grid[pos] = CellType.Room;
                        }
                    }
                }
            }

            if (!exitSpawned)
            {
                Vector3 center = new Vector3(Random.Range(10, sizeOfGrid.x - 10), Random.Range(10, sizeOfGrid.y - 10), 0);

                //Randomly pick a room from list of prefabs.
                GameObject room = Instantiate(roomPrefabs[1], center, new Quaternion(0, 0, 0, 1), this.transform);
                //Establish it as a room object with a random location

                BoxCollider2D boxCollider = room.GetComponent<BoxCollider2D>();

                if (Physics2D.OverlapBoxAll(boxCollider.bounds.center, boxCollider.bounds.size, 0).Length > 2)
                {
                    Destroy(room);
                }
                else
                {
                    exitSpawned = true;
                    rooms.Add(room);

                    Tilemap wallTilemap = room.transform.Find("Walls").GetComponent<Tilemap>();

                    foreach (Vector3Int point in wallTilemap.cellBounds.allPositionsWithin)
                    {
                        if (wallTilemap.HasTile(point))
                        {
                            Vector2Int pos = new Vector2Int((int)wallTilemap.CellToWorld(point).x, (int)wallTilemap.CellToWorld(point).y);
                            if (!grid.InBounds(pos))
                                return false;

                            grid[pos] = CellType.Room;
                            if (grid[pos] == CellType.Room)
                            {
                                //Instantiate(roomPrefab, new Vector3(pos.x, pos.y), new Quaternion(0, 0, 0, 1), this.transform);
                            }
                        }
                    }
                }
            }

        }


        return true;
    }

    private void placeRooms()
    {

        while (!_placeRooms())
        {
            if (rooms != null)
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    Destroy(rooms[i].gameObject);
                }
                rooms.Clear();
            }
        };

    }

    private void delaunayTriangulation()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
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
        
        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<GameObject>).Item;
            var endRoom = (edge.V as Vertex<GameObject>).Item;

            //var startPosf = startRoom.GetComponent<BoxCollider2D>().bounds.center;
            var startPosf = startRoom.transform.Find("Entrance").position;

            //var endPosf = endRoom.GetComponent<BoxCollider2D>().bounds.center;
            var endPosf = endRoom.transform.Find("Entrance").position;

            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            //RIGHT HERE
            //Debug.Log(startPos + ":" + endPos);
            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) =>
            {
                var pathCost = new DungeonPathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (a.Previous != null)
                {
                    //Zig Zag Check
                    if ((b.Position.x - a.Previous.Position.x == 1 || b.Position.x - a.Previous.Position.x == -1)
                        && (b.Position.y - a.Previous.Position.y == 1 || b.Position.y - a.Previous.Position.y == -1))
                    {
                        pathCost.cost += 500;


                    }

                    //Optimal Space Check (Left and Right)

                    if (grid.InBounds(b.Position + Vector2Int.up * 4) && grid.InBounds(b.Position + Vector2Int.down * 4))
                    {
                        if (grid[b.Position + Vector2Int.up] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.up * 2)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + Vector2Int.down] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.down * 2)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.down * 3)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.up * 3)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.down * 4)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.up * 4)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }
                    }

                    //Optimal Space Check (Up and Down)
                    if (grid.InBounds(b.Position + Vector2Int.left * 4) && grid.InBounds(b.Position + Vector2Int.right * 4))
                    {
                        if (grid[b.Position + Vector2Int.left] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + Vector2Int.right] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.left * 2)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.right * 2)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.left * 3)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.right * 3)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.left * 4)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + (Vector2Int.right * 4)] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }
                    }

                }

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost += 1000;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost += 20;
                }
                else if (grid[b.Position] == CellType.Hallway)
                {
                    pathCost.cost += 0;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];

                    if (grid[current] == CellType.None && grid[current] != CellType.Room)
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
                        //Testing full paths
                        hallwayInstances[grid.GetIndex(pos)] = Instantiate(hallwayPrefab, (Vector3Int)pos, new Quaternion(0, 0, 0, 1), this.transform);
                        //Debug.Log($"Pos in Gen: { pos } { grid[pos] } { hallwayInstances[grid.GetIndex(pos)] }");
                        if (path.IndexOf(pos) < path.Count - 1 && path.IndexOf(pos) > 0)
                        {
                            //Instantiate(hallwayPrefab, (Vector3Int)pos, new Quaternion(0, 0, 0, 1), this.transform);
                            //floorTilemap.SetTile((Vector3Int)pos, floorTile);
                            //wallTilemap.SetTile((Vector3Int)pos, wallTile);

                            var previousPos = path[path.IndexOf(pos) - 1];
                            var nextPos = path[path.IndexOf(pos) + 1];

                            var directionForward = nextPos - pos;
                            var directionBackwards = pos - previousPos;

                            if (directionForward == Vector2Int.up || directionForward == Vector2Int.down)
                            {
                                if (directionBackwards == Vector2Int.right)
                                {
//Adds a wall to behind where the hallway starts
                                    //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.up)
                                    {
                                        if (grid[pos + Vector2Int.down] == CellType.None && wallInstances[grid.GetIndex(pos + Vector2Int.down)] == null)
                                        {
                                            grid[pos + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.right] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.right)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.right + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.right + Vector2Int.right] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.right + Vector2Int.right)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.right + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.right + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.left] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.left)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.left + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.left + Vector2Int.left] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.left + Vector2Int.left)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.left + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.left + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right + Vector2Int.down), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.down)
                                    {
                                        if (grid[pos + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.right + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.right + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.right + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.left] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.left + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.left + Vector2Int.left] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.left + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right + Vector2Int.up), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                if (directionBackwards == Vector2Int.left)
                                {
                                    //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.up)
                                    {
                                        if (grid[pos + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.right + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.right + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.right + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.right + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.left] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.down + Vector2Int.left + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.down + Vector2Int.left + Vector2Int.left] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.down + Vector2Int.left + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.left + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left + Vector2Int.down), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.down)
                                    {
                                        if (grid[pos + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.right + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.right + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.right + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.left] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.up + Vector2Int.left + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.up + Vector2Int.left + Vector2Int.left] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.up + Vector2Int.left + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left + Vector2Int.up), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                grid[pos + Vector2Int.left] = CellType.Hallway;
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.left)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.left)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.right] = CellType.Hallway;
                                
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.right)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.right)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.left * 2] = CellType.Hallway;
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.left * 2)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.left * 2)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.left * 2), new Quaternion(0, 0, 0, 1), this.transform);
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.right * 2] = CellType.Hallway;
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.right * 2)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.right * 2)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.right * 2), new Quaternion(0, 0, 0, 1), this.transform);
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);

//Handles the invisible walls. If its not a room tile already than it becomes a wall
                                if(grid[pos + Vector2Int.left * 3] == CellType.None)
                                {
                                    grid[pos + Vector2Int.left * 3] = CellType.Wall;
                                    wallInstances[grid.GetIndex(pos + Vector2Int.left * 3)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left * 3), new Quaternion(0, 0, 0, 1), this.transform);
                                }
                                if (grid[pos + Vector2Int.right * 3] == CellType.None)
                                {
                                    grid[pos + Vector2Int.right * 3] = CellType.Wall;
                                    wallInstances[grid.GetIndex(pos + Vector2Int.right * 3)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right * 3), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                            }

                            if (directionForward == Vector2Int.right || directionForward == Vector2Int.left)
                            {
                                if (directionBackwards == Vector2Int.up)
                                {
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.left)
                                    {
                                        if (grid[pos + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.up] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.up + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.up + Vector2Int.up] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.up + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.up + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.down] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.down + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.down + Vector2Int.down] = CellType.Wall;
                                            wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.down + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.down + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up + Vector2Int.right), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.right)
                                    {
                                        if (grid[pos + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.up] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.up + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.up + Vector2Int.up] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up + Vector2Int.up)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.up + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.down + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.down + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.down + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up + Vector2Int.left), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                if (directionBackwards == Vector2Int.down)
                                {
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.left)
                                    {
                                        if (grid[pos + Vector2Int.right] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.right)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.right)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.up] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.up)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.up + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.up + Vector2Int.up] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.up + Vector2Int.up)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.up + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.up + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.right + Vector2Int.down + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.right + Vector2Int.down + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.down + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.right + Vector2Int.down + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.down + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up + Vector2Int.right), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.right)
                                    {
                                        if (grid[pos + Vector2Int.left] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left)] == null) 
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.up] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.up + Vector2Int.up] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.up + Vector2Int.up] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up + Vector2Int.up)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.up + Vector2Int.up)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.up + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }

                                        if (grid[pos + Vector2Int.left + Vector2Int.down + Vector2Int.down] == CellType.None)
                                        {
                                            grid[pos + Vector2Int.left + Vector2Int.down + Vector2Int.down] = CellType.Wall;
                                            if (wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down + Vector2Int.down)] == null)
                                                wallInstances[grid.GetIndex(pos + Vector2Int.left + Vector2Int.down + Vector2Int.down)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.down + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                        }
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up + Vector2Int.left), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                

                                grid[pos + Vector2Int.up] = CellType.Hallway;
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.up)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.up)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.up * 2] = CellType.Hallway;
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.up * 2)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.up * 2)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.up * 2), new Quaternion(0, 0, 0, 1), this.transform);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up * 2), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up * 2), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.down] = CellType.Hallway;
                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.down)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.down)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.down * 2] = CellType.Hallway;

                                if (hallwayInstances[grid.GetIndex(pos + Vector2Int.down * 2)] == null)
                                    hallwayInstances[grid.GetIndex(pos + Vector2Int.down * 2)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.down * 2), new Quaternion(0, 0, 0, 1), this.transform);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down * 2), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.down * 2), new Quaternion(0, 0, 0, 1), this.transform);

//Handles the invisible walls. If its not a room tile already than it becomes a wall
                                if (grid[pos + Vector2Int.up * 3] == CellType.None)
                                {
                                    wallInstances[grid.GetIndex(pos + Vector2Int.up * 3)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.up * 3), new Quaternion(0, 0, 0, 1), this.transform);
                                }
                                if (grid[pos + Vector2Int.down * 3] == CellType.None)
                                {
                                    wallInstances[grid.GetIndex(pos + Vector2Int.down * 3)] = Instantiate(wallPrefab, (Vector3Int)(pos + Vector2Int.down * 3), new Quaternion(0, 0, 0, 1), this.transform);
                                }
                                
                            }
                        }

                        #region
                        //get new position in path with path.FindIndex(pos) + 1, handle end of list problem probably.

                        //somehow check the direction related to the two positions to note direction inteded for the path.
                        //place additional hallway tiles in perpindicular direction

                        /*if ((path.IndexOf(pos)) < path.Count - 1 && path.IndexOf(pos) > 0)
                        {
                            //Instantiate(hallwayPrefab, new Vector3(pos.x, pos.y), new Quaternion(0, 0, 0, 1), this.transform);

                            //floorTilemap.SetTile((Vector3Int)pos, floorTile);
                            wallTilemap.SetTile((Vector3Int)pos, wallTile);

                            var nextPos = path[path.IndexOf(pos) + 1];

                            var direction = nextPos - pos;

                            if(direction == Vector2Int.up)
                            {
                                if(grid[pos + Vector2Int.left] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                    //Instantiate(hallwayPrefab, new Vector3(pos.x + Vector2Int.left.x, pos.y + Vector2Int.left.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                                if(grid[pos + Vector2Int.right] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                    //Instantiate(hallwayPrefab, new Vector3(pos.x + Vector2Int.right.x, pos.y + Vector2Int.right.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                            }

                            if (direction == Vector2Int.down)
                            {
                                if (grid[pos + Vector2Int.left] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                    //Instantiate(hallwayPrefab, new Vector3(pos.x + Vector2Int.left.x, pos.y + Vector2Int.left.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                                if (grid[pos + Vector2Int.right] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                    //Instantiate(hallwayPrefab, new Vector3(pos.x + Vector2Int.right.x, pos.y + Vector2Int.right.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                            }

                            if (direction == Vector2Int.left)
                            {
                                if ((grid[pos + Vector2Int.up] == CellType.None) && grid[pos + (Vector2Int.up * 2)] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up), wallTile);
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up * 2), wallTile);
                                    //Instantiate(roomPrefab, new Vector3(pos.x + Vector2Int.up.x, pos.y + Vector2Int.up.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                                if (grid[pos + Vector2Int.down] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), wallTile);
                                    //Instantiate(roomPrefab, new Vector3(pos.x + Vector2Int.down.x, pos.y + Vector2Int.down.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                            }

                            if (direction == Vector2Int.right)
                            {
                                if (grid[pos + Vector2Int.up] == CellType.None && grid[pos + (Vector2Int.up * 2)] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up), wallTile);
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up * 2), wallTile);
                                    //Instantiate(roomPrefab, new Vector3(pos.x + Vector2Int.up.x, pos.y + Vector2Int.up.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                                if (grid[pos + Vector2Int.down] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), wallTile);
                                    //Instantiate(roomPrefab, new Vector3(pos.x + Vector2Int.down.x, pos.y + Vector2Int.down.y), new Quaternion(0, 0, 0, 1), this.transform);
                                }

                            }


                        }

                        /*if((path.IndexOf(pos) + 1) < path.Count)
                        {
                            var nextPos = path[path.IndexOf(pos) + 1];

                            var direction = nextPos - pos;

                            //If the hallways direction is left or right, and the next position is still part of the hallway
                            if((direction == Vector2.left || direction == Vector2.right) && grid[nextPos] == CellType.Hallway)
                            {
                                wallTilemap.SetTile((Vector3Int)pos, wallTile);
                                floorTilemap.SetTile((Vector3Int)pos, floorTile);
                                if(grid[pos + Vector2Int.up] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up), wallTile);

                                    if(grid[pos + Vector2Int.up + Vector2Int.up] == CellType.None){
                                        wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up), wallTile);
                                    }
                                }

                                if (grid[pos + Vector2Int.down] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), wallTile);
                                }

                                //if(grid[pos.x+1, pos.y] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x + 1, pos.y, 0), wallTile); floorTilemap.SetTile(new Vector3Int(pos.x + 1, pos.y, 0), floorTile);
                                //if (grid[pos.x - 1, pos.y] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x - 1, pos.y, 0), wallTile); floorTilemap.SetTile(new Vector3Int(pos.x - 1, pos.y, 0), floorTile);
                            }

                            if((direction == Vector2.up || direction == Vector2.down) && grid[nextPos] == CellType.Hallway)
                            {
                                wallTilemap.SetTile((Vector3Int)pos, wallTile);
                                floorTilemap.SetTile((Vector3Int)pos, floorTile);

                                if (grid[pos + Vector2Int.left] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                }

                                if (grid[pos + Vector2Int.right] == CellType.None)
                                {
                                    wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                }

                                //if(grid[pos.x, pos.y + 1] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y + 1, 0), wallTile);
                                //    if (grid[pos.x, pos.y + 2] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y + 2, 0), wallTile);
                                //if (grid[pos.x, pos.y - 1] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y - 1, 0), wallTile);
                            }
                        }
                        else
                        {
                            wallTilemap.SetTile((Vector3Int)pos, wallTile);
                            floorTilemap.SetTile((Vector3Int)pos, floorTile);
                        }


                        /*if((path.IndexOf(pos) + 1) < path.Count)
                        {
                            var nextPos = path[path.IndexOf(pos) + 1];

                            var direction = nextPos - pos;

                            if((direction == new Vector2Int(0, 1) || direction == new Vector2Int(0, -1)) && grid[nextPos] == CellType.Hallway)
                            {
                                wallTilemap.SetTile((Vector3Int)pos, wallTile);
                                floorTilemap.SetTile((Vector3Int)pos, floorTile);
                                if (grid[pos.x + 1, pos.y] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x + 1, pos.y, 0), wallTile); floorTilemap.SetTile(new Vector3Int(pos.x + 1, pos.y, 0), floorTile);
                                if (grid[pos.x - 1, pos.y] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x - 1, pos.y, 0), wallTile); floorTilemap.SetTile(new Vector3Int(pos.x - 1, pos.y, 0), floorTile);
                            }

                            if((direction == new Vector2Int(1,0) || direction == new Vector2Int(-1, 0)) && grid[nextPos] == CellType.Hallway)
                            {
                                wallTilemap.SetTile((Vector3Int)pos, wallTile);
                                floorTilemap.SetTile((Vector3Int)pos, floorTile);
                                if(grid[pos.x, pos.y + 1] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y + 1, 0), wallTile);
                                if (grid[pos.x, pos.y + 2] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y + 2, 0), wallTile);
                                if (grid[pos.x, pos.y - 1] != CellType.Room) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y - 1, 0), wallTile);
                            }
                        }
                        else
                        {
                            //wallTilemap.SetTile((Vector3Int)pos, wallTile);
                            //floorTilemap.SetTile((Vector3Int)pos, floorTile);
                        }*/

                        //PlaceHallway(pos);
                        //if(grid[pos])
                        #endregion
                    }
                }
            }
        }
    }
    private void placeHallwayTile(Vector2Int position, Tilemap tilemap, RuleTile tile)
    {
        if (grid[position] != CellType.Room)
        {
            tilemap.SetTile((Vector3Int)position, tile);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (delaunay != null)
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


    public bool pruneRooms()
    {
        var roomsInGraph = new HashSet<GameObject>();
        foreach (var edge in selectedEdges)
        {
            var uRoom = (edge.U as Vertex<GameObject>).Item;
            var vRoom = (edge.V as Vertex<GameObject>).Item;
            roomsInGraph.Add(uRoom);
            roomsInGraph.Add(vRoom);
        }

        var roomsInScene = GameObject.FindGameObjectsWithTag("Room");
        foreach (var room in roomsInScene)
        {
            if (!roomsInGraph.Contains(room))
            {
                Object.Destroy(room);
                if (room.name.Contains("Entrance") || room.name.Contains("Exit"))
                {
                    return true;
                }
                
            }
        }
        return false;
    }

    public void generateDungeon()
    {

        DestroyEverything();
        StartCoroutine(DeferRegen());
        
    }

    public void DestroyEverything()
    {
        if (rooms != null)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Destroy(rooms[i].gameObject);
            }
            rooms.Clear();
        }
        for (int i = 0; i < wallInstances.Length; i++)
        {
            Destroy(wallInstances[i]);
            wallInstances[i] = null;
        }
        foreach (var i in GameObject.FindGameObjectsWithTag("Wall"))
        {
            Destroy(i);
        }

        grid = new Grid2D<CellType>(sizeOfGrid, Vector2Int.zero);
        hallwayInstances = new GameObject[grid.Size.x * grid.Size.y];
        wallInstances = new GameObject[grid.Size.x * grid.Size.y];
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        var objects = GameObject.FindGameObjectsWithTag("HallwayBit");
        //Debug.Log(objects.Length);
        foreach (var o in objects)
        {
            Object.Destroy(o);
        }
    }

    IEnumerator DeferRegen()
    {
        yield return new WaitForEndOfFrame();

        var invalid = true;
        while (invalid)
        {
            placeRooms();
            delaunayTriangulation();
            minimumSpanningTree();
            invalid = pruneRooms();
            pathfindHallways();
            cleanUp();
            cleanUp();
            cleanUp();
            cleanUp();

            if (invalid)
            {
                DestroyEverything();
                yield return new WaitForEndOfFrame();
            }
        }
        

        yield return new WaitForEndOfFrame();
        GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("PlayerSpawn").transform.position;
    }

    private bool SquareBottomRight(Vector2Int buttomRightPos)
    {
        return grid[buttomRightPos + Vector2Int.left] == CellType.Hallway
            && grid[buttomRightPos + Vector2Int.up] == CellType.Hallway
            && grid[buttomRightPos + Vector2Int.up + Vector2Int.left] == CellType.Hallway
            ;
    }

    private bool SquareBottomLeft(Vector2Int bottomLeftPos)
    {
        return grid[bottomLeftPos + Vector2Int.up] == CellType.Hallway
            && grid[bottomLeftPos + Vector2Int.right] == CellType.Hallway
            && grid[bottomLeftPos + Vector2Int.right + Vector2Int.up] == CellType.Hallway;
    }

    private bool SquareTopLeft(Vector2Int topLeftPos)
    {
        return grid[topLeftPos + Vector2Int.right] == CellType.Hallway
            && grid[topLeftPos + Vector2Int.down] == CellType.Hallway
            && grid[topLeftPos + Vector2Int.down + Vector2Int.right] == CellType.Hallway;
    }

    private bool SquareTopRight(Vector2Int topRightPos)
    {
        return grid[topRightPos + Vector2Int.down] == CellType.Hallway
            && grid[topRightPos + Vector2Int.left] == CellType.Hallway
            && grid[topRightPos + Vector2Int.down + Vector2Int.left] == CellType.Hallway;
    }

    private bool IsGapped(CellType a, CellType b)
    {
        return (a == CellType.None && b == CellType.Room) || (a == CellType.Room && b == CellType.None);

    }
    private bool NoGap(Vector2Int cell, Vector2Int dir)
    {
        return !IsGapped(grid[cell + dir], grid[cell + dir + dir]);
    }

    //                                hallwayInstances[grid.GetIndex(pos + Vector2Int.down * 2)] = Instantiate(hallwayPrefab, (Vector3Int)(pos + Vector2Int.down * 2), new Quaternion(0, 0, 0, 1), this.transform);


    public void cleanUp()
    {
    
        for (int i = 0; i < wallInstances.Length; i++)
        {
            if (wallInstances[i] != null)
            {
                if (hallwayInstances[i] != null)
                {
                    Destroy(wallInstances[i].gameObject);
                    Destroy(wallInstances[i]);
                    hallwayInstances[i] = Instantiate(hallwayPrefab, hallwayInstances[i].transform.position , new Quaternion(0, 0, 0, 1), this.transform);
                }
                if(hallwayInstances[grid.GetIndex(new Vector2Int((int) wallInstances[i].transform.position.x + 1, (int) wallInstances[i].transform.position.y))] != null && hallwayInstances[grid.GetIndex(new Vector2Int((int)wallInstances[i].transform.position.x - 1, (int)wallInstances[i].transform.position.y))] != null)
                {
                    Destroy(wallInstances[i].gameObject);
                    Destroy(wallInstances[i]);
                    hallwayInstances[i] = Instantiate(hallwayPrefab, wallInstances[i].transform.position, new Quaternion(0, 0, 0, 1), this.transform);
                }
                if (hallwayInstances[grid.GetIndex(new Vector2Int((int)wallInstances[i].transform.position.x , (int)wallInstances[i].transform.position.y + 1))] != null && hallwayInstances[grid.GetIndex(new Vector2Int((int)wallInstances[i].transform.position.x, (int)wallInstances[i].transform.position.y - 1))] != null)
                {
                    Destroy(wallInstances[i].gameObject);
                    Destroy(wallInstances[i]);
                    hallwayInstances[i] = Instantiate(hallwayPrefab, wallInstances[i].transform.position, new Quaternion(0, 0, 0, 1), this.transform);
                }
            }
        }
    }

}

