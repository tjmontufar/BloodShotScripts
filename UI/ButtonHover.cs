using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Lista estática para rastrear todas las instancias de los botones
    private static System.Collections.Generic.List<ButtonHover> allHovers = new System.Collections.Generic.List<ButtonHover>();

    // Colores del texto
    public Color normalColor = new Color32(152, 9, 4, 255);
    public Color highlightColor = new Color32(194, 67, 4, 255);

    // Obtener el valor de escalado
    public float scaleFactor = 1.1f;

    // Definir la duracion de la animacion
    public float transitionTime = 0.1f;

    private TextMeshProUGUI buttonText;
    private Vector3 originalScale;
    private bool isHovering = false;

    public AudioClip clickAudioClip;
    public AudioClip hoverAudioClip;
    public AudioSource audioSource;

    void Awake()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        // Si no se encuentra el texto del boton
        if (buttonText == null)
        {
            Debug.LogError("No se encontro un Texto.");
            return;
        }

        originalScale = transform.localScale;
        buttonText.color = normalColor; // Establecer el color inicial

        allHovers.Add(this);
    }

    void OnDestroy()
    {
        // Quitar esta instancia de la lista cuando se destruye
        allHovers.Remove(this);
    }

    // Metodo que reinicia el estado de Hover de esta instancia
    public void ResetHoverState()
    {
        isHovering = false;
        transform.localScale = originalScale;
        buttonText.color = normalColor;
    }

    public static void ResetAllHovers()
    {
        foreach (ButtonHover hover in allHovers)
        {
            hover.ResetHoverState();
        }
    }

    // Cuando el cursor entra en el botón
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (isHovering)
        {
            if (hoverAudioClip != null && audioSource != null)
            {
                // Reproducir sonido al colocar el cursor sobre el mouse
                audioSource.PlayOneShot(hoverAudioClip);
            }
        }
    }

    // Cuando el cursor sale del botón
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void Update()
    {
        float speed = Time.unscaledDeltaTime / transitionTime;

        // Si el cursor esta sobre el boton
        if (isHovering)
        {
            // Transición del Color
            buttonText.color = Color.Lerp(buttonText.color, highlightColor, speed);

            // Transición del Escalado
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * scaleFactor, speed);
        }
        else
        {
            // Transición del Color de vuelta
            buttonText.color = Color.Lerp(buttonText.color, normalColor, speed);

            // Transición del Escalado de vuelta
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, speed);
        }
    }

    public void PlayClickSound()
    {
        if (clickAudioClip != null && audioSource != null)
        {
            // Reproducir sonido al hacer clic en el boton
            audioSource.PlayOneShot(clickAudioClip);
        }
    }
}
