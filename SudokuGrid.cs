using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        if (PlayerPrefs.GetString("SavedBoard", "Default") == "Default") // if there is no saved board or new game is pressed
        {
            SetGridNumber(GameSettings.Instance.GetGameMode());

            // sets given squares to not interactable
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
        } else // if there is a saved board and continue is pressed
        {
            SetGridFromString();
        }

        RestartMenu.SetActive(false);
    }

    // calls functions to create empty grid
    private void CreateGrid()
    {
        SpawnGridSquares();
        SetSquaresPosition();
    }

    // instantiates gridSquare GameObjects
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

    // sets positions of instantiated gridSquares to form a grid
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

    // sets grid data based on level
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
        string correctString = PlayerPrefs.GetString("SavedCorrectBoard", "");

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

        for (int i = 0; i < markSets.Length - 1; i++)
        {
            string[] marks = markSets[i].Split(' ');
            int index = int.Parse(marks[0]);

            //gridSquares[index].GetComponent<GridSquare>().possibleNumsObject.SetActive(true);

            for (int j = 1; j < marks.Length; j++)
            {
                
                
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

        // sets correct board from string

        for (int i = 0; i < correctString.Length; i++)
        {
            int row = i / 9;
            int col = i % 9;

            SudokuData.filledMat[row, col] = int.Parse(correctString[i].ToString());
        }



    }

    // sets board numbers using input board data
    private void setGridSquareData(SudokuData.SudokuBoardData data)
    {
        unsolvedBoard = data;
        for (int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].GetComponent<GridSquare>().SetNumber(data.unsolvedData[i]);
        }
    }

    // returns current grid as an int array
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

    // pop recent change from undo stack and undo it
    public void UndoButton()
    {
        if (undoStack.Count > 0)
        {
            Tuple<int, int> tup = undoStack.Pop();
            gridSquares[tup.Item1].GetComponent<GridSquare>().SetNumber(tup.Item2);
        }
        else
        {
            return;
        }
    }

    // resets board to start state
    public static void ResetBoard()
    {
        for (int i = 0; i < gridSquares.Count; i++)
        {
            if (gridSquares[i].GetComponent<GridSquare>().IsInteractable())
            {
                gridSquares[i].GetComponent<GridSquare>().SetNumber(0);
            }
        }
    }

    // sets relevant squares to highlighted color based on given index of selected square
    public static void HighlightAssist(int idx)
    {
        // reset gridSquare colors
        Color normalColor = new Color(1, 1, 1, 0.7058f);
        for (int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].GetComponent<GridSquare>().SetColor(normalColor);
        }

        Color highlightedColor = new Color(0.6650f, 0.9878f, 1, 1);
        int row = idx / 9;
        int col = idx % 9;

        // set squares in same row and column to highlighted color
        for (int i = 0; i < SudokuData.N; i++)
        {
            int rowStart = idx - (idx % 9);

            gridSquares[rowStart + i].GetComponent<GridSquare>().SetColor(highlightedColor);
            gridSquares[col + (9 * i)].GetComponent<GridSquare>().SetColor(highlightedColor);
        }

        // set squares in same box to highlighted color
        int trow = (row / SudokuData.order) * SudokuData.order;
        int lcol = (col / SudokuData.order) * SudokuData.order;
        for (int i = 0; i < SudokuData.order; ++i)
        {
            for (int j = 0; j < SudokuData.order; ++j)
            {
                int val = (trow + i) * 9 + lcol + j;
                gridSquares[val].GetComponent<GridSquare>().SetColor(highlightedColor);

            }
        }
    }

    // removes assist annotations when toggling assist mode off
    public static void TurnOffAssist()
    {
        // reset colors of squares and numbers
        Color normalColor = new Color(1, 1, 1, 0.7058f);
        for (int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].GetComponent<GridSquare>().SetColor(normalColor);
            gridSquares[i].GetComponent<GridSquare>().numText.GetComponent<Text>().color = Color.black;
        }
        
    }

    // adds assist annotations when toggling assist mode on
    public static void TurnOnAssist()
    {
        // sets incorrect numbers to red
        for (int i = 0; i < gridSquares.Count; i++)
        {
            int row = i / 9;
            int col = i % 9;
            int thisNum = gridSquares[i].GetComponent<GridSquare>().num;
            if (thisNum != SudokuData.filledMat[row, col] && thisNum != 0)
            {
                gridSquares[i].GetComponent<GridSquare>().numText.GetComponent<Text>().color = Color.red;
            }
            
        }
    }

    // when a square is filled, removes that number from the marks of relevant squares
    // this is an assist mode feature
    public void RemoveMarks(int val, int idx)
    {
        int row = idx / 9;
        int col = idx % 9;

        // removes mark from rows and columns
        for (int i = 0; i < SudokuData.N; i++)
        {
            int rowStart = idx - (idx % 9);

            gridSquares[rowStart + i].GetComponent<GridSquare>().possibleNums[val - 1].SetActive(false);
            gridSquares[col + (9 * i)].GetComponent<GridSquare>().possibleNums[val - 1].SetActive(false);
        }

        // removes mark from squares in same box
        int trow = (row / SudokuData.order) * SudokuData.order;
        int lcol = (col / SudokuData.order) * SudokuData.order;
        for (int i = 0; i < SudokuData.order; ++i)
        {
            for (int j = 0; j < SudokuData.order; ++j)
            {
                int index = (trow + i) * 9 + lcol + j;
                gridSquares[index].GetComponent<GridSquare>().possibleNums[val - 1].SetActive(false);

            }
        }
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

    // Starts new board on play again
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
    public void OnBoardChanged(int idx, int prev, int newVal)
    {
        var tup = new Tuple<int, int>(idx, prev);
        undoStack.Push(tup);

        if (PlayerPrefs.GetInt("AssistOn", 1) == 1)
        {
            RemoveMarks(newVal, idx);
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


        // save marks as string in PlayerPrefs to be parsed on resume

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


        // save interactability of each square: 1 = interactable, 0 = not interactable

        string interactables = "";
        for (int i = 0; i < gridSquares.Count; i++)
        {
            if (gridSquares[i].GetComponent<GridSquare>().IsInteractable())
            {
                interactables += "1";
            }
            else
            {
                interactables += "0";
            }
        }

        PlayerPrefs.SetString("SavedInteractables", interactables);


        // save solved board as 81 char string

        string correctBoard = "";

        for (int i = 0; i < SudokuData.N; i++)
        {
            for (int j = 0; j < SudokuData.N; j++)
            {
                correctBoard += SudokuData.filledMat[i, j].ToString();
            }
        }

        PlayerPrefs.SetString("SavedCorrectBoard", correctBoard);
    }

}
