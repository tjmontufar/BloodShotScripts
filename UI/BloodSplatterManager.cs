using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BloodSplatterManager : MonoBehaviour
{
    public static BloodSplatterManager Instance { get; private set; }

    public GameObject splatterCanvas;

    public float lifetime = 1f;
    public float fadeDuration = 0.5f;

    private Image splatterImage;

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

        if (splatterCanvas != null)
        {
            splatterImage = splatterCanvas.GetComponentInChildren<Image>(true);
            splatterCanvas.SetActive(false);
        }

        if (splatterImage == null && splatterCanvas != null)
        {
            Debug.LogError("No se encontro el componente Image de BloodSplash.");
        }
    }

    public void ShowSplatter()
    {
        if (splatterImage == null) return;

        Color color = splatterImage.color;
        splatterImage.color = new Color(color.r, color.g, color.b, 1f);

        StopAllCoroutines();
        StartCoroutine(SplatterEffectCoroutine());
    }

    private IEnumerator SplatterEffectCoroutine()
    {
        if (splatterCanvas == null || splatterImage == null) yield break;

        splatterCanvas.SetActive(true);

        yield return new WaitForSeconds(lifetime);

        float startTime = Time.time;
        Color startColor = splatterImage.color;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;

            float newAlpha = Mathf.Lerp(startColor.a, 0f, t);

            splatterImage.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

            yield return null;

        }
        
        splatterImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        splatterCanvas.SetActive(false);
    }
}
