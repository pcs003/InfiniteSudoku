using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GridSquare : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    public GameObject numText;

    public GameObject possibleNumsObject;

    public GameObject[] possibleNums;

    public int num = 0;

    public bool selectable = true;

    private bool selected = false;
    private int squareIdx = -1;

    //public static Stack<List<GameObject>> undoStack = new Stack<List<GameObject>>();

    public static bool inEditMode = false;

    public bool IsSelected() { return selected; }

    public void SetSquareIndex(int index)
    {
        squareIdx = index;
    }

    public void SetSelectable(bool isSelectable)
    {
        selectable = isSelectable;
    }

    public int GetNum()
    {
        return num;
    }


    // Start is called before the first frame update
    void Start()
    {
        selected = false;
        inEditMode = false;
        for (int i = 0; i < possibleNums.Length; i++)
        {
            possibleNums[i].SetActive(false);
        }
        possibleNumsObject.SetActive(true);

        //undoStack.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayText()
    {
        if (num <= 0)
        {
            numText.GetComponent<Text>().text = " ";
        } else
        {
            numText.GetComponent<Text>().text = num.ToString();
        }
    }

    public void SetNumber(int inputNum)
    {
        num = inputNum;
        DisplayText();
    }

    public void SetMarkActive(int idx)
    {
        possibleNums[idx].SetActive(true);
    }

    public void SetMarkInactive(int idx)
    {
        possibleNums[idx].SetActive(false);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInteractable())
        {
            selected = true;
            if (PlayerPrefs.GetInt("AssistOn", 1) == 1)
            {
                SudokuGrid.HighlightAssist(squareIdx);
            }
            
            GameEvents.SquareSelectedMethod(squareIdx);
        }
        
    }

    public void OnSubmit(BaseEventData eventData)
    {

    }

    private void OnEnable()
    {
        GameEvents.OnUpdateSquareNumber += OnSetNumber;
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnEditModeChanged += OnEditModeChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnUpdateSquareNumber -= OnSetNumber;
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnEditModeChanged -= OnEditModeChanged;
    }

    public void OnSetNumber(int number)
    {

        if (selected)
        {
            if (inEditMode)
            {
                if (number == 0)
                {
                    for (int i = 0; i < possibleNums.Length; i++)
                    {
                        possibleNums[i].SetActive(false);
                    }
                    SetNumber(number);
                } else
                {
                    SetNumber(0);
                    RightNum();
                    possibleNumsObject.SetActive(true);

                    if (possibleNums[number - 1].activeSelf)
                    {
                        possibleNums[number - 1].SetActive(false);
                    }
                    else
                    {
                        possibleNums[number - 1].SetActive(true);
                    }
                }
                
            } else
            {
                GameEvents.OnBoardChangedMethod(squareIdx, num, number);
                if (number == 0)
                {
                    possibleNumsObject.SetActive(true);
                    SetNumber(number);

                    //set color to normal
                    RightNum();
                } else
                {
                    possibleNumsObject.SetActive(false);
                    SetNumber(number);

                    if (GameCompleted())
                    {
                        GameEvents.OnGameOverMethod();
                    }

                    int rowIdx = squareIdx / 9;
                    int colIdx = squareIdx % 9;
                    if (number != SudokuData.filledMat[rowIdx,colIdx])
                    {
                        if (PlayerPrefs.GetInt("AssistOn", 1) == 1)
                        {
                            WrongNum();
                        }
                        
                    } else
                    {
                        RightNum();
                    }
                }
                
            }
            
        }
    }

    public void OnSquareSelected(int index)
    {
        if (squareIdx != index)
        {
            selected = false;
        }
    }

    public void OnEditModeChanged()
    {
        if (inEditMode)
        {
            inEditMode = false;
        } else
        {
            inEditMode = true;
        }
    }

    public bool GameCompleted()
    {
        int[] currGrid = SudokuGrid.getCurrentGrid();

        if (SudokuData.IsCompleted(currGrid))
        {
            return true;
        }

        return false;
    }

    public void WrongNum()
    {
        numText.GetComponent<Text>().color = Color.red;
        //ColorBlock cb = GetComponent<GridSquare>().colors;
        

        //cb.normalColor = newColor;
        //cb.selectedColor = newColor;
        //GetComponent<GridSquare>().colors = cb;
    }

    public void RightNum()
    {
        //ColorBlock cb = GetComponent<GridSquare>().colors;
        //Color newColor1 = new Color(0, 1, 1, 0.7058f);
        numText.GetComponent<Text>().color = Color.black;
        //Color newColor2 = new Color(0.666f, 0.9882f, 1, 0.7058f);

        //cb.normalColor = newColor1;
        //cb.selectedColor = newColor2;
        //GetComponent<GridSquare>().colors = cb;
    }

    public void SetColor(Color color)
    {
        ColorBlock cb = GetComponent<GridSquare>().colors;
        cb.normalColor = color;
        cb.disabledColor = color;
        GetComponent<GridSquare>().colors = cb;
    }
    

}
