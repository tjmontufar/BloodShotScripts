using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;

// Este script debe ser añadido al GameObject del Boton de Correr en la UI
public class SprintButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // Arrastra tu objeto Jugador (el que tiene PlayerMovement) a este campo en el Inspector
    public PlayerMovement playerMovement;

    // Este metodo se llama cuando el dedo TOCA el boton
    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerMovement != null)
        {
            playerMovement.StartSprinting();
            UnityEngine.Debug.Log("Corriendo.");
        }
    }

    // Este metodo se llama cuando el dedo SE LEVANTA del boton
    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerMovement != null)
        {
            playerMovement.StopSprinting();
            UnityEngine.Debug.Log("Dejó de correr.");
        }
    }
}
