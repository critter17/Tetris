using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugGrid : MonoBehaviour
{
    [SerializeField] private GameObject blockCellPrefab;
    private List<Text> blockCells;
    private TetrisGrid currentGrid;

    private void Awake()
    {
        blockCells = new List<Text>();
        currentGrid = FindObjectOfType<TetrisGrid>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int column = 0; column < currentGrid.GridWidth; column++)
        {
            for (int row = 0; row < currentGrid.GridHeight; row++)
            {
                Debug.Log("Setting personas");
                GameObject blockCell = Instantiate(blockCellPrefab, transform);
                Debug.Log(blockCell);
                blockCells.Add(blockCell.GetComponent<Text>());
            }
        }
    }

    public void SetBlockCellText(int column, int row, string blockString)
    {
        blockCells[column + (row * 10)].text = blockString;
    }
}
