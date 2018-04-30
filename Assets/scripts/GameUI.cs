using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public Image fadeScreen;
    public GameObject gameOverUI;

    public RectTransform waveBanner;
    public Text waveTitle;
    public Text waveEnemyCount;
    public Text scoreUI;
    public Text gameOverScoreUI;
    public RectTransform healthBar;

    Spawner spawner;
    Player player;

	// Use this for initialization
	void Start () {
        player = FindObjectOfType<Player>();
        player.onDeath += onGameOver; //the += onGameOver is subscribing to the event of player onDeath event
	}

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.onNewWave += onNewWave;
    }

    void Update()
    {
        scoreUI.text = Score.score.ToString("D6");
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player.health / player.startHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }
    void onNewWave(int waveNum)
    {
        waveTitle.text = "-Wave " + waveNum + " -";
        string enemyCountString = ((spawner.waves[waveNum - 1].infinite)? "Infinite" : spawner.waves[waveNum - 1].enemyCount + "");
        waveEnemyCount.text = "Enemies: " + enemyCountString;

        StopCoroutine("AnimateWaveBanner");
        StartCoroutine("AnimateWaveBanner");
    }
	
    void onGameOver()
    {
        Cursor.visible = true;
        StartCoroutine(Fade(Color.clear, new Color(0,0,0,.9f), 1));
        gameOverScoreUI.text = scoreUI.text;
        scoreUI.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadeScreen.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    IEnumerator AnimateWaveBanner()
    {
        float animationPercent = 0;
        float speed = 2.5f;
        float delayTime = 1.5f;
        int dir = 1;

        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * speed * dir;
            if(animationPercent >= 1)
            {
                animationPercent = 1;
                if(Time.time> endDelayTime)
                {
                    dir = -1;
                }
            }
            waveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-200, 45, animationPercent);
            yield return null;
        }
    }
    //Button Input
    public void startNewGame()
    {
        //Application.LoadLevel("Game"); //obsolete version of code below
        SceneManager.LoadScene("Game");
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
