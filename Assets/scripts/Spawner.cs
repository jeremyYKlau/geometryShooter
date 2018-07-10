using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public bool developerMode;

    public Wave[] waves; //list of waves
    public Enemy enemy;
    public Enemy[] enemies;
    public Enemy boss;

    //I want 4 types of enemies prefered or at least 3 but idk how to spawn them. Especially since i don't want them randomly spawning

    Character player;
    Transform playerT;

    Wave currentWave;
    int currentWaveNum;

    int enemiesKilled;//used to get health for kills for the player
    int enemiesToSpawn; //amount of enemies to spawn
    int enemiesAlive; //count to see how many enemies are alive if it hits 0 spawn next wave

    int bossNum = 0;//count to make sure no boss is spawned more then once
    float spawnTime; //time between each enemy spawn

    MapGenerator map;

    bool isDisabled;

    public event System.Action<int> onNewWave;

    void Start()
    {
        player = FindObjectOfType<Player>();
        playerT = player.transform;
        player.onDeath += onPlayerDeath;
        map = FindObjectOfType<MapGenerator>();
        nextWave(); 
    }

    void Update()
    {
        if (!isDisabled)
        {
            if ((currentWaveNum != 1) && ((currentWaveNum % 5) == 0) && (bossNum == 0))
            {
                Debug.Log("Here comes boss enemy");
                spawnBoss();
            }
            if ((enemiesToSpawn > 0 || currentWave.infinite) && Time.time > spawnTime)
            {
                enemiesToSpawn--;
                spawnTime = Time.time + currentWave.timeBetweenSpawn;

                StartCoroutine("SpawnEnemy");
            }
        }
        if (developerMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                nextWave();
            }
        }
    }

    void onPlayerDeath()
    {
        isDisabled = true;
    } 

    void onEnemyDeath ()
    {
        enemiesAlive --;
        enemiesKilled++;
        if(enemiesAlive == 0)
        {
            //set a delay timer hopefully in the future
            nextWave();
        }
        else if((enemiesKilled % 5 == 0) && (enemiesKilled != 0))
        {
            player.health +=(1);
        }
    }

    void resetPlayerPos()
    {
        playerT.position = map.getTileFromPos(Vector3.zero).position + Vector3.up * 2 ;
    }

    void nextWave()
    {
        if(currentWaveNum > 0)
        {
            AudioManager.instance.play2DSound("Level Complete");
        }
        currentWaveNum++;
        if (currentWaveNum - 1 < waves.Length) {
            currentWave = waves[currentWaveNum - 1];
            enemiesToSpawn = currentWave.enemyCount;
            enemiesAlive = enemiesToSpawn;

            if(onNewWave!= null)
            {
                onNewWave(currentWaveNum);
            }
        }
        bossNum = 0;
        resetPlayerPos();
        Debug.Log(currentWaveNum);
    }

    [System.Serializable]
	public class Wave
    {
        public bool infinite;

        public int enemyCount;
        public float timeBetweenSpawn;

        public float moveSpeed;
        public int damageToPlayer;
        public float enemyHealth;
    }

    void spawnBoss()
    {
        bossNum += 1;
        Transform spawnTile = map.getRandomOpenTile();
        Enemy spawnedBoss = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy; //just spawns a single enemy probably use for boss enemy
        spawnedBoss.onDeath += onEnemyDeath;
        spawnedBoss.setStats(currentWave.moveSpeed * 0.4f, currentWave.damageToPlayer, currentWave.enemyHealth * 5);
        enemiesAlive++;
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform spawnTile = map.getRandomOpenTile();

        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColour = tileMat.color;
        Color flashColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColour, flashColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }
        //spawns an enemy from a list of enemies at random
        Enemy spawnedEnemy = Instantiate(enemies[Random.Range(0,enemies.Length)], spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.onDeath += onEnemyDeath;
        spawnedEnemy.setStats(currentWave.moveSpeed, currentWave.damageToPlayer, currentWave.enemyHealth);
    }
}
