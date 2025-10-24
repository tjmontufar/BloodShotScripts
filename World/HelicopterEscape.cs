using UnityEngine;

public class HelicopterEscape : MonoBehaviour
{
    private bool isDescending = false;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null )
        {
            Debug.LogError("Animator no encontrado en el HelicopteroPadre.");
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

    private void OnTriggerEnter(Collider other)
    {
        //if (!isDescending && other.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("Tocando helicoptero. ¡Nivel Completo!");
        //    GameManager.Instance.LevelComplete();
        //}
        //else
        //{
        //    Debug.Log("No funciona el trigger.");
        //}
        Debug.Log("Trigger disparado. Objeto que entró: " + other.gameObject.name);
    }
}
