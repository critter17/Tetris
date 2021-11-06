using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    // Static Variables
    private static int gridHeight = 20;
    private static int gridWidth = 10;
    private static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public bool CheckForLines()
    {
        bool lineFound = false;

        for (int i = gridHeight - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    Animation animation = grid[column, i].gameObject.GetComponent<Animation>();
                    animation.Play();

                    RemoveBlock(column, i);
                    MoveLines(column, i + 1);
                }

                lineFound = true;
            }
        }

        if (lineFound)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool HasLine(int i)
    {
        for (int j = 0; j < gridWidth; j++)
        {
            if (grid[j, i] == null)
            {
                return false;
            }
        }

        return true;
    }

    public void RemoveBlock(int column, int row)
    {
        Destroy(grid[column, row].gameObject);
        grid[column, row] = null;
    }

    public void MoveLines(int column, int row)
    {
        for (int y = row; y < gridHeight - 1; y++)
        {
            for (int x = y; x >= 1; x--)
            {
                if (grid[column, x - 1] == null && grid[column, x])
                {
                    grid[column, x - 1] = grid[column, x];
                    grid[column, x - 1].transform.position -= new Vector3(0, 1, 0);
                    grid[column, x] = null;
                }
            }
        }
    }

    public void AddToGrid(Transform blockTransform)
    {
        foreach (Transform block in blockTransform)
        {
            int column = Mathf.RoundToInt(block.transform.position.x);
            int row = Mathf.RoundToInt(block.transform.position.y);

            grid[column, row] = block;
        }
    }

    public bool TryMove(Transform tetromino)
    {
        foreach (Transform block in tetromino)
        {
            int column = Mathf.RoundToInt(block.transform.position.x);
            int row = Mathf.RoundToInt(block.transform.position.y);

            if (column < 0 || column >= gridWidth || row < 0 || row >= gridHeight)
            {
                return false;
            }

            if (grid[column, row] != null)
            {
                return false;
            }
        }

        return true;
    }
}
