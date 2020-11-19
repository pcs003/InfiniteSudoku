using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuObject;
    public GameObject restartMenuObject;
    public GameObject assistModeText;

    public static bool inAssistMode = false;

    void Start()
    {
        pauseMenuObject.SetActive(false);
        ToggleAssistMode();
        ToggleAssistMode();
    }


    void Update()
    {
        
    }

    public void GoToMainMenu()
    {
        GameEvents.OnHomeButtonPressedMethod();
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        pauseMenuObject.SetActive(true);
    }

    public void UnpauseGame()
    {
        pauseMenuObject.SetActive(false);
    }

    public void PressedRestartButton()
    {
        restartMenuObject.SetActive(true);
    }

    public void CancelStartOver()
    {
        restartMenuObject.SetActive(false);
    }

    public void StartOver()
    {
        SudokuGrid.ResetBoard();
        restartMenuObject.SetActive(false);
        pauseMenuObject.SetActive(false);
    }

    public void ToggleAssistMode()
    {
        if (PlayerPrefs.GetInt("AssistOn", 1) == 1)
        {
            assistModeText.GetComponent<Text>().text = "Assist Mode: OFF";
            PlayerPrefs.SetInt("AssistOn", 0);

            SudokuGrid.TurnOffAssist();
        } else
        {
            assistModeText.GetComponent<Text>().text = "Assist Mode: ON";
            PlayerPrefs.SetInt("AssistOn", 1);

            SudokuGrid.TurnOnAssist();
        }
    }
}
