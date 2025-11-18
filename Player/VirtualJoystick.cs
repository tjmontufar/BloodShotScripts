using UnityEngine;
using UnityEngine.EventSystems; // Necesario para la interfaz de Eventos de UI

// Implementa interfaces necesarias para manejo de toque/arrastre
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    // Variables públicas para el acceso desde el script de Movimiento del Jugador
    [HideInInspector] public Vector2 direction;

    [Header("Referencias")]
    // La imagen de fondo del joystick (el círculo base)
    [SerializeField] private RectTransform background;
    // La imagen del 'dedo' o stick que se mueve
    [SerializeField] private RectTransform handle;

    // Radio máximo de movimiento (para que el stick no se salga del fondo)
    private float joystickRadius;

    private void Start()
    {
        // Se calcula el radio del joystick como la mitad del ancho del fondo
        joystickRadius = background.sizeDelta.x / 2;

        // Asegúrate de que el joystick se inicialice en el centro
        ResetHandlePosition();
    }

    /// <summary>
    /// Llamado cuando el dedo toca y arrastra sobre el área del Joystick.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = Vector2.zero;

        // Intentar convertir la posición de la pantalla a una posición local del RectTransform del fondo.
        // Esto nos da la posición del toque relativa al centro del joystick.
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out position))
        {
            // Normalizar la posición (hacerla de -1 a 1) dividiendo por el radio.
            Vector2 rawDirection = position / joystickRadius;

            // Si la dirección es mayor a 1 (el dedo se salió del círculo), la limitamos.
            if (rawDirection.magnitude > 1)
            {
                rawDirection = rawDirection.normalized; // Mantenemos la dirección, pero la magnitud a 1
            }

            // Aplicamos la dirección al Handle (la imagen que se mueve)
            handle.anchoredPosition = rawDirection * joystickRadius;

            // Almacenamos la dirección normalizada (de -1 a 1) para el script de movimiento del jugador
            direction = rawDirection;
        }
    }

    /// <summary>
    /// Llamado cuando el dedo toca por primera vez el Joystick.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Al tocar, simplemente llamamos a OnDrag para empezar a calcular la posición.
        OnDrag(eventData);
    }

    /// <summary>
    /// Llamado cuando el dedo se levanta del Joystick.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        // Restablecemos la dirección a cero
        direction = Vector2.zero;

        // Devolvemos el Handle a la posición central
        ResetHandlePosition();
    }

    private void ResetHandlePosition()
    {
        // Centrar el stick
        handle.anchoredPosition = Vector2.zero;
    }
}