using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    // Static Variables
    private static int gridLeeway = 5;
    private static int gridHeight = 20;
    private static int gridWidth = 10;
    private Transform[,] grid = new Transform[gridWidth, gridHeight + gridLeeway];
    DebugGrid debugGrid;

    public int GridHeight
    {
        get => gridHeight;
    }

    public int GridWidth => gridWidth;

    public Transform[,] Grid
    {
        get => grid;
    }

    private void Awake()
    {
        debugGrid = FindObjectOfType<DebugGrid>();
    }

    private void SetDebugGridCell(int column, int row, string blockString)
    {
        if (!Debug.isDebugBuild)
        {
            return;
        }

        debugGrid.SetBlockCellText(column, row, blockString);
    }

    public List<Transform> CheckForLines()
    {
        List<Transform> clearedBlocks = new List<Transform>();

        for (int i = gridHeight - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                for (int column = 0; column < gridWidth; column++)
                {
                    clearedBlocks.Add(grid[column, i]);
                }
            }
        }

        return clearedBlocks;
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
        grid[column, row] = null;
        SetDebugGridCell(column, row, "X");
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

    public List<Transform> GetBlocksAboveRow(int row)
    {
        List<Transform> hoveringBlocks = new List<Transform>();

        for (int i = 0; i < gridWidth; i++)
        {
            for (int startingRow = row + 1; startingRow < gridHeight; startingRow++)
            {
                if (grid[i, startingRow])
                {
                    hoveringBlocks.Add(grid[i, startingRow]);
                }
            }
        }

        return hoveringBlocks;
    }

    public void MoveBlockDown(int column, int row)
    {
        //Debug.LogFormat("Moving block from ({0}, {1}) to ({2}, {3})", column, row, column, row - 1);
        if (!IsBlockGrounded(column, row))
        {
            grid[column, row - 1] = grid[column, row];
            grid[column, row - 1].position -= new Vector3(0, 1, 0);
            grid[column, row] = null;
            SetDebugGridCell(column, row - 1, "O");
            SetDebugGridCell(column, row, "X");
        }
    }

    public bool AddToGrid(Transform blockTransform)
    {
        Debug.Log("Adding blocks to grid...");

        foreach (Transform block in blockTransform)
        {
            int column = Mathf.RoundToInt(block.transform.position.x);
            int row = Mathf.RoundToInt(block.transform.position.y);

            Debug.Log("Column: " + column);
            Debug.Log("Row: " + row);

            if (row >= gridHeight)
            {
                return false;
            }

            grid[column, row] = block;
            SetDebugGridCell(column, row, "O");
        }

        return true;
    }

    public bool TryMove(Transform tetromino)
    {
        Debug.Log("Try move start...");
        foreach (Transform block in tetromino)
        {
            int column = Mathf.RoundToInt(block.transform.position.x);
            int row = Mathf.RoundToInt(block.transform.position.y);

            Debug.LogFormat("Piece at ({0}, {1})", column, row);

            if (column < 0 || column >= gridWidth || row < 0 || row >= (gridHeight + gridLeeway))
            {
                return false;
            }

            if (grid[column, row] != null)
            {
                return false;
            }
        }

        Debug.Log("Try move end...");

        return true;
    }

    public bool IsBlockGrounded(int column, int row)
    {
        //Debug.Log("Checking if block at (" + column + ", " + row + ") is valid");

        return row <= 0 || grid[column, row - 1] != null;
    }

    public int GetLowestRowCleared(List<Transform> blocksToClear)
    {
        int minRow = 19;

        for (int i = 0; i < blocksToClear.Count; i++)
        {
            int posX = Mathf.RoundToInt(blocksToClear[i].position.x);
            int posY = Mathf.RoundToInt(blocksToClear[i].position.y);

            //Debug.Log("GetLowestRowCleared: (" + posX + ", " + posY + ")");

            if (blocksToClear[i])
            {
                if (posY < minRow)
                {
                    minRow = (int)blocksToClear[i].position.y;
                }
            }
        }

        return minRow;
    }
}
