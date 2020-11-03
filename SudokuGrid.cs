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
    public GameObject RestartMenu;

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

        undoStack.Clear();

        CreateGrid();

        if (PlayerPrefs.GetString("SavedBoard", "Default") == "Default")
        {
            SetGridNumber(GameSettings.Instance.GetGameMode());
        } else
        {
            SetGridFromString();
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

        

        RestartMenu.SetActive(false);
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

    // sets grid and undo stack from respective strings
    private void SetGridFromString()
    {
        GameEvents.OnContinueGameMethod();

        string gridStr = PlayerPrefs.GetString("SavedBoard", "");
        string stackStr = PlayerPrefs.GetString("SavedUndoStack", "");
        string markStr = PlayerPrefs.GetString("SavedMarks", "");
        string interString = PlayerPrefs.GetString("SavedInteractables", "");

        // set grid from string

        string[] numsAsString = gridStr.Split(' ');
        int[] nums = new int[numsAsString.Length];

        for (int i = 0; i < numsAsString.Length; i++)
        {
            nums[i] = (int)Int32.Parse(numsAsString[i]);
        }
        for (int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].GetComponent<GridSquare>().SetNumber(nums[i]);
        }

        // set undo stack from string
        string[] stackString = stackStr.Split(' ');

        for (int i = 0; i < stackString.Length - 1; i++)
        {
            string[] tupString = stackString[i].Split(',');
            int idx = int.Parse(tupString[0]);
            int num = int.Parse(tupString[1]);

            Tuple<int, int> tup = new Tuple<int, int>(idx, num);
            undoStack.Push(tup);
        }

        // set marks from string

        string[] markSets = markStr.Split(',');
        Debug.Log(markStr);
        for (int i = 0; i < markSets.Length - 1; i++)
        {
            string[] marks = markSets[i].Split(' ');
            int index = int.Parse(marks[0]);
            
            for (int j = 1; j < marks.Length; j++)
            {
                Debug.Log(index);
                
                int markIdx = int.Parse(marks[j]);
                Debug.Log(markIdx);
                gridSquares[index].GetComponent<GridSquare>().possibleNums[markIdx].SetActive(true);
            }
        }

        // sets interactable from string

        for (int i = 0; i < interString.Length; i++)
        {
            if (interString[i] == '1')
            {
                gridSquares[i].GetComponent<GridSquare>().interactable = true;
            } else
            {
                gridSquares[i].GetComponent<GridSquare>().interactable = false;
            }
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

    // store recent change to undo stack
    public void OnBoardChanged(int idx, int val)
    {
        var tup = new Tuple<int, int>(idx, val);
        undoStack.Push(tup);
    }

    // pop recent change from undo stack and undo it
    public void UndoButton()
    {
        if (undoStack.Count > 0)
        {
            Tuple<int, int> tup = undoStack.Pop();
            gridSquares[tup.Item1].GetComponent<GridSquare>().SetNumber(tup.Item2);
        } else
        {
            return;
        }
        
    }

    // save board state, undo stack state, marks state, and interactable states as strings in PlayerPrefs
    public void OnHomeButtonPressed()
    {
        // Save board state to player prefs as string ex. "1 4 5 8 9 2 0 3"
        savedBoard = getCurrentGrid();
        string boardString = "";
        for (int i = 0; i < savedBoard.Length; i++)
        {
            boardString += savedBoard[i].ToString();
            boardString += " ";
        }
        boardString = boardString.Remove(boardString.Length - 1);
        PlayerPrefs.SetString("SavedBoard", boardString);

        // save undo stack to player prefs as string ex. "1,5 2,6 41,3"
        string stackString = "";
        int total = undoStack.Count;
        for (int i = 0; i < total; i++)
        {
            Tuple<int, int> tup = undoStack.Pop();
            string tupString = tup.Item1.ToString() + "," + tup.Item2.ToString();
            stackString = tupString + " " + stackString;
        }

        PlayerPrefs.SetString("SavedUndoStack", stackString);

        string squareMarks = "";
        string markString = "";

        for (int i = 0; i < gridSquares.Count; i++)
        {
            squareMarks = i.ToString();
            bool hasMarks = false;
            for (int j = 0; j < 9; j++)
            {
                if (gridSquares[i].GetComponent<GridSquare>().possibleNums[j].activeSelf)
                {
                    squareMarks = squareMarks + " " + j.ToString();
                    hasMarks = true;
                }
            }
            if (hasMarks)
            {
                markString += squareMarks;
                markString += ",";
            }
            
            squareMarks = "";
            
        }

        PlayerPrefs.SetString("SavedMarks", markString);

        string interactables = "";
        for (int i = 0; i < gridSquares.Count; i++)
        {
            if (gridSquares[i].GetComponent<GridSquare>().IsInteractable())
            {
                interactables += "1";
            } else
            {
                interactables += "0";
            }
        }

        PlayerPrefs.SetString("SavedInteractables", interactables);
    }

    // brings up confirmation popup menu
    public void PressedRestartButton()
    {
        RestartMenu.SetActive(true);
    }

    // confirm start over and close menu
    public void StartOver()
    {
        for (int i = 0; i < gridSquares.Count; i++)
        {
            if (gridSquares[i].GetComponent<GridSquare>().IsInteractable())
            {
                gridSquares[i].GetComponent<GridSquare>().SetNumber(0);
            }
        }
        RestartMenu.SetActive(false);
    }

    // cancel start over and close menu
    public void CancelStartOver()
    {
        RestartMenu.SetActive(false);
    }
}
