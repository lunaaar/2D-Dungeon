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

    [Header("Size Stuff")]
    public int numRooms;

    public Vector2Int sizeOfGrid;

    Grid2D<CellType> grid;

    [Header("Rooms")]
    public List<GameObject> roomPrefabs;

    [Header("Tilemap Stuff")]
    public GameObject hallwayPrefab;
    public GameObject roomPrefab;
    public GameObject testPrefab;
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

        placeRooms();
        delaunayTriangulation();
        minimumSpanningTree();
        pathfindHallways();
    }


    private bool _placeRooms()
    {
        for (int i = 0; i < numRooms; i++)
        {
            Vector3 center = new Vector3(Random.Range(10, sizeOfGrid.x - 10), Random.Range(10, sizeOfGrid.y - 10), 0);

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
        return true;
    }

    private void placeRooms()
    {

        while (!_placeRooms()) ;
        
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

            //var startPosf = startRoom.GetComponent<BoxCollider2D>().bounds.center;
            var startPosf = startRoom.transform.Find("Entrance").position;

            //var endPosf = endRoom.GetComponent<BoxCollider2D>().bounds.center;
            var endPosf = endRoom.transform.Find("Entrance").position;

            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);
            
            //RIGHT HERE
            //Debug.Log(startPos + ":" + endPos);
            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if(a.Previous != null)
                {
                    //Zig Zag Check
                    if ((b.Position.x - a.Previous.Position.x == 1 || b.Position.x - a.Previous.Position.x == -1)
                        && (b.Position.y - a.Previous.Position.y == 1 || b.Position.y - a.Previous.Position.y == -1)){
                        pathCost.cost += 500;


                    }

                    //Optimal Space Check (Left and Right)

                    if (grid.InBounds(b.Position + Vector2Int.up * 2) && grid.InBounds(b.Position + Vector2Int.down * 2))
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
                    }

                    //Optimal Space Check (Up and Down)
                    if (grid.InBounds(b.Position + Vector2Int.left) && grid.InBounds(b.Position + Vector2Int.right))
                    {
                        if (grid[b.Position + Vector2Int.left] == CellType.Room)
                        {
                            pathCost.cost += 1000;
                        }

                        if (grid[b.Position + Vector2Int.right] == CellType.Room)
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
                        Instantiate(hallwayPrefab, (Vector3Int)pos, new Quaternion(0, 0, 0, 1), this.transform);

                        if (path.IndexOf(pos) < path.Count - 1 && path.IndexOf(pos) > 0)
                        {
                            //Instantiate(hallwayPrefab, (Vector3Int)pos, new Quaternion(0, 0, 0, 1), this.transform);
                            //floorTilemap.SetTile((Vector3Int)pos, floorTile);
                            //wallTilemap.SetTile((Vector3Int)pos, wallTile);

                            var previousPos = path[path.IndexOf(pos) - 1];
                            var nextPos = path[path.IndexOf(pos) + 1];

                            var directionForward = nextPos - pos;
                            var directionBackwards = pos - previousPos;

                            if(directionForward == Vector2Int.up || directionForward == Vector2Int.down)
                            {
                                if (directionBackwards == Vector2Int.right)
                                {
                                    //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.up) {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right + Vector2Int.down), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.down)
                                    {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right + Vector2Int.up), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                if (directionBackwards == Vector2Int.left)
                                {
                                    //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.up) {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left + Vector2Int.down), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.down)
                                    {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left + Vector2Int.up), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                grid[pos + Vector2Int.left] = CellType.Hallway;
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.left), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.right] = CellType.Hallway;
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.right), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                            }

                            if (directionForward == Vector2Int.right || directionForward == Vector2Int.left)
                            {
                                if (directionBackwards == Vector2Int.up)
                                {
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.left) {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up + Vector2Int.right), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.right)
                                    {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up + Vector2Int.left), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                if (directionBackwards == Vector2Int.down)
                                {
                                    //Instantiate(testPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);
                                    if (directionForward == Vector2Int.left) {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down + Vector2Int.right), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.right), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                    if (directionForward == Vector2Int.right)
                                    {
                                        //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down + Vector2Int.left), wallTile);
                                        //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.down + Vector2Int.left), new Quaternion(0, 0, 0, 1), this.transform);
                                    }
                                }

                                grid[pos + Vector2Int.up] = CellType.Hallway;
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.up * 2] = CellType.Hallway;
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.up * 2), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.up * 2), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.down] = CellType.Hallway;
                                //floorTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), floorTile);
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.down), new Quaternion(0, 0, 0, 1), this.transform);

                                grid[pos + Vector2Int.down * 2] = CellType.Hallway;
                                //wallTilemap.SetTile((Vector3Int)(pos + Vector2Int.down * 2), wallTile);
                                //Instantiate(roomPrefab, (Vector3Int)(pos + Vector2Int.down * 2), new Quaternion(0, 0, 0, 1), this.transform);
                            }
                        }


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


    public void pruneRooms()
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
            }
        }
    }

    public void generateDungeon()
    {
        if (rooms != null)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                Destroy(rooms[i].gameObject);
            }
            rooms.Clear();
        }
        grid = new Grid2D<CellType>(sizeOfGrid, Vector2Int.zero);
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        var objects = GameObject.FindGameObjectsWithTag("HallwayBit");
        Debug.Log(objects.Length);
        foreach (var o in objects)
        {
            Object.Destroy(o);
        }

        placeRooms();
        delaunayTriangulation();
        minimumSpanningTree();
        pruneRooms();
        pathfindHallways();
    }
}

