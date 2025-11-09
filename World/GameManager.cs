using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Texto de la municion restante en pantalla
    public Text ammoText;
    public static GameManager Instance { get; private set; }
    // Valor inicial de la municion
    public int gunAmmo = 10;

    // Texto de la salud del jugador en pantalla
    public Text healthText;
    // Valor inicial de la salud
    public int health = 100;

    // Contador de enemigos eliminados
    public Text EnemyKillText;
    public int EnemyKillCount = 0;

    // Valor de puntaje por eliminacion de enemigos
    public Text ScoreText;
    public int CurrentScore = 0;

    // Contador de cristales recogidos
    public Text CrystalText;
    public int CrystalCount = 0;

    // Variables para los objetivos del nivel (se ajustan en las escenas)
    public int EnemyKillChallenge = 0;
    public int CrystalCollectChallenge = 0;
    private bool challengesCompleted = false;

    // Textos en pantalla para mostrar los objetivos que el jugador debe realizar
    public GameObject Quest1Text;
    public GameObject Quest2Text;
    public GameObject Quest3Text;

    private int currentQuestIndex = 0;

    private bool dynamiteUsed = false;

    // Variables para el temporizador de escape
    public Text timerText;
    public float maxEscapeTime = 60f;
    private float currentEscapeTime;
    private bool isTimerRunning = false;

    // Llamar el Script para manejo del descenso del helicoptero al lograr las metas
    public HelicopterEscape helicopterScript;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        ammoText.text = gunAmmo.ToString();
        healthText.text = health.ToString();
        EnemyKillText.text = EnemyKillCount.ToString() + " / " + EnemyKillChallenge.ToString();
        CrystalText.text = CrystalCount.ToString() + " / " + CrystalCollectChallenge.ToString();
        ScoreText.text = CurrentScore.ToString();

        CheckChallenges();
        CheckQuestProgression();

        if(isTimerRunning)
        {
            currentEscapeTime -= Time.deltaTime;

            DisplayTime(currentEscapeTime);

            if(currentEscapeTime <= 0)
            {
                currentEscapeTime = 0;
                isTimerRunning = false;
                GameOverByTimeOut();
            }
        }
    }

    // Metodo para perder vida
    public void LoseHealth(int healthToReduce)
    {
        health -= healthToReduce;
        CheckHealth();
    }

    // Comprobar la vida del Player
    public void CheckHealth()
    {
        if (health <= 0)
        {
            // Guardar el nombre de la escena que el jugador se encontraba actualmente
            string currentSceneName = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("LastPlayedScene", currentSceneName);
            PlayerPrefs.Save();

            Debug.Log("GAME OVER.");

            Time.timeScale = 1f;
            SceneManager.LoadScene("GameOverScene");

            //RestartLevel();
        }
    }

    // Metodo para mostrar tareas por cumplir en pantalla
    public void CheckQuestProgression()
    {
        switch (currentQuestIndex)
        {
            case 0:
                Quest1Text.SetActive(true);
                // Mision 1: Matar enemigos
                if (EnemyKillCount >= EnemyKillChallenge)
                {
                    Quest1Text.SetActive(false);
                    currentQuestIndex++;
                    CheckQuestProgression();
                }
                break;
            case 1:
                // Mision 2: Usar la dinamita y recoger los cristales
                Quest2Text.SetActive(true);

                if(CrystalCount >= CrystalCollectChallenge && dynamiteUsed)
                {
                    Quest2Text.SetActive(false);
                    currentQuestIndex++;
                    CheckQuestProgression();

                    currentEscapeTime = maxEscapeTime;
                    isTimerRunning = true;
                    timerText.gameObject.SetActive(true);
                }
                break;
            case 2:
                Quest3Text.SetActive(true);
                break;
        }
    }

    // Metodo para comprobar que haya detonado la dinamita
    public void DynamiteActionCompleted()
    {
        if (!dynamiteUsed)
        {
            dynamiteUsed = true;
            CheckQuestProgression();
        }
    }

    // Metodo para verificar si se han cumplido las metas
    public void CheckChallenges()
    {
        if (challengesCompleted)
        {
            return;
        }

        if(EnemyKillCount >= EnemyKillChallenge && CrystalCount >= CrystalCollectChallenge)
        {
            challengesCompleted = true;

            Debug.Log("Cumpliste la meta. (Avanza el siguiente nivel)");

            if (helicopterScript != null)
            {
                helicopterScript.StartDescent();
            }
        }
    }

    // Metodo para terminar el nivel
    public void LevelComplete()
    {
        Debug.Log("GANASTE! Escapando del mapa.");

        PlayerPrefs.SetInt("FinalPlayerScore", CurrentScore);
        PlayerPrefs.Save();

        LoadingScreenManager.LoadScene("GameVictoryScene");
    }

    // Metodo para formatear y mostrar el tiempo
    void DisplayTime(float timeToDisplay)
    {
        // Asegura que no se muestren números negativos
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        // Calcula los minutos y segundos (formato MM:SS)
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Metodo para cargar pantalla de Game Over si el tiempo se ha agotado
    void GameOverByTimeOut()
    {
        Debug.Log("¡Tiempo agotado!");

        Time.timeScale = 0f;

        SceneManager.LoadScene("GameOverScene");
    }
}
