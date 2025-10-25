using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Metodo para reiniciar el nivel al presionar el boton
    public void RestartLevel()
    {
        Time.timeScale = 1f;

        // Si no se encontro una escena guardada, el Nivel 1 sera la escena predeterminada
        string lastScene = PlayerPrefs.GetString("LastPlayedScene", "Nivel_1");

        //SceneManager.LoadScene(lastScene);
        LoadingScreenManager.LoadScene(lastScene);

        Debug.Log("Escena cargada al reiniciar nivel: " + lastScene);
    }

    // Metodo para volver al menu principal
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");

        Debug.Log("Volviendo al menú principal.");
    }
}
