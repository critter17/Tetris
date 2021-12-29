using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static Vector3 startingPoint = new Vector3(5.0f, 18f, 0.0f);

    // Serialized Variables
    [SerializeField] private TetrisGrid tetrisGrid;
    [SerializeField] private BlockSpawner blockSpawner;
    [SerializeField] private GameObject tetrominoContainer;
    [SerializeField] private GameObject holdContainer;
    [SerializeField] private GameObject nextContainer;

    private float previousTime;
    private float dirPreviousTime;
    [SerializeField] private TetrisBlock currentTetromino;
    [SerializeField] private TetrisBlock nextTetromino;
    private bool canHoldTetromino;
    private TetrisBlock currentlyHeldTetromino;
    private bool clearLinesMode;

    // Audio Source
    private AudioSource audioSource;

    // Audio Clips
    [SerializeField] private AudioClip fallSFX;
    [SerializeField] private AudioClip touchSFX;
    [SerializeField] private AudioClip touchDownSFX;
    [SerializeField] private AudioClip moveSFX;
    [SerializeField] private AudioClip rotateSFX;
    [SerializeField] private AudioClip rotateFailSFX;
    [SerializeField] private AudioClip hardDropSFX;

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
        Transform currentTransform = currentTetromino.Blocks.transform;

        if (!clearLinesMode)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currentTetromino.FallSpeed = 0.10f;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                currentTetromino.FallSpeed = 1.0f;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (Time.time - dirPreviousTime > currentTetromino.DirSpeed)
                {
                    currentTetromino.MoveLeft();

                    if (!tetrisGrid.TryMove(currentTransform))
                    {
                        currentTetromino.MoveRight();
                        audioSource.clip = touchSFX;
                    }
                    else
                    {
                        audioSource.clip = moveSFX;
                    }

                    audioSource.Play();

                    dirPreviousTime = Time.time;
                }

                UpdateBlockShadows(currentTransform);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (Time.time - dirPreviousTime > currentTetromino.DirSpeed)
                {
                    currentTetromino.MoveRight();

                    if (!tetrisGrid.TryMove(currentTransform))
                    {
                        currentTetromino.MoveLeft();
                        audioSource.clip = touchSFX;
                    }
                    else
                    {
                        audioSource.clip = moveSFX;
                    }

                    audioSource.Play();

                    dirPreviousTime = Time.time;
                }

                UpdateBlockShadows(currentTransform);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                currentTetromino.RotateRight();
                audioSource.clip = rotateSFX;

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.RotateLeft();
                    audioSource.clip = rotateFailSFX;
                }

                audioSource.Play();

                UpdateBlockShadows(currentTransform);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                currentTetromino.RotateLeft();
                audioSource.clip = rotateSFX;

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.RotateRight();
                    audioSource.clip = rotateFailSFX;
                }

                audioSource.Play();

                UpdateBlockShadows(currentTransform);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                HoldTetromino();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentTetromino.transform.position = currentTetromino.DropShadowBlocks.transform.position;
                LockTetromino(currentTransform);
                audioSource.clip = hardDropSFX;
                audioSource.Play();
                clearLinesMode = true;
            }

            if (Time.time - previousTime > currentTetromino.FallSpeed && !clearLinesMode)
            {
                currentTetromino.MoveDown();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.MoveUp();

                    LockTetromino(currentTransform);

                    audioSource.clip = touchDownSFX;
                    audioSource.Play();

                    clearLinesMode = true;
                }
                else
                {
                    audioSource.clip = fallSFX;
                    audioSource.Play();
                    UpdateBlockShadows(currentTransform);
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
                Debug.Log("All possible lines cleared");
                DropTetromino();
                clearLinesMode = false;
            }
        }
    }

    private void LockTetromino(Transform currentTransform)
    {
        tetrisGrid.AddToGrid(currentTransform);
        currentTetromino.DropShadowBlocks.SetActive(false);
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
            currentTetromino.DropShadowBlocks.SetActive(true);
            UpdateBlockShadows(currentTetromino.Blocks.transform);

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

        currentlyHeldTetromino.DropShadowBlocks.SetActive(false);

        holdContainer.GetComponent<AudioSource>().Play();
    }

    public void DropTetromino()
    {
        if (!nextTetromino)
        {
            nextTetromino = blockSpawner.dropBlock();
        }

        // TODO: Swap currentTetromino with block in nextTetromino, then spawn new block in next
        currentTetromino = nextTetromino;
        currentTetromino.transform.SetParent(tetrominoContainer.transform);
        currentTetromino.transform.localPosition = startingPoint;
        currentTetromino.DropShadowBlocks.SetActive(true);

        Transform currentTransform = currentTetromino.Blocks.transform;

        nextTetromino = blockSpawner.dropBlock();

        nextTetromino.transform.SetParent(nextContainer.transform);
        nextTetromino.transform.localPosition = Vector3.zero;

        nextTetromino.DropShadowBlocks.SetActive(false);

        canHoldTetromino = true;

        UpdateBlockShadows(currentTransform);
    }

    private void UpdateBlockShadows(Transform currentTransform)
    {
        bool hitBottom = false;
        Transform shadowBlocks = currentTetromino.DropShadowBlocks.transform;
        shadowBlocks.position = new Vector3(currentTransform.position.x, currentTransform.position.y);

        while (!hitBottom)
        {
            for (int j = 0; j < shadowBlocks.childCount; j++)
            {
                Transform shadowBlock = shadowBlocks.GetChild(j);
                int column = Mathf.RoundToInt(shadowBlock.position.x);
                int row = Mathf.RoundToInt(shadowBlock.position.y);

                if (row >= 0 && tetrisGrid.Grid[column, row] != null)
                {
                    shadowBlocks.position = new Vector3(shadowBlocks.position.x, shadowBlocks.position.y + 1);
                    hitBottom = true;
                }
                else if (row == 0)
                {
                    hitBottom = true;
                }
            }

            if (!hitBottom)
            {
                shadowBlocks.position = new Vector3(shadowBlocks.position.x, shadowBlocks.position.y - 1);
            }
        }
    }
}
