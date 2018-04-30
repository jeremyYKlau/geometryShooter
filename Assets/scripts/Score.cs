using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {
    
    public static int score { get; private set; }
    float lastEnemyKilledTIme;
    int streakCount;
    float streakEndTime = 2;

	void Start () {
        Enemy.onDeathStatic += onEnemyKilled;
        FindObjectOfType<Player>().onDeath += onPlayerDeath;
	}
	
    void onEnemyKilled()
    {
        if (Time.time < lastEnemyKilledTIme + streakEndTime)
        {
            streakCount++;
        }
        else
        {
            streakCount = 0;
        }
        lastEnemyKilledTIme = Time.time;
        score += 5 + (int)(Mathf.Pow(2, streakCount));
    }

    void onPlayerDeath()
    {
        Enemy.onDeathStatic -= onEnemyKilled; //unsubscribing to static event when player dies to avoid duplicates. Be careful and use this when using static events
    } 

	void Update () {
		
	}
}
