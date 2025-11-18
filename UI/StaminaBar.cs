using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider staminaSlider;
    public float maxStamina = 100;
    private float currentStamina;

    // Velocidades por segundo
    public float staminaDrainRate = 15f;
    public float staminaRegenRate = 10f;
    public float regenDelay = 1.0f;
    private float lastSprintTime;

    // Referencia al jugador
    private PlayerMovement playerMovement;

    void Start()
    {
        currentStamina = maxStamina;
        staminaSlider.maxValue = maxStamina;
        staminaSlider.value = maxStamina;

        // Asignar referencia al jugador
        playerMovement = FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        if (playerMovement == null) return;

        if (playerMovement.isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            // Guardar el tiempo del último sprint
            lastSprintTime = Time.time;

            // Si se agota la estamina
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                // Forzar al jugador a dejar de correr
                playerMovement.isSprinting = false;
                Debug.Log("Estamina Agotada.");
            }
        }
        else
        {
            if (Time.time >= lastSprintTime + regenDelay)
            {
                if (currentStamina < maxStamina)
                {
                    // Regenera la estamina suavemente por segundo
                    currentStamina += staminaRegenRate * Time.deltaTime;
                    currentStamina = Mathf.Min(currentStamina, maxStamina); // No exceder el máximo
                }
            }
        }

        // Actualizar la barra visual
        staminaSlider.value = currentStamina;
    }

    // Método para que PlayerMovement.cs pueda chequear si hay estamina
    public bool IsStaminaAvailable()
    {
        // Consideramos que la estamina está disponible si es > 0, o > un mínimo si quieres un buffer
        return currentStamina > 0;
    }
}