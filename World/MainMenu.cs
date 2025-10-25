using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;

    public void OpenOptionsPanel()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
        
    }
    public void OpenMainMenuPanel()
    {
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
        
    }

    public void OpenCreditsMenuPanel()
    {
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
        LoadingScreenManager.LoadScene("Nivel_1");
    }
}
