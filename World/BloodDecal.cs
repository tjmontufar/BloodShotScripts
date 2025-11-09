using System.Collections;
using UnityEngine;

public class BloodDecal : MonoBehaviour
{
    public float lifetime = 15f;
    public float fadeDuration = 5f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        StartCoroutine(FadeAndDestroy());
    }

    private IEnumerator FadeAndDestroy()
    {
        // Esperar el tiempo de vida antes de empezar a desvanecerse
        yield return new WaitForSeconds(lifetime);

        float startTime = Time.time;
        Color startColor = spriteRenderer.color;

        // Desvanecer (Fade out)
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            // Reducir el componente Alpha (transparencia) del color
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, t));
            spriteRenderer.color = newColor;

            yield return null;
        }

        // Asegurar la transparencia final y destruir el objeto
        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        Destroy(gameObject);
    }
}
