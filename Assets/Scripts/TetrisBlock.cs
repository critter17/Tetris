using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    // Static Variables
    private static float moveDist = 1.0f;

    // Object References
    [SerializeField] private GameObject dropShadowBlocks;
    [SerializeField] private GameObject blocks;

    // Design adjusters
    [SerializeField] private Vector3 rotationPoint;
    [SerializeField] private float fallTime = 1.0f;
    private float dirTime = 0.15f;

    // Audio Source
    private AudioSource audioSource;

    #region Properties
    public GameObject DropShadowBlocks
    {
        get => dropShadowBlocks;
    }

    public GameObject Blocks
    {
        get => blocks;
    }

    public float DirSpeed
    {
        get => dirTime;
        set => dirTime = value;
    }
    #endregion

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (blocks.transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }

    public void MoveLeft()
    {
        transform.position += new Vector3(-moveDist, 0, 0);
    }

    public void MoveRight()
    {
        transform.position += new Vector3(moveDist, 0, 0);
    }

    public void MoveDown()
    {
        transform.position += new Vector3(0, -moveDist, 0);
    }

    public void MoveUp()
    {
        transform.position += new Vector3(0, moveDist, 0);
    }

    public void RotateLeft()
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
    }

    public void RotateRight()
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
    }


}
