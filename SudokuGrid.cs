using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class SudokuGrid : MonoBehaviour
{
    public int columns = 0;
    public int rows = 0;
    public float square_offset = 0.0f;

    public GameObject gridSquare;

    public Vector2 startPos = new Vector2(0.0f, 0.0f);
    public float squareScale = 1.0f;

    public static List<GameObject> gridSquares;
    public static int[] savedBoard = new int[81];

    private int selectedGridData = -1;

    public static SudokuData.SudokuBoardData unsolvedBoard;

    public static Stack<Tuple<int,int>> undoStack = new Stack<Tuple<int, int>>();


    // Start is called before the first frame update
    void Start()
    {
        if (gridSquare.GetComponent<GridSquare>() == null)
        {
            Debug.LogError("This Game Object needs gridsquare script attached");
        }

        gridSquares = new List<GameObject>();

        CreateGrid();
        if (PlayerPrefs.GetString("SavedBoard", "Default") == "Default")
        {
            SetGridNumber(GameSettings.Instance.GetGameMode());
        } else
        {
            Debug.Log("Trying to set board from string");
            SetGridFromString(PlayerPrefs.GetString("SavedBoard"));
        }
        

        for (int i = 0; i < gridSquares.Count; i++)
        {
            if (gridSquares[i].GetComponent<GridSquare>().GetNum() == 0)
            {
                gridSquares[i].GetComponent<GridSquare>().interactable = true;
            }
            else
            {
                gridSquares[i].GetComponent<GridSquare>().interactable = false;
            }
        }

        undoStack.Clear();


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetSquaresPosition();
    }

    private void SpawnGridSquares()
    {
        int squareIdx = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                gridSquares.Add(Instantiate(gridSquare) as GameObject);
                gridSquares[gridSquares.Count - 1].GetComponent<GridSquare>().SetSquareIndex(squareIdx);
                gridSquares[gridSquares.Count - 1].transform.parent = this.transform;
                gridSquares[gridSquares.Count - 1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);

                squareIdx++;
            }
        }
    }

    private void SetSquaresPosition()
    {
        var squareRect = gridSquares[0].GetComponent<RectTransform>();
        Vector2 offset = new Vector2();
        offset.x = squareRect.rect.width * squareRect.transform.localScale.x + square_offset;
        offset.y = squareRect.rect.height * squareRect.transform.localScale.y + square_offset;

        int columnNum = 0;
        int rowNum = 0;

        foreach(GameObject square in gridSquares)
        {
            if (columnNum + 1 > columns)
            {
                rowNum++;
                columnNum = 0;
            }

            var xOffset = offset.x * columnNum;
            var yOffset = offset.y * rowNum;
            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPos.x + xOffset, startPos.y - yOffset);
            columnNum++;
        }
    }

    private void SetGridNumber(string level)
    {
        var data = SudokuEasyData.getData();

        if (level == "Medium")
        {
            data = SudokuMediumData.getData();
        } else if (level == "Hard")
        {
            data = SudokuHardData.getData();
        }

        setGridSquareData(data);

    }

    private void SetGridFromString(string str)
    {
        string[] numsAsString = str.Split(' ');
        int[] nums = new int[numsAsString.Length];

        Debug.Log(nums.Length);

        for (int i = 0; i < numsAsString.Length; i++)
        {
            nums[i] = (int)Int32.Parse(numsAsString[i]);
        }
        for (int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].GetComponent<GridSquare>().SetNumber(nums[i]);
        }
    }

    private void setGridSquareData(SudokuData.SudokuBoardData data)
    {
        unsolvedBoard = data;
        for (int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].GetComponent<GridSquare>().SetNumber(data.unsolvedData[i]);
        }
    }

    public static int[] getCurrentGrid()
    {
        int n = SudokuData.N;
        int[] output = new int[n*n];

        for (int i = 0; i < gridSquares.Count; i++)
        {
            output[i] = gridSquares[i].GetComponent<GridSquare>().GetNum();
        }

        return output;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayAgain += OnPlayAgain;
        GameEvents.OnBoardChanged += OnBoardChanged;
        GameEvents.OnHomeButtonPressed += OnHomeButtonPressed;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayAgain -= OnPlayAgain;
        GameEvents.OnBoardChanged -= OnBoardChanged;
        GameEvents.OnHomeButtonPressed -= OnHomeButtonPressed;

    }

    public void OnPlayAgain()
    {
        SetGridNumber(GameSettings.Instance.GetGameMode());
        for (int i = 0; i < gridSquares.Count; i++)
        {
            if (gridSquares[i].GetComponent<GridSquare>().GetNum() == 0)
            {
                gridSquares[i].GetComponent<GridSquare>().interactable = true;
            }
            else
            {
                gridSquares[i].GetComponent<GridSquare>().interactable = false;
            }
        }
    }

    public void OnBoardChanged(int idx, int val)
    {
        var tup = new Tuple<int, int>(idx, val);
        undoStack.Push(tup);
    }

    public void UndoButton()
    {
        Tuple<int,int> tup = undoStack.Pop();
        gridSquares[tup.Item1].GetComponent<GridSquare>().SetNumber(tup.Item2);
    }

    public void OnHomeButtonPressed()
    {
        savedBoard = getCurrentGrid();
        string boardString = "";
        for (int i = 0; i < savedBoard.Length; i++)
        {
            boardString += savedBoard[i].ToString();
            boardString += " ";
        }
        boardString = boardString.Remove(boardString.Length - 1);
        PlayerPrefs.SetString("SavedBoard", boardString);
    }
}
