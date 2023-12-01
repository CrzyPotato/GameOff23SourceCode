using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }
    }

    [Header("References")]
    // [SerializeField] private GameObject readyGameIndicators; // ex: text telling the player to ready up
    [SerializeField] private AudioSource masterAudioSource;
    [SerializeField] private AudioClip victorySFX;
    [SerializeField] private AudioClip defeatSFX;
    [SerializeField] private TMP_Text readyGameText;
    [SerializeField] private TMP_Text finishGameText;
    [SerializeField] private GameObject playerHud;
    [SerializeField] private GameObject victoryRibbon;
    [SerializeField] private GameObject defeatRibbon;
    // [SerializeField] private GameObject postLevelScreen;
    private WaveManager waveManager;

    [Header("Bools")]
    public bool hasGameStarted;
    public bool hasGameEnded;
    public bool hasLevelCompleted; // Might be checked somewhere else when implementing saving

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void OnEnable()
    {
        LandMark.OnDeath += EnemyWins;
        WaveManager.OnPlayerWin += PlayerWins;
    }

    private void OnDisable()
    {
        LandMark.OnDeath -= EnemyWins;
        WaveManager.OnPlayerWin -= PlayerWins;
    }

    void Start()
    {
        waveManager = WaveManager.Instance;

        playerHud.SetActive(true);
        // postLevelScreen.SetActive(false);
        victoryRibbon.SetActive(false);
        defeatRibbon.SetActive(false);

        hasGameStarted = false;
        hasGameEnded = false;

        readyGameText.enabled = true;
        finishGameText.enabled = false;
    }

    // What should happen at the beginning of a level?
    public void LevelStart()
    {
        if (hasGameStarted)
            return;

        hasGameStarted = true;

        readyGameText.enabled = false;
        waveManager.timerIsRunning = true; // Start spawning
    }

    // Game end logic if player wins
    public void PlayerWins()
    {
        UnlockNewLevel();

        // Play victory sfx or music
        masterAudioSource.PlayOneShot(victorySFX);
        // VFX? Confetti?
        victoryRibbon.SetActive(true);
        // Post game screen
        finishGameText.enabled = true;
        playerHud.SetActive(false);
        hasGameEnded = true;
    }

    // Game end logic if enemy wins
    public void EnemyWins()
    {
        // Play lose sfx
        masterAudioSource.PlayOneShot(defeatSFX);
        defeatRibbon.SetActive(true);
        // Post game screen
        finishGameText.enabled = true;
        playerHud.SetActive(false);
        hasGameEnded = true;
    }

    public void ReturnToMap()
    {
        if (hasGameEnded)
            SceneManager.LoadScene("Map");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void UnlockNewLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex >= PlayerPrefs.GetInt("ReachedIndex"))
        {
            PlayerPrefs.SetInt("ReachedIndex", SceneManager.GetActiveScene().buildIndex + 1);
            PlayerPrefs.SetInt("UnlockedLevel", PlayerPrefs.GetInt("UnlockedLevel", 1) + 1);
            PlayerPrefs.Save();
        }
    }
}
