using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    public int maxHealth = 100;

    // Valor de puntaje por eliminacion de enemigos
    public Text ScoreText;
    public int CurrentScore = 0;

    // Indice del siguiente nivel
    private int nextSceneIndex = 0;

    private void Awake()
    {
        Instance = this;

        //Time.timeScale = 1f;

        // Inicializar la salud máxima en el gestor de efectos
        if (HealthColorEffect.Instance != null)
        {
            HealthColorEffect.Instance.maxHealth = maxHealth;
            HealthColorEffect.Instance.ResetSaturation();
        }

        // Obtener el Indice del siguiente nivel
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        nextSceneIndex = currentSceneIndex + 1;

        // Asignar el valor de la puntuacion del nivel anterior
        CurrentScore = PlayerPrefs.GetInt("FinalPlayerScore", 0);
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

    // Metodo para recuperar salud
    public void GainHealth(int amount)
    {
        health += amount;

        if (health > maxHealth) health = maxHealth;

        CheckHealth();
    }

    // Comprobar la vida del Player
    public void CheckHealth()
    {
        if (HealthColorEffect.Instance != null)
        {
            HealthColorEffect.Instance.UpdateSaturation(health);
        }

        if (health <= 0)
        {
            // Guardar el nombre de la escena que el jugador se encontraba actualmente
            string currentSceneName = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("LastPlayedScene", currentSceneName);
            PlayerPrefs.SetInt("FinalPlayerScore", 0);
            PlayerPrefs.Save();

            Debug.Log("GAME OVER.");

            if (HealthColorEffect.Instance != null)
            {
                HealthColorEffect.Instance.UpdateSaturation(0);
            }

            StartCoroutine(LoadGameOverWithDelay(1.0f));
        }
        else if (health > maxHealth)
        {
            health = maxHealth; // Lógica para evitar sobrecuración
        }
    }

    // Metodo para terminar el nivel
    public void LevelComplete()
    {
        Debug.Log("GANASTE! Escapando del mapa.");

        string nextSceneName = "";

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            nextSceneName = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(nextSceneIndex));

            PlayerPrefs.SetInt("FinalPlayerScore", CurrentScore);

            PlayerPrefs.SetString("NextSceneToLoad", nextSceneName);
            PlayerPrefs.Save();

            LoadingScreenManager.LoadScene(nextSceneName);
            //SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            PlayerPrefs.SetInt("FinalPlayerScore", CurrentScore);
            PlayerPrefs.Save();
            LoadingScreenManager.LoadScene("GameVictoryScene");
            //SceneManager.LoadScene("GameVictoryScene");
        }

        

        //SceneManager.LoadScene("LevelComplete");
    }

    // Metodo para borrar e inicializar todos los datos al iniciar el juego
    public void ResetGameSession()
    {
        CurrentScore = 0;

        health = 100;
        gunAmmo = 50;

        PlayerPrefs.SetInt("FinalPlayerScore", 0);
        PlayerPrefs.DeleteKey("LastPlayedScene");
        PlayerPrefs.Save();
    }

    // Metodo para cargar la escena de Game Over con un tiempo de espera
    private IEnumerator LoadGameOverWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Time.timeScale = 1f;

        SceneManager.LoadScene("GameOverScene");
    }
}
