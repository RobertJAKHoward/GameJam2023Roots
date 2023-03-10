using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Canvas UI;
    public TextMeshProUGUI CountDown;
    public TextMeshProUGUI Timer;
    public GameObject TutorialContainer;
    public GameObject Player;

    public enum gameState { Tutorial, Playing, Lost, Won }
    public gameState curState = gameState.Tutorial;
    public float gameTimer;
    public float tutorialTimer = 5;
    public gameDifficulty curDifficulty;
    private float endTimer = 5;
    private MasterGameController MasterController;
    public GameObject killbox;

    private AudioSource[] music;

    // Start is called before the first frame update
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
            gameTimer = 20;
        }
        else if (curDifficulty == gameDifficulty.Medium)
        {
            gameTimer = 30;
        }
        else
        {
            gameTimer = 45;
        }
        music = GetComponents<AudioSource>();
        music[1].PlayDelayed(music[0].clip.length);
    }

    void Update()
    {
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
            curState = gameState.Playing;
            Player.gameObject.GetComponent<PlayerMovement>().startMoving = true;
        }
        else if (gameTimer > 0 && curState == gameState.Playing)
        {
            gameTimer -= Time.deltaTime;
            Timer.text = Mathf.RoundToInt(gameTimer).ToString();
        }
        else if (curState == gameState.Playing && gameTimer <= 0)
        {
            killbox.SetActive(false);
            Player.GetComponent<CapsuleCollider>().enabled = false;
            curState = gameState.Won;
            UI.GetComponent<Spudacus_UI>().LoadVictoryScreen();
        }
        else if ((curState == gameState.Lost || curState == gameState.Won) && endTimer > 0)
        {
            endTimer -= Time.deltaTime;
        }
        else if ((curState == gameState.Lost || curState == gameState.Won) && endTimer <= 0)
        {
            if (MasterController && curState == gameState.Won)
            {
                MasterController.gamesPlayed++;
                MasterController.timesPlayed["SwingingVine"]++;
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
