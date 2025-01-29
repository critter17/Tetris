using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBlinkingEvents : MonoBehaviour
{
    private TetrisGrid tetrisGrid;

    private void Awake()
    {
        tetrisGrid = FindObjectOfType<TetrisGrid>();
    }

    public void PrintEvent(string s)
    {
        //Debug.Log("PrintEvent: " + s + "called at: " + Time.time);
    }

    public void DeleteBlock()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);

        //Debug.Log("Deleting block at (" + x + ", " + y + ")");
        tetrisGrid.RemoveBlock(x, y);
        Destroy(gameObject);
        //Debug.Log("Block destroyed");
    }
}
