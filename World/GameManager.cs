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
        EnemyKillText.text = EnemyKillCount.ToString();
        CrystalText.text = CrystalCount.ToString();
        ScoreText.text = CurrentScore.ToString();

        CheckChallenges();
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
}
