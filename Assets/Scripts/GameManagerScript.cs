using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    WaitingForMoveToEnd,
    GameOver
}

public class GameManagerScript : MonoBehaviour
{
    public static GameManagerScript INSTANCE;

    public GameObject gameOverPanel;
    public GameState state;
    [Range(0, 1f)] public float delay = 0.1f;

    private bool isMoveMade = false;
    private bool[] lineMoveComplete = new bool[4] { true, true, true, true };

    private TileScript[,] allTiles = new TileScript[4, 4]; // all tiles
    private List<TileScript[]> columns = new List<TileScript[]>(); // holds all cols details
    private List<TileScript[]> rows = new List<TileScript[]>(); // holds all rows details
    private List<TileScript> emptyTiles = new List<TileScript>(); // list of empty tiles

    private void Awake()
    {
        if (INSTANCE == null)
            INSTANCE = this;
        else
            Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        gameOverPanel.SetActive(false);

        // set all tiles number's to 0 and add them to empty tiles list
        TileScript[] allTilesOnDimention = FindObjectsOfType<TileScript>();
        foreach (TileScript tile in allTilesOnDimention)
        {
            tile.Number = 0;
            allTiles[tile.indexRow, tile.indexCol] = tile;
            emptyTiles.Add(tile);
        }

        AssignColumnAndRowLists();
        GenerateTile();
        GenerateTile();
    }

    private void AssignColumnAndRowLists()
    {
        //          col1    col2    col3    col4
        // row1     0,0     0,1     0,2     0,3
        // row2     1,0     1,1     1,2     1,3
        // row3     2,0     2,1     2,2     2,3
        // row4     3,0     3,1     3,2     3,3

        columns.Add(new TileScript[] { allTiles[0, 0], allTiles[1, 0], allTiles[2, 0], allTiles[3, 0] });
        columns.Add(new TileScript[] { allTiles[0, 1], allTiles[1, 1], allTiles[2, 1], allTiles[3, 1] });
        columns.Add(new TileScript[] { allTiles[0, 2], allTiles[1, 2], allTiles[2, 2], allTiles[3, 2] });
        columns.Add(new TileScript[] { allTiles[0, 3], allTiles[1, 3], allTiles[2, 3], allTiles[3, 3] });


        rows.Add(new TileScript[] { allTiles[0, 0], allTiles[0, 1], allTiles[0, 2], allTiles[0, 3] });
        rows.Add(new TileScript[] { allTiles[1, 0], allTiles[1, 1], allTiles[1, 2], allTiles[1, 3] });
        rows.Add(new TileScript[] { allTiles[2, 0], allTiles[2, 1], allTiles[2, 2], allTiles[2, 3] });
        rows.Add(new TileScript[] { allTiles[3, 0], allTiles[3, 1], allTiles[3, 2], allTiles[3, 3] });
    }

    // ----------------------------------------- Generating New Tile With a number either 2 or 4
    private bool GenerateTile()
    {
        if (emptyTiles.Count > 0)
        {
            // pick index for random number tile;
            int indexForNewNumber = UnityEngine.Random.Range(0, emptyTiles.Count);
            int randomNumber = UnityEngine.Random.Range(0, 10);
            if (randomNumber > 8) // around 20% chances
                emptyTiles[indexForNewNumber].Number = 4;
            else
                emptyTiles[indexForNewNumber].Number = 2;
            emptyTiles.RemoveAt(indexForNewNumber);

            return true;
        }
        return false;
    }

    private void UpdateEmptyTiles()
    {
        emptyTiles.Clear();
        foreach (TileScript tile in allTiles)
        {
            if (tile.Number == 0)
                emptyTiles.Add(tile);
        }

        if (emptyTiles.Count == 0)
        {
            state = GameState.GameOver;
            GameOver();
        }

    }

    private void ResetMergeTags()
    {
        foreach (TileScript tile in allTiles)
            tile.hasMergedAlready = false;
    }

    #region MOVE  AND MERGE MECHANISM -----------------------------------------------------
    public void MoveAndMerge(MoveDirection _direction)
    {
        isMoveMade = false;
        ResetMergeTags();

        if (delay > 0)
            StartCoroutine(Co_MoveAndMerge(_direction));
        else
        {
            for (int i = 0; i < rows.Count; i++)
            {
                switch (_direction)
                {
                    case MoveDirection.Down:
                        while (MakeOneMoveUpIndex(columns[i])) { isMoveMade = true; }
                        break;

                    case MoveDirection.Right:
                        while (MakeOneMoveUpIndex(rows[i])) { isMoveMade = true; }
                        break;

                    case MoveDirection.Left:
                        while (MakeOneMoveDownIndex(rows[i])) { isMoveMade = true; }
                        break;

                    case MoveDirection.Up:
                        while (MakeOneMoveDownIndex(columns[i])) { isMoveMade = true; }
                        break;
                }
            }

            if (isMoveMade)
            {
                UpdateEmptyTiles();
                GenerateTile();
            }
        }
    }

    #region COROUTINES
    private IEnumerator Co_MoveAndMerge(MoveDirection _direction)
    {
        state = GameState.WaitingForMoveToEnd;

        // move each line waith delays depending on the move direction
        switch (_direction)
        {
            case MoveDirection.Down:
                for (int i = 0; i < columns.Count; i++)
                    StartCoroutine(Co_MoveOneLineUpIndex(columns[i], i));
                break;
            case MoveDirection.Right:
                for (int i = 0; i < rows.Count; i++)
                    StartCoroutine(Co_MoveOneLineUpIndex(rows[i], i));
                break;
            case MoveDirection.Left:
                for (int i = 0; i < rows.Count; i++)
                    StartCoroutine(Co_MoveOneLineDownIndex(rows[i], i));
                break;
            case MoveDirection.Up:
                for (int i = 0; i < columns.Count; i++)
                    StartCoroutine(Co_MoveOneLineDownIndex(columns[i], i));
                break;
        }

        // Wait until the move is over in all lines
        while (!(lineMoveComplete[0]
              && lineMoveComplete[1]
              && lineMoveComplete[2]
                 && lineMoveComplete[3]))
            yield return null;

        if (isMoveMade)
        {
            UpdateEmptyTiles();
            GenerateTile();

            if (state == GameState.GameOver)
                GameOver();
            else
                state = GameState.Playing;
        }
    }

    private IEnumerator Co_MoveOneLineDownIndex(TileScript[] tile, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveDownIndex(tile))
        {
            isMoveMade = true;
            yield return new WaitForSeconds(delay);
        }
        lineMoveComplete[index] = true;
    }

    private IEnumerator Co_MoveOneLineUpIndex(TileScript[] tile, int index)
    {
        lineMoveComplete[index] = false;
        while (MakeOneMoveUpIndex(tile))
        {
            isMoveMade = true;
            yield return new WaitForSeconds(delay);
        }
        lineMoveComplete[index] = true;
    }
    #endregion

    bool MakeOneMoveDownIndex(TileScript[] _lineOfTiles)
    {
        // This method is called on Right or Down Arrow/Swipe
        // search for move if available make a single move and return true else return false
        for (int i = 0; i < _lineOfTiles.Length - 1; i++)
        {
            // MOVE BLOCK
            if (_lineOfTiles[i].Number == 0 && _lineOfTiles[i + 1].Number != 0) // check if move is available
            {
                _lineOfTiles[i].Number = _lineOfTiles[i + 1].Number; // i is the available empty tile
                _lineOfTiles[i + 1].Number = 0;
                return true;
            }

            // MERGE BLOCK
            if (_lineOfTiles[i].Number != 0 && _lineOfTiles[i + 1].Number != 0
                && _lineOfTiles[i].Number == _lineOfTiles[i + 1].Number
                && !_lineOfTiles[i].hasMergedAlready
                && !_lineOfTiles[i + 1].hasMergedAlready)
            {
                _lineOfTiles[i].Number *= 2;
                _lineOfTiles[i + 1].Number = 0;
                _lineOfTiles[i].hasMergedAlready = true;
                ScoreManager.INSTANCE.UpdateCurrentScore(_lineOfTiles[i].Number); //  Update Score
                return true;
            }
        }
        return false;
    }

    bool MakeOneMoveUpIndex(TileScript[] _lineOfTiles)
    {
        // This method is called on Left or Up Arrow/Swipe
        // search for move if available make a single move and return true else return false
        for (int i = _lineOfTiles.Length - 1; i > 0; i--)
        {
            // Move Block
            if (_lineOfTiles[i].Number == 0 && _lineOfTiles[i - 1].Number != 0)
            {
                _lineOfTiles[i].Number = _lineOfTiles[i - 1].Number;
                _lineOfTiles[i - 1].Number = 0;
                return true;
            }

            // MERGE BLOCK
            if (_lineOfTiles[i].Number != 0 && _lineOfTiles[i - 1].Number != 0
                && _lineOfTiles[i].Number == _lineOfTiles[i - 1].Number
                && !_lineOfTiles[i].hasMergedAlready
                && !_lineOfTiles[i - 1].hasMergedAlready)
            {
                _lineOfTiles[i].Number *= 2;
                _lineOfTiles[i - 1].Number = 0;
                _lineOfTiles[i].hasMergedAlready = true;
                ScoreManager.INSTANCE.UpdateCurrentScore(_lineOfTiles[i].Number); //  Update Score
                return true;
            }
        }
        return false;
    }
    #endregion

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void NewGame_Btn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
