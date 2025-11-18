using UnityEngine;
using UnityEngine.EventSystems; // Requerido para ignorar toques en la UI

public class CameraLook : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivity = 80f;
    public float touchSensitivity = 0.5f; // Sensibilidad para el control tactil

    [Header("Referencias")]
    public Transform playerBody;

    private float xRotation = 0;
    private PlayerMovement playerMovement; // Referencia para saber si forzamos el modo movil

    void Start()
    {
        // Encontrar el script del jugador
        playerMovement = FindObjectOfType<PlayerMovement>();

        bool isMobileForced = false;
        #if UNITY_EDITOR
        if (playerMovement != null)
        {
            isMobileForced = playerMovement.forceMobileControlsInEditor;
        }
        #endif

        // Bloquear la posicion del mouse solo en PC
        if (!Application.isMobilePlatform && !isMobileForced)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        float lookX = 0;
        float lookY = 0;

        bool isMobileForced = false;
        #if UNITY_EDITOR
        if (playerMovement != null)
        {
            isMobileForced = playerMovement.forceMobileControlsInEditor;
        }
        #endif

        // --- Logica para Movil ---
        if (Application.isMobilePlatform || isMobileForced)
        {
            // Iterar a traves de todos los toques en la pantalla
            foreach (Touch touch in Input.touches)
            {
                // Si el toque NO esta sobre un elemento de la UI (joystick, boton, etc.)
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    // Si el dedo se esta moviendo, ese es nuestro toque para mirar
                    if (touch.phase == TouchPhase.Moved)
                    {
                        lookX = touch.deltaPosition.x * touchSensitivity * Time.deltaTime;
                        lookY = touch.deltaPosition.y * touchSensitivity * Time.deltaTime;
                        // Rompemos el bucle porque ya encontramos el toque para mirar
                        break;
                    }
                }
            }
        }
        // --- Logica para PC ---
        else
        {
            lookX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            lookY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }

        // --- Rotacion (comun para ambas plataformas) ---
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * lookX);
    }
}
