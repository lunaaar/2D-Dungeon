using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest
{
    private const int move_straight = 10;
    private const int move_diagonal = 20;
    
    private Grid<PathNode> grid;

    private List<PathNode> openList;
    private List<PathNode> closedList;

    public PathfindingTest(int width, int height)
    {
        grid = new Grid<PathNode>(width, height, 1);
    }

    private List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);
        
        openList = new List<PathNode> {startNode};
        closedList = new List<PathNode>();

        for(int i = 0; i < grid.getWidth(); i++)
        {
            for(int j = 0; j < grid.getHeight(); j++)
            {
                PathNode pathNode = grid.GetValue(i, j);
                pathNode.gCost = int.MaxValue;
                pathNode.calculateFCost();
                pathNode.previousNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = calculateDistance(startNode, endNode);

        startNode.calculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = getLowestFCost(openList);

            if(currentNode == endNode)
            {
                return calculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);


        }

        return null;
    }

    private List<PathNode> getNeighbourList(PathNode curretNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        
        if(curretNode.x - 1 >= 0)
        {
            //neighbourList.Add()
        }

        return null;
    }

    private List<PathNode> calculatePath(PathNode endNode)
    {
        return null;
    }

    private int calculateDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return move_diagonal * Mathf.Min(xDistance, yDistance) + move_straight * remaining;
    }

    private PathNode getLowestFCost(List<PathNode> pathNodeList)
    {
        PathNode lowest = pathNodeList[0];
        for(int i = 0; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowest.fCost)
            {
                lowest = pathNodeList[i];
            }
        }

        return lowest;
    }
}
