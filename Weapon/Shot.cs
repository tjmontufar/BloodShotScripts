using UnityEngine;

public class Shot : MonoBehaviour
{
    // Elementos para el disparo
    public Transform spawnPoint;
    public GameObject bullet;
    // Varialbes para manejo del disparo
    public float shotForce = 1500f;
    public float shotRate = 0.5f;

    private float shotRateTime = 0;

    void Update()
    {
        // Evitar que el arma dispare si el juego esta en pausa.
        if (Time.timeScale == 0f) return;

        // Establecer clic izquierdo como boton para disparar
        if (Input.GetButtonDown("Fire1"))
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
                // Agregar el ratio de disparo
                shotRateTime = Time.time + shotRate;
                // Eliminar la bala despues de ser disparada
                Destroy(newBullet,1);
            }
        }
    }
}
