using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    public Transform startPosition;
    private void OnTriggerEnter(Collider other)
    {
        // Comprobar si el jugador colisiona con una caja de municion
        if (other.gameObject.CompareTag("GunAmmo"))
        {
            // Sumar municion luego de colisionar con la caja
            GameManager.Instance.gunAmmo += other.gameObject.GetComponent<AmmoBox>().ammo;
            
            Destroy(other.gameObject);
        }

        // Comprobar si el jugador cayo al suelo de muerte
        if(other.gameObject.CompareTag("DeathFloor"))
        {
            // Perder vida, respawnear nuestro jugador
            GameManager.Instance.LoseHealth(50);

            GetComponent<CharacterController>().enabled = false;

            gameObject.transform.position = startPosition.position;

            GetComponent<CharacterController>().enabled = true;
        }
    }
}
