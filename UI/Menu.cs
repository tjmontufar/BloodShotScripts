using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public enum ConfirmationAction { None, RestartLevel, QuitToMenu }

public class Menu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject menuPausePanel;
    public GameObject confirmExitPanel;

    private bool isGamePaused = false;
    private ConfirmationAction pendingAction = ConfirmationAction.None;

    public SceneLoader sceneLoader;
    public TextMeshProUGUI ConfirmationTextPanel;

    [Header("Audio Mixer Snapshots")]
    public AudioMixer mainMixer;
    public AudioMixerSnapshot unpausedSnapshot; // Arrastra el Snapshot 'Unpaused' aquí
    public AudioMixerSnapshot pausedSnapshot;   // Arrastra el Snapshot 'Paused' aquí

    private const float transitionTime = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pausePanel.SetActive(false);

        // Establecer volumen normal al iniciar el nivel
        if (unpausedSnapshot != null)
        {
            unpausedSnapshot.TransitionTo(transitionTime);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }  
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Silenciar efectos de sonido
        if (pausedSnapshot != null)
        {
            // Transiciona al Snapshot que tiene el volumen de SFX bajado a -80dB
            pausedSnapshot.TransitionTo(transitionTime);
            Debug.Log("Transicionando a estado Pausado (Audio Silenciado).");
        }

        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        // Restaurar sonidos
        if (unpausedSnapshot != null)
        {
            // Transiciona de vuelta al Snapshot normal, restaurando los volúmenes configurados por el usuario.
            unpausedSnapshot.TransitionTo(transitionTime);
            Debug.Log("Transicionando a estado Normal (Audio Restaurado).");
        }

        ButtonHover.ResetAllHovers();

        isGamePaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pausePanel.SetActive(false);
    }

    public void OpenOptionsPanel()
    {
        ButtonHover.ResetAllHovers();

        menuPausePanel.SetActive(false);
        optionsPanel.SetActive(true);

        if (pausedSnapshot != null)
        {
            pausedSnapshot.TransitionTo(transitionTime);
        }
    }

    public void openMenuPausePanel()
    {
        ButtonHover.ResetAllHovers();

        optionsPanel.SetActive(false);
        confirmExitPanel.SetActive(false);
        menuPausePanel.SetActive(true);

        if (pausedSnapshot != null)
        {
            pausedSnapshot.TransitionTo(transitionTime);
        }
    }

    public void AskToRestart()
    {
        pendingAction = ConfirmationAction.RestartLevel;
        OpenConfirmPanel("¿SEGURO QUE QUIERES REINICIAR EL NIVEL?");
    }

    public void AskToQuit()
    {
        pendingAction = ConfirmationAction.QuitToMenu;
        OpenConfirmPanel("¿SEGURO QUE QUIERES VOLVER AL MENÚ?");
    }

    public void OpenConfirmPanel(string askText)
    {
        ButtonHover.ResetAllHovers();
        menuPausePanel.SetActive(false);
        confirmExitPanel.SetActive(true);
        ConfirmationTextPanel.text = askText;
    }

    public void CancelConfirmation()
    {
        pendingAction = ConfirmationAction.None;
        openMenuPausePanel();
    }

    public void ExecutePendingAction()
    {
        switch (pendingAction)
        {
            case ConfirmationAction.RestartLevel:
                sceneLoader.RestartLevel();
                break;

            case ConfirmationAction.QuitToMenu:
                sceneLoader.GoToMainMenu();
                break;

            case ConfirmationAction.None:
            default:
                Debug.LogWarning("Confirmación pulsada sin acción pendiente. Revisa la UI.");
                CancelConfirmation();
                break;
        }

        pendingAction = ConfirmationAction.None;
    }
}
