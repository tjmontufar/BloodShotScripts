using System.Collections;
using UnityEngine;

public class DynamiteSpot : MonoBehaviour
{
    [Header("Referencias de Escena")]
    public GameObject wallToDestroy;
    public GameObject hiddenCrystals;
    public GameObject dynamitePrefab;

    [Header("UI (Arrastra los objetos)")]
    public GameObject promptTextPC;
    public GameObject placeDynamiteButtonMobile;

    [Header("Condicion")]
    public int requiredKills = 15;

    [Header("Explosion")]
    public float timeToExplode = 3.0f;

    [Header("Sonido")]
    public AudioClip explosionAudioClip;

    private bool playerIsNearby = false;
    private bool requirementsMet = false;
    private bool isCountingDown = false;
    private PlayerMovement playerMovement; // Referencia para saber el contexto

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        if (hiddenCrystals != null)
        {
            hiddenCrystals.SetActive(false);
        }
        HidePrompts();
    }

    void Update()
    {
        if (playerIsNearby && !isCountingDown)
        {
            // Comprobar si se cumplen los requisitos
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            requirementsMet = (levelManager != null && levelManager.EnemyKillCount >= requiredKills);

            if (requirementsMet)
            {
                ShowCorrectPrompt();

                // Logica para PC: presionar la tecla 'E'
                if (!IsMobileContext() && Input.GetKeyDown(KeyCode.E))
                {
                    PlaceDynamite();
                }
            }
            else
            {
                HidePrompts();
            }
        }
    }

    // Este metodo es publico para ser llamado por el boton de la UI en movil
    public void PlaceDynamite()
    {
        if (requirementsMet && !isCountingDown)
        {
            HidePrompts();
            StartCoroutine(DestroyWallSequence());
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
            requirementsMet = false;
            HidePrompts();
        }
    }

    private void ShowCorrectPrompt()
    {
        if (IsMobileContext())
        {
            if (promptTextPC != null) promptTextPC.SetActive(false);
            if (placeDynamiteButtonMobile != null) placeDynamiteButtonMobile.SetActive(true);
        }
        else
        {
            if (promptTextPC != null) promptTextPC.SetActive(true);
            if (placeDynamiteButtonMobile != null) placeDynamiteButtonMobile.SetActive(false);
        }
    }

    private void HidePrompts()
    {
        if (promptTextPC != null) promptTextPC.SetActive(false);
        if (placeDynamiteButtonMobile != null) placeDynamiteButtonMobile.SetActive(false);
    }

    private bool IsMobileContext()
    {
        bool context = Application.isMobilePlatform;
#if UNITY_EDITOR
        if (playerMovement != null && playerMovement.forceMobileControlsInEditor)
        {
            context = true;
        }
#endif
        return context;
    }

    private IEnumerator DestroyWallSequence()
    {
        isCountingDown = true;
        GameObject placedDynamite = null;

        if (dynamitePrefab != null)
        {
            Quaternion targetRotation = Quaternion.Euler(0, -45f, 0);
            placedDynamite = Instantiate(dynamitePrefab, transform.position, targetRotation);
        }

        yield return new WaitForSeconds(timeToExplode);

        if (explosionAudioClip != null)
        {
            GameObject audioObject = new GameObject("ExplosionSound");
            audioObject.transform.position = transform.position;
            AudioSource tempAudioSource = audioObject.AddComponent<AudioSource>();
            tempAudioSource.clip = explosionAudioClip;
            tempAudioSource.Play();
            Destroy(audioObject, explosionAudioClip.length);
        }

        if (wallToDestroy != null)
        {
            Destroy(wallToDestroy);
            if (QuestManager.Instance != null)
            {
                QuestManager.Instance.DynamiteActionCompleted();
            }
        }

        if (placedDynamite != null)
        {
            Destroy(placedDynamite);
        }

        if (hiddenCrystals != null)
        {
            hiddenCrystals.SetActive(true);
        }

        isCountingDown = false;
        Destroy(gameObject);
    }
}
