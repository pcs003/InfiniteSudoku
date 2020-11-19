using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/* GameEvents
 * 
 * Event manager class. All events initialized here
 */
public class GameEvents : MonoBehaviour
{
    // event when any gridsquare's number is changed
    public delegate void UpdateSquareNumber(int number);
    public static event UpdateSquareNumber OnUpdateSquareNumber;

    public static void UpdateSquareNumberMethod(int number)
    {
        if (OnUpdateSquareNumber != null)
        {
            OnUpdateSquareNumber(number);
        }
    }

    // Event when a gridsquare is selected
    public delegate void SquareSelected(int squareIdx);
    public static event SquareSelected OnSquareSelected;

    public static void SquareSelectedMethod(int index)
    {
        if (OnSquareSelected != null)
        {
            OnSquareSelected(index);
        }
    }

    // event on toggling edit mode
    public delegate void UpdateEditMode();
    public static event UpdateEditMode OnEditModeChanged;

    public static void UpdateEditModeMethod()
    {
        if (OnEditModeChanged != null)
        {
            OnEditModeChanged();
        }
    }

    // event on puzzle finished
    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public static void OnGameOverMethod()
    {
        if (OnGameOver != null)
        {
            OnGameOver();
        }
    }

    // event on play again clicked
    public delegate void PlayAgain();
    public static event PlayAgain OnPlayAgain;

    public static void OnPlayAgainMethod()
    {
        if (OnPlayAgain != null)
        {
            OnPlayAgain();
        }
    }

    // event on any board change
    public delegate void BoardChanged(int idx, int prev, int newVal);
    public static event BoardChanged OnBoardChanged;

    public static void OnBoardChangedMethod(int idx, int prev, int newVal)
    {
        if (OnBoardChanged != null)
        {
            OnBoardChanged(idx, prev, newVal);
        }
    }

    // event on home button pressed
    public delegate void HomeButtonPressed();
    public static event HomeButtonPressed OnHomeButtonPressed;

    public static void OnHomeButtonPressedMethod()
    {
        if (OnHomeButtonPressed != null)
        {
            OnHomeButtonPressed();
        }
    }

    // event on continuing saved game
    public delegate void ContinueGame();
    public static event ContinueGame OnContinueGame;

    public static void OnContinueGameMethod()
    {
        if (OnContinueGame != null)
        {
            OnContinueGame();
        }
    }
}
