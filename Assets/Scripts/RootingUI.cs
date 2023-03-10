using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootingUI : MonoBehaviour
{
    public Canvas UI;
    public Canvas Stamina;
    public TextMeshProUGUI CountDown;
    public TextMeshProUGUI Timer;
    public GameObject TutorialContainer;
    public GameObject Pizza = null;
    public GameObject Player;
    public enum gameState { Tutorial, Playing, Lost, Won }
    public gameState curState = gameState.Tutorial;
    public float gameTimer;
    public float tutorialTimer = 5;
    public gameDifficulty curDifficulty;
    private float endTimer = 5;

    private MasterGameController MasterController;

    void Start()
    {
        GameObject MCObj = GameObject.FindGameObjectWithTag("MC");

        if (MCObj)
        {
            MasterController = MCObj.GetComponent<MasterGameController>();
            curDifficulty = MasterController.nextDifficulty;
        }
        else
            curDifficulty = gameDifficulty.Medium;

        if (curDifficulty == gameDifficulty.Easy)
        {
            gameTimer = 30;
        }
        else if (curDifficulty == gameDifficulty.Medium)
        {
            gameTimer = 20;
        }
        else
        {
            gameTimer = 15;
        }
    }

    void Update()
    {
        if (gameTimer > 0 && curState == gameState.Playing)
        {
            gameTimer -= Time.deltaTime;
            Timer.text = Mathf.RoundToInt(gameTimer).ToString();
        }
        if (Pizza == null) 
        {
            Pizza = GameObject.Find("Pizza1Prefab(Clone)");
        }
        if (tutorialTimer > 0 && curState == gameState.Tutorial)
        {
            tutorialTimer -= Time.deltaTime;
            CountDown.text = Mathf.RoundToInt(tutorialTimer).ToString();
        }
        else if (curState == gameState.Tutorial && tutorialTimer < 0)
        {
            CountDown.text = "0";
            CountDown.gameObject.SetActive(false);
            TutorialContainer.SetActive(false);
            Player.GetComponent<RootinRacoonScript>().play = true;
            curState = gameState.Playing;
        }
        else if (Pizza.GetComponent<RootinPizzaScript>().AmountOnPizza() <= .2f && curState == gameState.Playing)
        {
            Stamina.gameObject.SetActive(false);
            curState = gameState.Won;
            UI.GetComponent<Spudacus_UI>().LoadVictoryScreen();
        }
        else if (curState == gameState.Playing && gameTimer <= 0)
        {
            Stamina.gameObject.SetActive(false);
            curState = gameState.Lost;
            UI.GetComponent<Spudacus_UI>().LoadDeathScreen();
        }
        
        if ((curState == gameState.Lost || curState == gameState.Won) && endTimer > 0)
        {
            Player.GetComponent<RootinRacoonScript>().play = false;
            endTimer -= Time.deltaTime;
        }
        else if ((curState == gameState.Lost || curState == gameState.Won) && endTimer <= 0)
        {
            if (MasterController && curState == gameState.Won)
            {
                MasterController.gamesPlayed++;
                MasterController.timesPlayed["RootingAround"]++;
                SceneManager.LoadScene("TreeScene");
            }
            else
                SceneManager.LoadScene("MainMenu");
        }
    }

    public void GameOver()
    {
        curState = gameState.Lost;
        Timer.gameObject.SetActive(false);
        if (MasterController)
        {
            Destroy(GameObject.FindGameObjectWithTag("MenuMusic"));
            Destroy(MasterController.gameObject);
        }
        UI.GetComponent<Spudacus_UI>().LoadDeathScreen();
    }
}
