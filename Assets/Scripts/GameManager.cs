using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Serialized Variables
    [SerializeField] private TetrisGrid tetrisGrid;
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private GameObject tetrominoContainer;
    [SerializeField] private GameObject holdContainer;
    
    private float previousTime;
    private TetrisBlock currentTetromino;
    private bool canHoldTetromino;
    private TetrisBlock currentlyHeldTetromino;
    private bool clearLinesMode;

    // Audio Source
    private AudioSource audioSource;

    // Audio Clips
    [SerializeField] private AudioClip fallSFX;
    [SerializeField] private AudioClip touchSFX;
    [SerializeField] private AudioClip touchDownSFX;

    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("GameManager");
                    _instance = container.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }
    #endregion

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentlyHeldTetromino = null;
        clearLinesMode = false;
        DropTetromino();
    }

    // Update is called once per frame
    void Update()
    {
        Transform currentTransform = currentTetromino.gameObject.transform;

        if (!clearLinesMode)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currentTetromino.MoveLeft();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.MoveRight();
                    audioSource.clip = touchSFX;
                    audioSource.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currentTetromino.MoveRight();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.MoveLeft();
                    audioSource.clip = touchSFX;
                    audioSource.Play();
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentTetromino.Speed = 0.10f;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                currentTetromino.Speed = 1.0f;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                currentTetromino.RotateRight();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.RotateLeft();
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                currentTetromino.RotateLeft();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.RotateRight();
                }
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                HoldTetromino();
            }

            if (Time.time - previousTime > currentTetromino.Speed)
            {
                currentTetromino.MoveDown();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.MoveUp();
                    audioSource.clip = touchDownSFX;
                    audioSource.Play();
                    tetrisGrid.AddToGrid(currentTransform);

                    clearLinesMode = true;
                }
                else
                {
                    audioSource.clip = fallSFX;
                    audioSource.Play();
                }

                previousTime = Time.time;
            }
        }
        else
        {
            //TODO: Figure out how to update the blocks when they're in the moving phase (one row at a time)
            bool allLinesCleared = tetrisGrid.CheckForLines();

            if (allLinesCleared)
            {
                DropTetromino();
                clearLinesMode = false;
            }
        }
    }

    private void HoldTetromino()
    {
        if (!canHoldTetromino) return;

        if (currentlyHeldTetromino != null)
        {
            TetrisBlock tempTetromino = currentTetromino;
            currentTetromino = currentlyHeldTetromino;
            currentlyHeldTetromino = tempTetromino;

            currentTetromino.transform.position = tempTetromino.transform.position;

            canHoldTetromino = false;
        }
        else
        {
            currentlyHeldTetromino = currentTetromino;
            DropTetromino();
        }

        currentTetromino.transform.SetParent(tetrominoContainer.transform);
        currentlyHeldTetromino.transform.SetParent(holdContainer.transform);

        currentlyHeldTetromino.transform.localPosition = Vector3.zero;
        currentlyHeldTetromino.transform.localRotation = Quaternion.identity;

        holdContainer.GetComponent<AudioSource>().Play();
    }

    public void DropTetromino()
    {
        currentTetromino = blockSpawner.dropBlock();

        canHoldTetromino = true;
    }
}
