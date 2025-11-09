using System.Collections;
using UnityEngine;

public class DynamiteSpot : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject wallToDestroy;
    public GameObject hiddenCrystals;
    public GameObject promptUI;

    [Header("Condición")]
    // El jugador debe eliminar 15 enemigos para poder colocar una dinamita
    public int requiredKills = 15;

    [Header("Explosión")]
    public float timeToExplode = 3.0f;
    public GameObject dynamitePrefab;

    private bool playerIsNearby = false;
    private bool isCountingDown = false;

    void Start()
    {
        if (hiddenCrystals != null)
        {
            hiddenCrystals.SetActive(false);
        }

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerIsNearby && !isCountingDown)
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();

            if (levelManager != null && levelManager.EnemyKillCount >= requiredKills)
            {
                if (promptUI != null)
                {
                    promptUI.SetActive(true);
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (promptUI != null)
                    {
                        promptUI.SetActive(false);
                    }

                    //DestroyWall();
                    StartCoroutine(DestroyWallSequence());
                }
            }
            else
            {
                if (promptUI != null)
                {
                    promptUI.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
            // Ocultar el prompt UI al salir
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    private IEnumerator DestroyWallSequence()
    {
        isCountingDown = true;

        GameObject placedDynamite = null;

        // Instanciar la dinamita en el punto de interaccion
        if (dynamitePrefab != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, -45f, 0);

            placedDynamite = Instantiate(dynamitePrefab, transform.position, targetRotation);

        }

        Debug.Log("Dinamita colocada. Explosión en " + timeToExplode + " segundos...");

        yield return new WaitForSeconds(timeToExplode);

        // Destruir la pared
        if (wallToDestroy != null)
        {
            Destroy(wallToDestroy);
            LevelManager.Instance.DynamiteActionCompleted();
        }

        // Eliminar la dinamita despues de detonar
        if (placedDynamite != null)
        {
            Destroy(placedDynamite);
            Debug.Log("Eliminando dinamita...");
        }

        // Liberar los cristales
        if (hiddenCrystals != null)
        {
            hiddenCrystals.SetActive(true);
        }

        // Eliminar el objeto DynamiteSpot
        isCountingDown = false;
        Destroy(gameObject);
    }
}
