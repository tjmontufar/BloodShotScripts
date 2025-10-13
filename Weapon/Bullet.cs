using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy hitEnemy = collision.gameObject.GetComponent<Enemy>();

            if (hitEnemy != null)
            {
                hitEnemy.LoseEnemyHealth(20);
                Debug.Log("Vida restante del enemigo: " + hitEnemy.health);

                if (hitEnemy.CheckEnemyHealth() == true)
                {
                    Debug.Log("Enemigo derrotado.");

                    hitEnemy.Die();
                }
            }

            Destroy(gameObject);
        }
    }
}
