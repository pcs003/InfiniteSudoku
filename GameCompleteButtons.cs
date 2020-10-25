using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameCompleteButtons : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Handling of best time, current time, new best time popup, game complete menu popup
    //done in Timer.cs

    public void PlayAgain(string name)
    {
        GameEvents.OnPlayAgainMethod();
    }

    public void MainMenu(string name)
    {
        SceneManager.LoadScene(name);
    }

}
