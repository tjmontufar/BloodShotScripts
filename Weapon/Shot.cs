using UnityEngine;
using UnityEngine.Audio;

public class Shot : MonoBehaviour
{
    // Elementos para el disparo
    public Transform spawnPoint;
    public GameObject bullet;
    // Varialbes para manejo del disparo
    public float shotForce = 1500f;
    public float shotRate = 0.5f;

    public AudioClip shotAudioClip;
    public AudioSource shotAudioSource;

    private float shotRateTime = 0;

    // Referencia al PlayerMovement para saber si estamos en modo movil forzado
    private PlayerMovement playerMovement;

    private void Start()
    {
        // Encontrar el script del jugador en la escena
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        // Evitar que el arma dispare si el juego esta en pausa.
        if (Time.timeScale == 0f) return;

        bool isMobileForced = false;
        #if UNITY_EDITOR
        if (playerMovement != null)
        {
            isMobileForced = playerMovement.forceMobileControlsInEditor;
        }
        #endif

        // Mantener el clic izquierdo como boton para disparar solo en PC
        if (!Application.isMobilePlatform && !isMobileForced)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                PerformShot();
            }
        }
    }

    // Metodo publico para ser llamado desde un boton de UI
    public void PerformShot()
    {
        // Agregar retardo por disparo y comprobar si hay suficiente municion
        if (Time.time > shotRateTime && GameManager.Instance.gunAmmo > 0)
        {
            // Reducir el valor de la municion del arma
            GameManager.Instance.gunAmmo--;
            // Crear una bala
            GameObject newBullet;
            // Agregar la bala en el punto de aparicion
            newBullet = Instantiate(bullet, spawnPoint.position, spawnPoint.rotation);
            // Agregar fuerza de disparo
            newBullet.GetComponent<Rigidbody>().AddForce(spawnPoint.forward * shotForce);

            if (shotAudioSource != null && shotAudioClip != null)
            {
                // Reproducir sonido de disparo
                shotAudioSource.PlayOneShot(shotAudioClip);
            }
            // Agregar el ratio de disparo
            shotRateTime = Time.time + shotRate;
            // Eliminar la bala despues de ser disparada
            Destroy(newBullet, 1);
        }
    }
}

