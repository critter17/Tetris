using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] tetrisBlocks;

    public TetrisBlock dropBlock()
    {
        GameObject tetromino = Instantiate(tetrisBlocks[Random.Range(0, tetrisBlocks.Length)], transform.position, Quaternion.identity);
        tetromino.transform.SetParent(GameObject.FindWithTag("Next").transform);
        return tetromino.GetComponent<TetrisBlock>();
    }
}
