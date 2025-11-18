using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject pauseMenuUI;
    public CameraLook cameraLook;       // Para apagar/encender la camara
    public PlayerMovement playerMovement; // Para saber si se fuerza el modo movil

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Asegurarse de que el juego siempre comience sin pausa
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        if (cameraLook != null)
        {
            cameraLook.enabled = true;
        }
    }

    private void Update()
    {
        // Determinar si estamos en un contexto de PC o no
        bool isMobileContext = IsMobileContext();

        // --- Input para Pausa en PC (tecla Escape) ---
        if (!isMobileContext && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        bool isMobileContext = IsMobileContext();

        if (isPaused)
        {
            // Pausar el juego
            Time.timeScale = 0f;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
            }
            if (cameraLook != null)
            {
                cameraLook.enabled = false;
            }

            // Desbloquear el cursor solo en contexto de PC
            if (!isMobileContext)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else
        {
            // Reanudar el juego
            Time.timeScale = 1f;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }
            if (cameraLook != null)
            {
                cameraLook.enabled = true;
            }

            // Bloquear el cursor de nuevo solo en contexto de PC
            if (!isMobileContext)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    // Metodo de ayuda para saber si estamos en movil o forzando el modo movil
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
}
