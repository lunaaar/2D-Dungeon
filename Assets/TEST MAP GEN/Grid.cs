using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid<T>
{
    private int width;
    private int height;
    private float cellSize;

    private T[,] gridArray;

    public Grid(int w, int h, float c)
    {
        width = w;
        height = h;
        cellSize = c;

        gridArray = new T[width, height];
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    private Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(x, y) * cellSize;
    }

    private void GetXY(Vector2 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
        }
    }

    public void SetValue(Vector2 worldPosition, T value)
    {
        int x;
        int y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public T GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public T GetValue(Vector2 worldPosition)
    {
        int x;
        int y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }


}
