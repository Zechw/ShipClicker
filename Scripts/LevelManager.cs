using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int score;
    private int highScore;

    public Canvas levelUI;
    [HideInInspector] public Canvas activeUI;

    public Transform starField;
    private Transform activeStarField;

    public Transform playerShip;
    [HideInInspector] public Transform activePlayer;

    public Transform asteroid;

    public Transform enemy;
    private bool enemiesAreSpawning = false;

    public Transform boss;
    private Transform activeBoss;

    public AudioSource laserSound;
    public AudioSource boomSound;

    private bool isRunning = true;

	void Start () {
        activeUI = Instantiate(levelUI);
        activeUI.transform.Find("Play Again").GetComponent<Button>().onClick.AddListener(startGame);

        activeStarField = Instantiate(starField);
        activeStarField.GetComponent<StarFieldController>().activeLevel = this;
        
        InvokeRepeating("SpawnAsteroid", 0, 2);

        startGame();
    }
	
	void Update () {
		if (activePlayer == null && isRunning)
        {
            endGame(false);
        }

        if (score > 50 && score < 300)
        {
            activeUI.transform.Find("Tutorial").GetComponent<Text>().text = "Your shields will absorb lasers, as long as you have enough power.";
        }
        else if (score > 300)
        {
            activeUI.transform.Find("Tutorial").GetComponent<Text>().text = "Large Enemy Incoming!!";
        }

        if (score > 100 && enemiesAreSpawning == false && isRunning)
        {
            InvokeRepeating("SpawnEnemy", 2, 4);
            enemiesAreSpawning = true;
        }

        if (score > 300 && activeBoss == null && isRunning)
        {
            activeBoss = Instantiate(boss, new Vector3(30, 20, 0), Quaternion.Euler(0, 0, -90));
            activeBoss.GetComponent<BossController>().activeLevel = this;
        }
	}

    public void startGame()
    {
        activeUI.transform.Find("Tutorial").GetComponent<Text>().text = "Click on your ship to build power. Use that power to fire your weapons. \n Move with WASD";

        score = 0;
        enemiesAreSpawning = false;

        if (activePlayer == null) SpawnPlayer();

        activeUI.transform.Find("Play Again").gameObject.SetActive(false);

        isRunning = true;
    }

    public void endGame(bool victory)
    {
        isRunning = false;
        if (score > highScore) highScore = score;

        CancelInvoke("SpawnEnemy");

        string victoryText = victory ? "Congratulations!" : "Game Over!";

        activeUI.transform.Find("Scoreboard").GetComponent<Text>().text = victoryText + "\n Final Score: " + score.ToString() + "\n High Score: " + highScore.ToString();
        StartCoroutine(DelayedCleanup());
    }

    IEnumerator DelayedCleanup()
    {
        yield return new WaitForSeconds(1);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Destroy(enemies[i]);
        }

        activeUI.transform.Find("Play Again").gameObject.SetActive(true);
    }

    public void AddScore(int amount)
    {
        if (!isRunning) return;
        score += amount;
        activeUI.transform.Find("Scoreboard").GetComponent<Text>().text = "Score: " + score.ToString();
    }

    private void SpawnPlayer()
    {
        activePlayer = Instantiate(playerShip, new Vector3(-20, 0, 0), Quaternion.identity);
        activePlayer.GetComponent<ShipController>().activeLevel = this;
        activePlayer.GetComponent<ShipController>().target = activeUI.transform.Find("Crosshairs");
    }

    private void SpawnEnemy()
    {
        if (!isRunning) return;

        //pick a random spot just off camera
        float xloc;
        float yloc;
        if (Random.value < .5)
        {//top/bottom
            //Only spawn on right half 
            xloc = Random.Range(0, 30);
            yloc = 17 * (Random.value < .5 ? 1 : -1);
        } else
        {//left/right
            xloc = 29; //Only spawn on right half // * (Random.value < .5 ? 1 : -1);
            yloc = Random.Range(-16, 16);
        }
        Transform newDude = Instantiate(enemy, new Vector3(xloc, yloc), Quaternion.identity);
        newDude.GetComponent<EnemyController>().activeLevel = this;
    }

    private void SpawnAsteroid()
    {
        if (!isRunning) return;

        Transform newAsteroid = Instantiate(asteroid, new Vector3(30, Random.Range(-18, 18)), Quaternion.identity);
        newAsteroid.GetComponent<AsteroidController>().activeLevel = this;
    }
}
