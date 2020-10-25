using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public enum EGameMode
    {
        NOT_SET,
        EASY,
        MEDIUM,
        HARD
    }

    public static GameSettings Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        } else
        {
            Destroy(this);
        }
    }

    private static EGameMode gameMode;

    private void Start()
    {
        gameMode = EGameMode.NOT_SET;
    }

    public void SetGameMode(EGameMode mode)
    {
        gameMode = mode;
    }

    public void SetGameMode(string mode)
    {
        if (mode == "Easy") SetGameMode(EGameMode.EASY);
        else if (mode == "Medium") SetGameMode(EGameMode.MEDIUM);
        else if (mode == "Hard") SetGameMode(EGameMode.HARD);
        else SetGameMode(EGameMode.NOT_SET);
    }

    public string GetGameMode()
    {
        switch (gameMode)
        {
            case EGameMode.EASY: return "Easy";
            case EGameMode.MEDIUM: return "Medium";
            case EGameMode.HARD: return "Hard";
        }

        Debug.LogError("ERROR: Game level not set");
        return " ";
    }
}
