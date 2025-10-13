using UnityEngine;

public class CameraLook : MonoBehaviour
{
    // Manejo de la sensibilidad del Mouse
    public float mouseSensitivity = 80f;
    // Obtener la posicion del jugador
    public Transform playerBody;

    float xRotation = 0;

    void Start()
    {
        // Bloquear la posicion del mouse al centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log("Hola mundo");
    }

    void Update()
    {
        // Obtener la posicion en X del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        // Obtener la posicion en Y del mouse
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
