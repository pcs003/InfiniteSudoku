using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    public static int gameTime;
    private int hour;
    private int minute;
    private int second;

    private Text clockText;

    float deltaTime;
    private bool stopClock = false;

    public static Timer instance;

    public GameObject gameFinshedPopup;
    public GameObject thisRunTimeObject;
    public GameObject bestRunTimeObject;
    public GameObject newHighScoreCelebrate;

    public int bestTime;

    private void Awake()
    {
        if (instance)
        {
            Destroy(instance);
        }
        instance = this;

        clockText = GetComponent<Text>();
        deltaTime = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        stopClock = false;
        newHighScoreCelebrate.SetActive(false);
        gameFinshedPopup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (stopClock == false)
        {
            deltaTime += Time.deltaTime;
            TimeSpan span = TimeSpan.FromSeconds(deltaTime);

            string hours = LeadingZero(span.Hours);
            string minutes = LeadingZero(span.Minutes);
            string seconds = LeadingZero(span.Seconds);

            clockText.text = hours + ":" + minutes + ":" + seconds;
        }
    }

    string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

    public void OnGameOver()
    {
        stopClock = true;

        thisRunTimeObject.GetComponent<Text>().text = "Time: " + clockText.text;

        int totalSeconds = (int)deltaTime;
        int defaultVal = int.MaxValue;
        int bestTime = 0;

        Debug.Log(totalSeconds);

        if (GameSettings.Instance.GetGameMode() == "Easy")
        {
            Debug.Log("got here1");
            if (totalSeconds < PlayerPrefs.GetInt("BestEasyTime",defaultVal))
            {
                Debug.Log("got here2");
                PlayerPrefs.SetInt("BestEasyTime", totalSeconds);
                newHighScoreCelebrate.SetActive(true);
            }
            bestTime = PlayerPrefs.GetInt("BestEasyTime");
        } else if (GameSettings.Instance.GetGameMode() == "Medium")
        {
            if (totalSeconds < PlayerPrefs.GetInt("BestMediumTime", defaultVal))
            {
                PlayerPrefs.SetInt("BestMediumTime", totalSeconds);
                newHighScoreCelebrate.SetActive(true);
            }
            bestTime = PlayerPrefs.GetInt("BestMediumTime");
        } else if (GameSettings.Instance.GetGameMode() == "Hard")
        {
            if (totalSeconds < PlayerPrefs.GetInt("BestHardTime", defaultVal))
            {
                PlayerPrefs.SetInt("BestHardTime", totalSeconds);
                newHighScoreCelebrate.SetActive(true);
            }
            bestTime = PlayerPrefs.GetInt("BestHardTime");
        }

        int bestHour = bestTime / 3600;
        int bestMinute = (bestTime % 3600) / 60;
        int bestSecond = bestTime % 60;
        bestRunTimeObject.GetComponent<Text>().text = "Best Time: " + LeadingZero(bestHour) + ":" + LeadingZero(bestMinute) + ":" + LeadingZero(bestSecond);

        gameFinshedPopup.SetActive(true);
        PlayerPrefs.SetString("SavedBoard", "Default");
    }

    public void OnPlayAgain()
    {
        gameFinshedPopup.SetActive(false);
        newHighScoreCelebrate.SetActive(false);
        deltaTime = 0;
        stopClock = false;
    }

    private void OnEnable()
    {
        GameEvents.OnGameOver += OnGameOver;
        GameEvents.OnPlayAgain += OnPlayAgain;
        GameEvents.OnHomeButtonPressed += OnHomeButtonPressed;
        GameEvents.OnContinueGame += OnContinueGame;
    }

    private void OnDisable()
    {
        GameEvents.OnGameOver -= OnGameOver;
        GameEvents.OnPlayAgain -= OnPlayAgain;
        GameEvents.OnHomeButtonPressed -= OnHomeButtonPressed;
        GameEvents.OnContinueGame -= OnContinueGame;
    }

    public Text GetCurrentTimeText()
    {
        return clockText;
    }

    public void OnHomeButtonPressed()
    {
        PlayerPrefs.SetFloat("SavedTime", deltaTime);
    }

    public void OnContinueGame()
    {
        deltaTime = PlayerPrefs.GetFloat("SavedTime", 0.0f);
    }
}
