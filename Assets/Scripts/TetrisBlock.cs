﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisBlock : MonoBehaviour
{
    // Static Variables
    private static float moveDist = 1.0f;

    // Design adjusters
    [SerializeField] private Vector3 rotationPoint;
    [SerializeField] private float fallTime = 1.0f;
    [SerializeField] private float previousTime;

    public float Speed
    {
        get => fallTime;
        set => fallTime = value;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount == 0)
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
