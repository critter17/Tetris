using CritterGames.UI;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Play,
    ClearLines,
    DropBlocks,
    Paused
}

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
    private GameState gameState;
    private GameState previousGameState;
    private List<Transform> blocksToClear;
    List<Transform> blocksAboveClearedBlocks;
    private bool clearedLines;
    private int lowestRow;
    private bool isGameOver = false;

    // UI
    [SerializeField] private UISystem menu;

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

        if (Debug.isDebugBuild)
        {
            DebugGrid debugUI = FindObjectOfType<DebugGrid>(true);
            debugUI.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentlyHeldTetromino = null;
        blocksToClear = new List<Transform>();
        blocksAboveClearedBlocks = new List<Transform>();
        clearedLines = false;
        gameState = GameState.Play;
        DropTetromino();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (gameState == GameState.Play)
        {
            Transform currentTransform = currentTetromino.Blocks.transform;

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
                CheckForLinesFormed();
                previousTime = Time.time;
            }

            if (Time.time - previousTime > currentTetromino.FallSpeed)
            {
                currentTetromino.MoveDown();

                if (!tetrisGrid.TryMove(currentTransform))
                {
                    currentTetromino.MoveUp();

                    LockTetromino(currentTransform);

                    audioSource.clip = touchDownSFX;
                    audioSource.Play();

                    CheckForLinesFormed();
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
        else if (gameState == GameState.ClearLines)
        {
            if (!clearedLines)
            {
                clearedLines = true;
                float animationLength = 0.0f;

                foreach (Transform t in blocksToClear)
                {
                    animationLength = t.GetComponent<Animation>().clip.length;
                    t.GetComponent<Animation>().Play();
                }

                Invoke("AfterBlockAnimationsFinish", animationLength + 1.0f);
            }
        }
        else if (gameState == GameState.DropBlocks)
        {
            if (Time.time - previousTime > 0.75f)
            {
                List<Transform> blocksNotGrounded = new List<Transform>();

                for (int row = 0; row < tetrisGrid.GridHeight; row++)
                {
                    for (int column = 0; column < tetrisGrid.GridWidth; column++)
                    {
                        if (tetrisGrid.Grid[column, row] &&
                            !tetrisGrid.IsBlockGrounded(column, row) &&
                            blocksAboveClearedBlocks.Contains(tetrisGrid.Grid[column, row]))
                        {
                            blocksNotGrounded.Add(tetrisGrid.Grid[column, row]);
                            tetrisGrid.MoveBlockDown(column, row);
                        }
                    }
                }

                if (blocksNotGrounded.Count == 0)
                {
                    clearedLines = false;
                    CheckForLinesFormed();
                }

                previousTime = Time.time;
            }
        }
    }

    private void TogglePause()
    {
        if (gameState != GameState.Paused)
        {
            previousGameState = gameState;
            SetGameState(GameState.Paused);
        }
        else
        {
            SetGameState(previousGameState);
        }

        menu.gameObject.SetActive(!menu.gameObject.activeSelf);
    }

    public void AfterBlockAnimationsFinish()
    {
        SetBlocksAboveClearedBlocks();
        SetGameState(GameState.DropBlocks);
    }

    public void SetGameState(GameState newState)
    {
        gameState = newState;
    }

    private void CheckForLinesFormed()
    {
        blocksToClear = tetrisGrid.CheckForLines();

        if (blocksToClear.Count == 0)
        {
            DropTetromino();
            gameState = GameState.Play;
            blocksToClear.Clear();
            blocksAboveClearedBlocks.Clear();
        }
        else
        {
            lowestRow = tetrisGrid.GetLowestRowCleared(blocksToClear);
            SetGameState(GameState.ClearLines);
        }
    }

    public void SetBlocksAboveClearedBlocks()
    {
        blocksAboveClearedBlocks = tetrisGrid.GetBlocksAboveRow(lowestRow);
    }

    private void LockTetromino(Transform currentTransform)
    {
        if (!tetrisGrid.AddToGrid(currentTransform))
        {
            Debug.Log("Game is over");
            currentTetromino.FallSpeed = 0.0f;
            isGameOver = true;
        }
        currentTetromino.DropShadowBlocks.SetActive(false);
    }

    private void HoldTetromino()
    {
        if (!canHoldTetromino) return;

        if (currentlyHeldTetromino != null)
        {
            (currentlyHeldTetromino, currentTetromino) = (currentTetromino, currentlyHeldTetromino);
            SetCurrentPieceAtStartingPoint();

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
        currentlyHeldTetromino.transform.position = currentlyHeldTetromino.transform.GetChild(currentlyHeldTetromino.transform.childCount - 1).position;

        currentlyHeldTetromino.DropShadowBlocks.SetActive(false);

        holdContainer.GetComponent<AudioSource>().Play();
    }

    public void DropTetromino()
    {
        if (isGameOver)
        {
            return;
        }

        if (!nextTetromino)
        {
            nextTetromino = blockSpawner.dropBlock();
        }

        currentTetromino = nextTetromino;
        currentTetromino.transform.SetParent(tetrominoContainer.transform);
        SetCurrentPieceAtStartingPoint();
        
        nextTetromino = blockSpawner.dropBlock();
        nextTetromino.transform.SetParent(nextContainer.transform);
        nextTetromino.transform.localPosition = Vector3.zero;
        nextTetromino.transform.position = nextTetromino.transform.GetChild(nextTetromino.transform.childCount - 1).position;
        nextTetromino.DropShadowBlocks.SetActive(false);

        canHoldTetromino = true;
    }

    private void SetCurrentPieceAtStartingPoint()
    {
        currentTetromino.transform.position = startingPoint;

        if (currentTetromino.name[0] == 'I' || currentTetromino.name[0] == 'O' ||
            currentTetromino.name[0] == 'S' || currentTetromino.name[0] == 'Z')
        {
            currentTetromino.transform.position += Vector3Int.up;
        }
        currentTetromino.DropShadowBlocks.SetActive(true);

        while (!tetrisGrid.TryMove(currentTetromino.Blocks.transform))
        {
            currentTetromino.MoveUp();
        }

        UpdateBlockShadows(currentTetromino.Blocks.transform);
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
