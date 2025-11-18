using UnityEngine;
using UnityEngine.EventSystems;

// Implementa las interfaces para detectar eventos de la UI (tocar, arrastrar, levantar)
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [HideInInspector]
    public Vector2 direction; // La direccion del joystick, para que la lea el PlayerMovement

    [Header("Referencias")]
    [SerializeField] private RectTransform background; // La imagen de fondo del joystick
    [SerializeField] private RectTransform handle;     // La imagen del punto central que se mueve

    private float joystickRadius;
    private bool isBeingTouched = false; // Un flag para saber si el joystick esta siendo usado

    private void Start()
    {
        joystickRadius = background.sizeDelta.x / 2;
        ResetJoystick();
    }

    private void Update()
    {
        // --- Mecanismo de Seguridad ---
        // Este bloque previene que el joystick se quede "pegado" despues de pausar.
        // Si no hay ningun dedo en la pantalla y el joystick cree que sigue presionado, lo reseteamos.
        if (Application.isMobilePlatform && Input.touchCount == 0 && isBeingTouched)
        {
            // Forzar el reseteo
            OnPointerUp(null);
        }
    }

    // Se llama cuando el dedo se arrastra sobre el joystick
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position))
        {
            // Calcular la direccion y limitarla al radio del joystick
            Vector2 rawDirection = position / joystickRadius;
            direction = (rawDirection.magnitude > 1) ? rawDirection.normalized : rawDirection;

            // Mover el handle visualmente
            handle.anchoredPosition = direction * joystickRadius;
        }
    }

    // Se llama la primera vez que el dedo toca el joystick
    public void OnPointerDown(PointerEventData eventData)
    {
        isBeingTouched = true; // Marcamos que esta siendo usado
        OnDrag(eventData);     // Ejecutamos la logica de arrastre para que responda al primer toque
    }

    // Se llama cuando el dedo se levanta del joystick
    public void OnPointerUp(PointerEventData eventData)
    {
        isBeingTouched = false; // Marcamos que ya no se esta usando
        ResetJoystick();
    }

    // Metodo para resetear la posicion y direccion del joystick
    private void ResetJoystick()
    {
        direction = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
