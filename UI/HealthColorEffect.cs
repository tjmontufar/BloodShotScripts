using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthColorEffect : MonoBehaviour
{
    public static HealthColorEffect Instance { get; private set; }

    public Volume volume;

    private ColorAdjustments colorAdjustmentsLayer = null;

    [HideInInspector] public int maxHealth = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        if (volume == null)
        {
            volume = FindObjectOfType<Volume>();
        }

        if (volume == null)
        {
            Debug.LogError("HealthColorEffect: No se encontró ningún Volume en la escena.");
            return;
        }

        if (!volume.profile.TryGet(out colorAdjustmentsLayer))
        {
            Debug.LogError("HealthColorEffect: El perfil del volumen no contiene la capa de Color Adjustments.");
        }
    }

    public void UpdateSaturation(int currentHealth)
    {
        if (colorAdjustmentsLayer == null) return;

        float healthRatio = (float)currentHealth / maxHealth;
        float effectRatio = 1f - healthRatio;

        float targetSaturation = Mathf.Lerp(0f, -100f, effectRatio);

        colorAdjustmentsLayer.saturation.value = targetSaturation;
    }

    public void ResetSaturation()
    {
        if (colorAdjustmentsLayer != null)
        {
            colorAdjustmentsLayer.saturation.value = 0f;
        }
    }
}
