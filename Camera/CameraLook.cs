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
        float mouseX;
        float mouseY;

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
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                // Si el dedo se mueve y NO esta sobre un objeto de la UI (botones, joystick, etc.)
                if (touch.phase == TouchPhase.Moved && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    mouseX = touch.deltaPosition.x * touchSensitivity * Time.deltaTime;
                    mouseY = touch.deltaPosition.y * touchSensitivity * Time.deltaTime;
                }
                else
                {
                    // No mover la camara si no se arrastra el dedo o si esta sobre la UI
                    mouseX = 0;
                    mouseY = 0;
                }
            }
            else
            {
                // No hay toques en la pantalla
                mouseX = 0;
                mouseY = 0;
            }
        }
        // --- Logica para PC ---
        else
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }

        // --- Rotacion (comun para ambas plataformas) ---
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
