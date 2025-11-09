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

    // Valor de puntaje por eliminacion de enemigos
    public Text ScoreText;
    public int CurrentScore = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        ammoText.text = gunAmmo.ToString();
        healthText.text = health.ToString();
        
        ScoreText.text = CurrentScore.ToString();

        
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

    // Metodo para terminar el nivel
    public void LevelComplete()
    {
        Debug.Log("GANASTE! Escapando del mapa.");

        PlayerPrefs.SetInt("FinalPlayerScore", CurrentScore);
        PlayerPrefs.Save();

        LoadingScreenManager.LoadScene("GameVictoryScene");
    }
}
