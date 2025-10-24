using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Daño predeterminado para las balas
    public int bulletDamage = 20;

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy hitEnemy = collision.gameObject.GetComponent<Enemy>();

            if (hitEnemy != null)
            {
                hitEnemy.LoseEnemyHealth(bulletDamage);
            }

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
