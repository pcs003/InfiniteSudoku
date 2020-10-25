using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public static string desiredDifficulty;

    public GameObject difficultyImage;
    public Sprite easyImage;
    public Sprite mediumImage;
    public Sprite hardImage;

    public GameObject bestTimeText;

    private void Start()
    {
        desiredDifficulty = "Easy";
        difficultyImage.GetComponent<Image>().sprite = easyImage;
        bestTimeText.GetComponent<Text>().text = DisplayBestTime(desiredDifficulty);
    }

    public void LoadScene(string name)
    {
        PlayerPrefs.SetString("SavedBoard", "Default");
        if (desiredDifficulty == "Easy")
        {
            GameSettings.Instance.SetGameMode(GameSettings.EGameMode.EASY);
            SceneManager.LoadScene(name);
        } else if (desiredDifficulty == "Medium")
        {
            GameSettings.Instance.SetGameMode(GameSettings.EGameMode.MEDIUM);
            SceneManager.LoadScene(name);
        } else
        {
            GameSettings.Instance.SetGameMode(GameSettings.EGameMode.HARD);
            SceneManager.LoadScene(name);
        }
    }

    public void AdjustDifficulty(float difficulty)
    {
        if (difficulty == 0.0f)
        {
            difficultyImage.GetComponent<Image>().sprite = easyImage;
            desiredDifficulty = "Easy";
        } else if (difficulty == 1.0f)
        {
            difficultyImage.GetComponent<Image>().sprite = mediumImage;
            desiredDifficulty = "Medium";
        } else
        {
            difficultyImage.GetComponent<Image>().sprite = hardImage;
            desiredDifficulty = "Hard";
        }
        bestTimeText.GetComponent<Text>().text = DisplayBestTime(desiredDifficulty);
    }

    public void ResetBestTimes()
    {
        PlayerPrefs.SetInt("BestEasyTime", int.MaxValue);
        PlayerPrefs.SetInt("BestMediumTime", int.MaxValue);
        PlayerPrefs.SetInt("BestHardTime", int.MaxValue);

        bestTimeText.GetComponent<Text>().text = DisplayBestTime(desiredDifficulty);
    }

    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    public string DisplayBestTime(string level)
    {
        int bestTime = 0; ;
        string defaultStr = "Best Time: 00:00:00";
        if (level == "Easy")
        {
            bestTime = PlayerPrefs.GetInt("BestEasyTime", int.MaxValue);
            
        } else if (level == "Medium")
        {
            bestTime = PlayerPrefs.GetInt("BestMediumTime", int.MaxValue);
        } else if (level == "Hard")
        {
            bestTime = PlayerPrefs.GetInt("BestHardTime", int.MaxValue);
        }
        if (bestTime != int.MaxValue)
        {
            int bestHour = bestTime / 3600;
            int bestMinute = (bestTime % 3600) / 60;
            int bestSecond = bestTime % 60;
            return "Best Time: " + LeadingZero(bestHour) + ":" + LeadingZero(bestMinute) + ":" + LeadingZero(bestSecond);
        }
        return defaultStr;
    }

    public void LoadSavedBoard(string name)
    {
        if (PlayerPrefs.GetString("SavedBoard", "Default") == "Default")
        {
            return;
        }
        SceneManager.LoadScene(name);
    }
}
