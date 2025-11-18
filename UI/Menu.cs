using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public enum ConfirmationAction { None, RestartLevel, QuitToMenu }

public class Menu : MonoBehaviour
{
    [Header("Referencias de Paneles")]
    public GameObject pausePanel;
    public GameObject optionsPanel;
    public GameObject menuPausePanel;
    public GameObject confirmExitPanel;

    [Header("Referencias de Logica")]
    public SceneLoader sceneLoader;
    public TextMeshProUGUI ConfirmationTextPanel;
    public CameraLook cameraLook; // Para solucionar el bug de la camara
    public PlayerMovement playerMovement; // Para saber si se fuerza el modo movil

    [Header("Audio Mixer Snapshots")]
    public AudioMixer mainMixer;
    public AudioMixerSnapshot unpausedSnapshot;
    public AudioMixerSnapshot pausedSnapshot;

    private bool isGamePaused = false;
    private ConfirmationAction pendingAction = ConfirmationAction.None;
    private const float transitionTime = 0.0f;

    void Start()
    {
        // El juego siempre empieza sin pausa
        isGamePaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        if (cameraLook != null)
        {
            cameraLook.enabled = true;
        }

        // Configurar el cursor y el audio para el estado inicial
        if (!IsMobileContext())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (unpausedSnapshot != null)
        {
            unpausedSnapshot.TransitionTo(transitionTime);
        }
    }

    void Update()
    {
        // Solo permitir pausar con la tecla 'Escape' en contexto de PC
        if (!IsMobileContext() && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    // Este es el metodo principal que deben llamar los botones de UI para pausar/reanudar
    public void TogglePause()
    {
        if (isGamePaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        // Desactivar el script de la camara para evitar conflictos
        if (cameraLook != null)
        {
            cameraLook.enabled = false;
        }
        
        // Logica de audio
        if (pausedSnapshot != null)
        {
            pausedSnapshot.TransitionTo(transitionTime);
        }
        
        // Logica del cursor solo para PC
        if (!IsMobileContext())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        
        // Es importante que el panel se desactive ANTES de reanudar la camara
        // para que no haya conflicto en el primer frame.
        pausePanel.SetActive(false);

        // Reactivar el script de la camara
        if (cameraLook != null)
        {
            cameraLook.enabled = true;
        }
        
        // Logica de audio
        if (unpausedSnapshot != null)
        {
            unpausedSnapshot.TransitionTo(transitionTime);
        }

        ButtonHover.ResetAllHovers();
        
        // Logica del cursor solo para PC
        if (!IsMobileContext())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Metodo de ayuda para saber si estamos en movil o forzando el modo movil en el editor
    private bool IsMobileContext()
    {
        bool context = Application.isMobilePlatform;
#if UNITY_EDITOR
        if (playerMovement != null && playerMovement.forceMobileControlsInEditor)
        {
            context = true;
        }
#endif
        return context;
    }

    #region Manejo de Paneles Adicionales
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
                CancelConfirmation();
                break;
        }
        pendingAction = ConfirmationAction.None;
    }
    #endregion
}
