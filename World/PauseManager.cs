using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Referencias")]
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private void Awake()
    {
        // --- Patron Singleton ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // Opcional: DontDestroyOnLoad(gameObject); si quieres que persista entre escenas
        }
        // -------------------------
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
    }

    private void Update()
    {
        // --- Input para Pausa en PC (tecla Escape) ---
#if !UNITY_ANDROID && !UNITY_IOS
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
#endif
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Pausar el juego
            Time.timeScale = 0f;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(true);
            }

            // Desbloquear el cursor para poder interactuar con el menu en PC
#if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
#endif
        }
        else
        {
            // Reanudar el juego
            Time.timeScale = 1f;
            if (pauseMenuUI != null)
            {
                pauseMenuUI.SetActive(false);
            }

            // Bloquear el cursor de nuevo para el modo de juego en PC
#if !UNITY_ANDROID && !UNITY_IOS
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
        }
    }
}
