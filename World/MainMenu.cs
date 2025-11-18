using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;

    public void OpenOptionsPanel()
    {
        ButtonHover.ResetAllHovers();
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        
    }
    public void OpenMainMenuPanel()
    {
        ButtonHover.ResetAllHovers();
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
        
    }

    public void OpenCreditsMenuPanel()
    {
        ButtonHover.ResetAllHovers();
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);

    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void PlayGame()
    {
        // Restablecer los valores de salud y municion
        PlayerPrefs.SetInt("FinalPlayerScore", 0);
        PlayerPrefs.Save();

        LoadingScreenManager.LoadScene("Nivel_1");
    }
}
