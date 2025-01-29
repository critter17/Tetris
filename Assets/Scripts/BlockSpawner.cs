using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] tetrisBlocks;
    [SerializeField] List<TetrisBlock> nextBlocksPreviews;

    public TetrisBlock dropBlock()
    {
        GameObject tetromino = Instantiate(tetrisBlocks[Random.Range(0, tetrisBlocks.Length)], transform.position, Quaternion.identity);
        return tetromino.GetComponent<TetrisBlock>();
    }
}
