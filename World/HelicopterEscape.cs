using UnityEngine;

public class HelicopterEscape : MonoBehaviour
{
    private bool isDescending = false;

    private Animator animator;

    public AudioClip helicopterAudioClip;
    public AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null )
        {
            Debug.LogError("Animator no encontrado en el HelicopteroPadre.");
        }

        if (audioSource != null && helicopterAudioClip != null)
        {
            // Reproducir sonido del helicoptero
            audioSource.clip = helicopterAudioClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StartDescent()
    {
        if (!isDescending && animator != null)
        {
            isDescending = true;
            Debug.Log("El helicoptero esta descendiendo.");

            animator.SetBool("IsLanding", true);

            Invoke("FinishLanding", 6.0f);
        }
    }

    private void FinishLanding()
    {
        isDescending = false;
        Debug.Log("Helicoptero aterrizado.");
    }
}
