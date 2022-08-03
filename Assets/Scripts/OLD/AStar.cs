using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{
    private static readonly Vector3Int[] neighbors =
    {
        Vector3Int.up,
        Vector3Int.left,
        Vector3Int.right,
        Vector3Int.down,
    };
    public Tilemap solids;
    public Transform playerTransform;
    class Node : IComparable<Node>
    {
        public bool IsWall;
        public Vector3Int Position;
        public float F;

        public Node(Vector3Int Position, bool IsWall = false)
        {
            this.IsWall = IsWall;
            this.Position = Position;
        }

        public int CompareTo(Node other)
        {
            return F.CompareTo(other.F);
        }

        override public string ToString()
        {
            return $"Node[ Position: { Position }, F: { F } ]";
        }
    }

    float d(Node from, Node too)
    {
        var dx = Mathf.Abs(from.Position.x - too.Position.x);
        var dy = Mathf.Abs(from.Position.y - too.Position.y);
        return dx + dy;
    }

    int MAX_ITERATIONS = 100_000;
    public List<Vector3> ComputePath(Vector3 startPosition, Vector3 endPosition)
    {
        // Initialize the A* algorithm:
        var goal = new Node(solids.WorldToCell(endPosition));
        var start = new Node(solids.WorldToCell(startPosition));
        Debug.Log($"{goal} {start}");
        start.F = d(start, goal);

        var openSet = new SortedSet<Node> { start };
        var cameFrom = new Dictionary<Node, Node>();

        var gScore = new Dictionary<Node, float>
        {
            [start] = 0f
        };

        var fScore = new Dictionary<Node, float>
        {
            [start] = d(start, goal)
        };


        var iterations = 0;
        while (openSet.Count > 0)
        {
            iterations += 1;
            if (iterations > MAX_ITERATIONS)
            {
                Debug.Log("Search took too long!");
                break;
            }
                
            var current = openSet.Min;
            // We've reached the goal
            if (current.Position == goal.Position)
            {
                // Build the path starting from the goal back to the start
                var path = new List<Vector3> { solids.CellToWorld(current.Position) };
                foreach (var node in cameFrom.Keys)
                {
                    current = cameFrom[node];
                    path.Add(solids.CellToWorld(current.Position));
                }

                // Reverse for convience.
                path.Reverse();
                return path;
            }

            openSet.Remove(current);
            Debug.Log(current);

            // for each neighbor of current
            foreach (var cell in neighbors)
            {
                var pos = cell + current.Position;

                // Skip Solid tiles
                if (solids.HasTile(pos))
                    continue;

                var neighbor = new Node(pos);

                // Computer Score
                var gScoreTentative = gScore[current] + d(current, neighbor);

                // Neighbor is unexplored OR Going from Current -> Neighbor is shorter than any other route
                if (!gScore.ContainsKey(neighbor) || gScoreTentative < gScore[neighbor])
                {
                    // computer the F score
                    neighbor.F = gScoreTentative + d(neighbor, goal);
                    
                    // Track neighbor
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = gScoreTentative;
                    fScore[neighbor] = neighbor.F;

                    // Add it to the open set if it isn't already in the set.
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        Debug.Log("No Path Found!");
        return new List<Vector3>();
    }

    List<Vector3> currentPath = new List<Vector3>();
    Color[] colors = { Color.white, Color.red };
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPath = ComputePath(worldPoint, playerTransform.position);
            Debug.Log(currentPath.Count);
        }
        
        for (int i = 1; i < currentPath.Count; i++)
        {
            Debug.DrawLine(currentPath[i - 1], currentPath[i], colors[i % 2]);
        }
    }
}
