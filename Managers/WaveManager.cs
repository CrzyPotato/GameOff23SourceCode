using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Serialization;
using TMPro;

// NOTE: Sub-wave refers to naturally spawned enemies in between big waves
// NOTE: Big-wave refers to big waves before the final wave
// NOTE: The final wave is the final big wave of the game. We stop spawning after this.
// TODO: Make wave durations vary maybe?
// TODO: Keep track of wave spawning progress up until the final wave.

public class WaveManager : MonoBehaviour
{
    private static WaveManager _instance;
    // private GameManager gameManager;
    public static WaveManager Instance
    {
        get { return _instance; }
    }

    // Events
    public delegate void WinEvent();
    public static event WinEvent OnPlayerWin;

    [Header("Lists")]
    public List<EnemySO> enemies;
    public List<GameObject> enemiesToSpawn = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();
    public int enemiesAlive;

    [Header("Wave Parameters")]
    public int currWave;
    [Tooltip("The total amount of waves for this level. Once it reaches the max wave, the game will trigger the final wave and stop running.")]
    public int totalWaves = 10;
    [FormerlySerializedAs("subWaveDuration")] [Tooltip("Waves are when enemies spawn (excluding big/final waves)")] public int waveDuration = 20;
    public int startingWaveValue = 1;
    [Tooltip("After every x amount of waves, increase the wave value.")] public int waveValueIncreaseInterval = 3;
    [Tooltip("How much should the wave value be increased?")] public int waveValueIncreaseAmount = 3;

    [Header("Misc")] 
    public Transform finalEnemyPos;
    private Transform chosenSpawnpoint;

    [Header("UI References")]
    [SerializeField] private TMP_Text countdownText;
    public bool timerIsRunning;
    private float timeRemaining; // Time remaining before first wave

    private int waveValue;
    [SerializeField] private int waveIntervalValue; // The wave value for the current set of sub-waves
    private int waveValueCheck;
    private float[] weights;
    private float waveTimer;
    private float spawnInterval;
    private float spawnTimer;
    private bool isFinalWave;
    private bool canSpawn;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else 
            _instance = this;
    }

    private void OnEnable()
    {
        LandMark.OnDeath += StopSpawning;
    }

    private void OnDisable()
    {
        LandMark.OnDeath -= StopSpawning;
    }

    private void Start()
    {
        isFinalWave = false;
        canSpawn = false;
        timeRemaining = 6f;
        countdownText.enabled = false;
    }

    private void Update()
    {
        StartCountdown();
    }

    // FixedUpdate for more accurate timer
    void FixedUpdate()
    {
        if (canSpawn == false)
            return;

        /*if (enemiesAlive.Count == 1 && isFinalWave)
            finalEnemyPos = enemiesAlive[0].transform;*/

        // Player wins the game
        if (enemiesAlive == 0 && isFinalWave && enemiesToSpawn.Count == 0)
        {
            canSpawn = false;
            OnPlayerWin.Invoke();
        }

        // If all enemies from last wave are dead before the wave timer ends, start the next wave immediately.
        // FIX
        /*if (enemiesAlive.Count == 0)
        {
            waveTimer = 0;
        }*/
        
        if (enemiesToSpawn.Count > 0)
        {
            Vector3 spawnPos = chosenSpawnpoint.transform.position;
            spawnPos.x += Random.Range(-5, 5);
            spawnPos.z += Random.Range(-5, 5);

            GameObject newEnemy = ObjectPoolManager.SpawnObject(enemiesToSpawn[0], spawnPos, Quaternion.identity, ObjectPoolManager.PoolType.Enemy); // Spawn enemy at random spawn point
            enemiesAlive++;
            enemiesToSpawn.RemoveAt(0);
        }
            
        if (currWave != totalWaves/* && !isFinalWave*/) // If this isn't the last wave, spawn next standard wave
        {
            // If sub-wave is still active, wait.
            if (waveTimer > 0)
            {
                waveTimer -= Time.fixedDeltaTime;
            }
            else if (waveTimer <= 0) // Once the sub-wave is over, we start a new one and send a few more enemies
            {
                Debug.Log("Wave Generated");
                GenerateWave();
            }
        }
    }

    private void StartFirstWave()
    {
        waveTimer = waveDuration;
        waveValue = startingWaveValue;
        waveIntervalValue = waveValue;
        GenerateWave();
        canSpawn = true;
    }

    private void GenerateWave()
    {
        currWave++;
        waveValueCheck++;

        if (waveValueCheck >= waveValueIncreaseInterval)
        {
            waveIntervalValue += waveValueIncreaseAmount;
            waveValueCheck = 0; // Resets back to 0, wait for next interval amount of waves
        }

        if (currWave == totalWaves)
        {
            isFinalWave = true;
        }

        waveValue = waveIntervalValue;

        chosenSpawnpoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GenerateEnemies();
        
        waveTimer = waveDuration;
    }

    public void GenerateEnemies()
    {
        // Create a temporary list of enemies to generate.
        // In a loop, grab a random enemy and see if we can afford it.
        // If we can, add it to our list, and deduct the cost.

        // Repeat...

        // -> If we have no points left, leave the loop
        List<GameObject> generatedEnemies = new List<GameObject>();
        while (waveValue > 0)
        {
            int randEnemyId = Random.Range(0, enemies.Count);
            int randEnemyCost = enemies[randEnemyId].spawnCost;
            if (waveValue - randEnemyCost >= 0)
            {
                generatedEnemies.Add(enemies[randEnemyId].enemyPrefab);
                waveValue -= randEnemyCost;
            }
            else if (waveValue <= 0)
            {
                break;
            }
        }
        enemiesToSpawn.Clear();
        enemiesToSpawn = generatedEnemies;
    }

    private void StartCountdown()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                if (!countdownText.enabled)
                    countdownText.enabled = true;

                timeRemaining -= Time.deltaTime;
                countdownText.text = "Enemies arrive in: " + Mathf.FloorToInt(timeRemaining).ToString();
            }
            else
            {
                countdownText.enabled = false;
                timerIsRunning = false;
                timeRemaining = 0;
                // Wave start sfx
                StartFirstWave();
            }
        }
    }

    private void StopSpawning()
    {
        canSpawn = false;
    }
}
