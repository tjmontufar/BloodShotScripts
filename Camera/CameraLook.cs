using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic; // Requerido para usar List<T>

public class CameraLook : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivity = 80f;
    public float touchSensitivity = 0.5f;

    [Header("Referencias")]
    public Transform playerBody;

    private float xRotation = 0;
    private PlayerMovement playerMovement;

    // Lista para almacenar los IDs de los toques que debemos ignorar (porque empezaron sobre la UI)
    private List<int> ignoredFingerIds = new List<int>();

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        UpdateCursorLock();
    }

    void Update()
    {
        // Si el juego esta pausado, no procesar nada.
        if (Time.timeScale == 0f) return;

        float lookX = 0;
        float lookY = 0;

        bool isMobileForced = false;
        #if UNITY_EDITOR
        if (playerMovement != null)
        {
            isMobileForced = playerMovement.forceMobileControlsInEditor;
        }
        #endif

        if (Application.isMobilePlatform || isMobileForced)
        {
            // --- Logica de Toques Avanzada ---
            foreach (Touch touch in Input.touches)
            {
                // FASE 1: Deteccion de nuevos toques
                if (touch.phase == TouchPhase.Began)
                {
                    // Si el toque empieza sobre la UI, lo añadimos a la lista de ignorados.
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        ignoredFingerIds.Add(touch.fingerId);
                    }
                }

                // FASE 2: Procesar movimiento
                // Si el dedo se mueve Y su ID no esta en la lista de ignorados.
                if (touch.phase == TouchPhase.Moved && !ignoredFingerIds.Contains(touch.fingerId))
                {
                    lookX = touch.deltaPosition.x * touchSensitivity * Time.deltaTime;
                    lookY = touch.deltaPosition.y * touchSensitivity * Time.deltaTime;
                    // Rompemos el bucle, ya que solo un dedo debe controlar la camara a la vez.
                    break;
                }

                // FASE 3: Limpieza
                // Si el dedo se levanta, lo quitamos de la lista de ignorados para liberar la memoria.
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    if (ignoredFingerIds.Contains(touch.fingerId))
                    {
                        ignoredFingerIds.Remove(touch.fingerId);
                    }
                }
            }
        }
        else
        {
            // --- Logica para PC ---
            // Solo actualizamos el estado del cursor si no estamos en modo movil
            UpdateCursorLock();
            lookX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            lookY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }

        // --- Rotacion (comun para ambas plataformas) ---
        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        playerBody.Rotate(Vector3.up * lookX);
    }
    
    // Metodo para manejar el bloqueo del cursor
    void UpdateCursorLock()
    {
        if (Time.timeScale > 0f) // Si no estamos en pausa
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else // Estamos en pausa
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
