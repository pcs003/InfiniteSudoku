using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* MenuButtons
 * 
 * Handles the difficulty slider, difficulty image, and high score text
 * Also initializes the game music audio source
 */
public class MenuButtons : MonoBehaviour
{
    public static string desiredDifficulty;

    public GameObject difficultyImage;
    public Sprite easyImage;
    public Sprite mediumImage;
    public Sprite hardImage;

    public GameObject bestTimeText;

    public Slider difficultySlider;

    public static AudioSource gameMusic;
    public static bool muteMusic = false;

    private void Start()
    {
        desiredDifficulty = PlayerPrefs.GetString("PreferredDifficulty", "Easy");

        // set slider and difficulty image depending on most recently chosen difficulty
        if (desiredDifficulty == "Easy")
        {
            difficultySlider.GetComponent<Slider>().value = 0;
            difficultyImage.GetComponent<Image>().sprite = easyImage;

        } else if (desiredDifficulty == "Medium")
        {
            difficultySlider.GetComponent<Slider>().value = 1;
            difficultyImage.GetComponent<Image>().sprite = mediumImage;
        } else if (desiredDifficulty == "Hard")
        {
            difficultySlider.GetComponent<Slider>().value = 2;
            difficultyImage.GetComponent<Image>().sprite = hardImage;
        }

        // display best time for currently selected difficulty
        bestTimeText.GetComponent<Text>().text = DisplayBestTime(desiredDifficulty);


        
        if (!gameMusic)
        {
            gameMusic = GetComponent<AudioSource>();
        }
        
        // start music if not muted and not already playing
        if (!gameMusic.isPlaying && !muteMusic)
        {
            gameMusic.Play();
        }
        
    }

    // loads scene based on input string
    public void LoadScene(string name)
    {
        PlayerPrefs.SetString("SavedBoard", "Default");
        if (desiredDifficulty == "Easy")
        {
            GameSettings.Instance.SetGameMode(GameSettings.EGameMode.EASY);
            SceneManager.LoadScene(name);

            PlayerPrefs.SetString("PreferredDifficulty", "Easy");
        } else if (desiredDifficulty == "Medium")
        {
            GameSettings.Instance.SetGameMode(GameSettings.EGameMode.MEDIUM);
            SceneManager.LoadScene(name);

            PlayerPrefs.SetString("PreferredDifficulty", "Medium");
        } else
        {
            GameSettings.Instance.SetGameMode(GameSettings.EGameMode.HARD);
            SceneManager.LoadScene(name);

            PlayerPrefs.SetString("PreferredDifficulty", "Hard");
        }
    }

    // code attached to slider: 0 = easy, 1 = medium, 2 = hard
    // modifies difficulty image, desiredDifficulty variable, and high score text
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

    // resets high scores for all difficulties
    public void ResetBestTimes()
    {
        PlayerPrefs.SetInt("BestEasyTime", int.MaxValue);
        PlayerPrefs.SetInt("BestMediumTime", int.MaxValue);
        PlayerPrefs.SetInt("BestHardTime", int.MaxValue);

        bestTimeText.GetComponent<Text>().text = DisplayBestTime(desiredDifficulty);
    }

    // modifies input time int to be modified to fit 00:00:00 format
    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    // returns best time as string of format 00:00:00 depending on difficulty level
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

        // if the player has a current high score for the specified difficulty then convert the bestTime int into a string of format 00:00:00
        if (bestTime != int.MaxValue)
        {
            // gets hours, minutes, and seconds from total seconds
            int bestHour = bestTime / 3600;
            int bestMinute = (bestTime % 3600) / 60;
            int bestSecond = bestTime % 60;
            return "Best Time: " + LeadingZero(bestHour) + ":" + LeadingZero(bestMinute) + ":" + LeadingZero(bestSecond);
        }
        return defaultStr;
    }

}
