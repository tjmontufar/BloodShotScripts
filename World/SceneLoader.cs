using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Metodo para reiniciar el nivel al presionar el boton
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        string sceneToLoad = "";

        // Si el jugador murio, cargar la ultima escena antes de morir
        if(SceneManager.GetActiveScene().name == "GameOverScene")
        {
            sceneToLoad = PlayerPrefs.GetString("LastPlayedScene", "MainMenu");

            PlayerPrefs.DeleteKey("LastPlayedScene");
            PlayerPrefs.Save();
        }
        else // Caso contrario, reiniciar el nivel actual
        {
            sceneToLoad = SceneManager.GetActiveScene().name;

            // Restablecer los valores de salud y municion
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetGameSession();
            }
        }

        LoadingScreenManager.LoadScene(sceneToLoad);

        Debug.Log("Escena cargada al reiniciar nivel: " + sceneToLoad);
    }

    // Metodo para reiniciar el juego hasta el primer nivel
    public void RestartGame()
    {
        Time.timeScale = 1f;

        // Restablecer los valores de salud y municion
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameSession();
        }
        else
        {
            PlayerPrefs.SetInt("FinalPlayerScore", 0);
            PlayerPrefs.DeleteKey("LastPlayedScene");
            PlayerPrefs.DeleteKey("NextSceneToLoad");
            PlayerPrefs.Save();
        }

        LoadingScreenManager.LoadScene("Nivel_1");
    }

    // Metodo para volver al menu principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        // Restablecer los valores de salud y municion
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGameSession();
        }
        else
        {
            PlayerPrefs.SetInt("FinalPlayerScore", 0);
            PlayerPrefs.DeleteKey("LastPlayedScene");
            PlayerPrefs.DeleteKey("NextSceneToLoad");
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene("MainMenu");

        Debug.Log("Volviendo al menú principal.");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        string nextLevel = PlayerPrefs.GetString("NextSceneToLoad", "GameVictoryScene");
        LoadingScreenManager.LoadScene(nextLevel);
    }
}
