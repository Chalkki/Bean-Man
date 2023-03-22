using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Controller : MonoBehaviour
{
    // set up the score board
    public TextMeshProUGUI ScoreLabel;
    public TextMeshProUGUI HighScoreLabel;
    public static int score;
    public static int highScore;
    // find the ghosts objects in the scene
    GameObject redGhost;
    GameObject blueGhost;
    GameObject orangeGhost;
    GameObject yellowGhost;

    // set up the respawn points
    [SerializeField] GameObject[] ghostRespawns;
    [SerializeField] GameObject playerRespawn;

    // set up the reference to prefabs
    [SerializeField] GameObject[] ghostsPrefabs;
    [SerializeField] GameObject playerPrefab;

    // set up the reference to player
    GameObject player;
    private bool initPlayer;
    // get the counts of coins and bigcoins
    private int megaCoins_Count;
    private int coins_Count;
    // set up the ghosts' frightenTime
    private float frightenTime;
    // start combo time for eating ghost
    private float startCT;

    // define how many ghost has been eaten in the vulnerable time
    private int ghostsCombo;

    // set up the current level and total level
    private int currentLevel;
    private const int totalLevel = 10;
    [SerializeField] private TextMeshProUGUI currentLevelGUI;

    // detect whether the player starts the game
    private bool startGame;
    [SerializeField] TextMeshProUGUI startCue;

    // set up the remaining lives and maximum lives allowed for the player
    private int remainedLives;
    [SerializeField] private TextMeshProUGUI remainedLivesGUI;
    private const int maxLives = 5;

    // audio manager
    AudioManager audioManager;
    // setting manager
    SettingsPopup popUP;
    private bool isSettingsOpen;
    void Start()
    {
        audioManager = GetComponent<AudioManager>();
        popUP = GetComponent<SettingsPopup>();
        isSettingsOpen = false;
        // set up the current Level
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        currentLevelGUI.text = currentLevel.ToString();
        if (currentLevel == 1) {
            remainedLives = 3;
        }
        else
        {
            remainedLives = PlayerPrefs.GetInt("RemainedLives");
        }
        remainedLivesGUI.text = remainedLives.ToString();
        Time.timeScale = 0f;
        startGame = false;
        startCue.gameObject.SetActive(true);
        ghostsCombo = 0;
        startCT = 0;
        // show the highScore when the Level Generator starts
        highScore = PlayerPrefs.GetInt("HighScore");
        HighScoreLabel.text = highScore.ToString();

        initPlayer = false;
        frightenTime = 5f - 0.1f *(currentLevel - 1) * 5f;
        Debug.Log($"The current level is {currentLevel} and the frightened Time is {frightenTime}");
        ScoreLabel.text = score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && startGame)
        {
            if(!isSettingsOpen)
            {
                popUP.Open();
            }
            else
            {
                popUP.Close();
            }
            isSettingsOpen = !isSettingsOpen;

        }
        if (Input.anyKey && !startGame)
        {
            startGame = true;
            StartGame();
        }
    }

    private void StartGame()
    {
        UseLive();
        Time.timeScale = 1f;
        startCue.gameObject.SetActive(false);
    }
    private void AddScore(int value)
    {
        score += value;
        ScoreLabel.text = score.ToString();
        if(score > highScore)
        {
            UpdateHighScore();
        }
    }

    void UpdateHighScore()
    {
        highScore = score;
        PlayerPrefs.SetInt("HighScore", highScore);
        HighScoreLabel.text = highScore.ToString();
    }
    public void PickUpCoin()
    {
        audioManager.PickCoin();

        AddScore(10);
        coins_Count--;
        CheckCoinCount();
    }

    public void PickUpBigCoin() {
        audioManager.PickBigCoin();

        AddScore(100);
        megaCoins_Count--;
        CheckCoinCount();
        //get each ghost
        GetGhosts();
        // set up each ghost's frighten mode
        if(redGhost != null)
        {
            redGhost.GetComponent<RedGhostMovement>().Frightened(frightenTime);
        }
        if (blueGhost != null)
        {
            blueGhost.GetComponent<BlueGhostMovement>().Frightened(frightenTime);
        }
        if (orangeGhost != null)
        {
            orangeGhost.GetComponent<OrangeGhostMovement>().Frightened(frightenTime);
        }
        if (yellowGhost != null)
        {
            yellowGhost.GetComponent<YellowGhostMovement>().Frightened(frightenTime);
        }
    }

    public void EatGhost()
    {
        audioManager.EatGhost();

        if (startCT != 0 && Time.time - startCT < frightenTime)
        {
            ghostsCombo += 1;
        }
        else
        {
            startCT = Time.time;
            ghostsCombo = 0;
        }




        int value =   200 * (int)Mathf.Pow(2, ghostsCombo);
        AddScore(value);
    }
    private void CheckCoinCount()
    {
        if(coins_Count == 0 && megaCoins_Count == 0)
        {
            StartCoroutine(WinGame());
        }
    }

    public void SetCoinCount()
    {
        coins_Count = GameObject.Find("Coins").transform.childCount;
        megaCoins_Count = GameObject.Find("MegaCoins").transform.childCount;
    }
    private void GetGhosts()
    {
        redGhost = GameObject.Find("redGhost(Clone)");
        blueGhost = GameObject.Find("blueGhost(Clone)");
        orangeGhost = GameObject.Find("orangeGhost(Clone)");
        yellowGhost = GameObject.Find("yellowGhost(Clone)");
    }
    private void GetPlayer()
    {
        player = GameObject.FindWithTag("Player");

    }
    public void GhostRespawn(GameObject ghost, int idx)
    {
        Destroy(ghost);
        Instantiate(ghostsPrefabs[idx], ghostRespawns[idx].transform.position, Quaternion.identity);
    }

    public void PlayerRespawn()
    {
        audioManager.Death();

        if (initPlayer)
        {
            return;
        }
        else
        {
            initPlayer = true;
        }
        GetPlayer();
        Destroy(player);
        if(remainedLives > 0)
        {
            startGame = false;
            Time.timeScale = 0f;
            startCue.gameObject.SetActive(true);
        }
        else
        {
            LostGame();
        }
        if (initPlayer && remainedLives > 0)
        {
            Instantiate(playerPrefab, playerRespawn.transform.position, Quaternion.identity);
            resetPos();
        }
        initPlayer = false;
    }

    private void UseLive()
    {
        Time.timeScale = 1f;
        remainedLives--;
        remainedLivesGUI.text = remainedLives.ToString();
    }
    private void resetPos()
    {
        // reset the ghost's position after the player is touched
        GetGhosts();
        Destroy(redGhost);
        Destroy(blueGhost);
        Destroy(yellowGhost);
        Destroy(orangeGhost);
        for (int i = 0; i<ghostRespawns.Length; i++)
        {
            Instantiate(ghostsPrefabs[i], ghostRespawns[i].transform.position, Quaternion.identity);
        }
    }

    IEnumerator WinGame()
    {
        audioManager.LevelComplete();
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);
        LoadMap();
    }
    
    private void LoadMap()
    {
        if (currentLevel < totalLevel)
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevel + 1);
            SceneManager.LoadScene("Map");
            Debug.Log($"Congradulations! You completed level {currentLevel}.");
            if (remainedLives + 1 < maxLives)
            {
                remainedLives += 2;
            }
            else
            {
                remainedLives++;
            }
            PlayerPrefs.SetInt("RemainedLives", remainedLives);

        }
        else
        {
            Debug.Log("Game Over, you won!");
        }
    }
    private void LostGame()
    {
        Time.timeScale = 0f;
        Debug.Log("You lost.");
        PlayerPrefs.SetInt("CurrentLevel", 1);
    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("CurrentLevel", 1);
        Debug.Log($"Application quited. Current level = {PlayerPrefs.GetInt("CurrentLevel")}");
    }

}
