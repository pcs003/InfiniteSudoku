using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameEvents : MonoBehaviour
{
    public delegate void UpdateSquareNumber(int number);
    public static event UpdateSquareNumber OnUpdateSquareNumber;

    public static void UpdateSquareNumberMethod(int number)
    {
        if (OnUpdateSquareNumber != null)
        {
            OnUpdateSquareNumber(number);
        }
    }


    public delegate void SquareSelected(int squareIdx);
    public static event SquareSelected OnSquareSelected;

    public static void SquareSelectedMethod(int index)
    {
        if (OnSquareSelected != null)
        {
            OnSquareSelected(index);
        }
    }

    public delegate void UpdateEditMode();
    public static event UpdateEditMode OnEditModeChanged;

    public static void UpdateEditModeMethod()
    {
        if (OnEditModeChanged != null)
        {
            OnEditModeChanged();
        }
    }

    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public static void OnGameOverMethod()
    {
        if (OnGameOver != null)
        {
            OnGameOver();
        }
    }

    public delegate void PlayAgain();
    public static event PlayAgain OnPlayAgain;

    public static void OnPlayAgainMethod()
    {
        if (OnPlayAgain != null)
        {
            OnPlayAgain();
        }
    }

    public delegate void BoardChanged(int idx, int val);
    public static event BoardChanged OnBoardChanged;

    public static void OnBoardChangedMethod(int idx, int val)
    {
        if (OnBoardChanged != null)
        {
            OnBoardChanged(idx, val);
        }
    }

    public delegate void HomeButtonPressed();
    public static event HomeButtonPressed OnHomeButtonPressed;

    public static void OnHomeButtonPressedMethod()
    {
        if (OnHomeButtonPressed != null)
        {
            OnHomeButtonPressed();
        }
    }

}
