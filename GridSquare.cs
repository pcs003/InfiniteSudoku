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


    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsInteractable())
        {
            selected = true;
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
        //undoStack.Push(SudokuGrid.gridSquares);
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
                GameEvents.OnBoardChangedMethod(squareIdx, num);
                if (number == 0)
                {
                    possibleNumsObject.SetActive(true);
                    SetNumber(number);
                } else
                {
                    possibleNumsObject.SetActive(false);
                    SetNumber(number);

                    if (GameCompleted())
                    {
                        GameEvents.OnGameOverMethod();
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

    

}
