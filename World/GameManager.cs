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

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        ammoText.text = gunAmmo.ToString();
        healthText.text = health.ToString();
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
            Debug.Log("GAME OVER.");

            RestartLevel();
        }
    }

    // Metodo para reiniciar el nivel al presionar el boton
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);

        Debug.Log("Haciendo clic en el boton de reiniciar.");
    }
    
    // Metodo para volver al menu principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 

        Debug.Log("Volviendo al menú principal.");
    }
}
