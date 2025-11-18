using System;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    // Textos en pantalla para mostrar los objetivos que el jugador debe realizar
    [Header("Textos de Mision")]
    public GameObject Quest1Text;
    public GameObject Quest2Text;
    public GameObject Quest3Text;

    private int currentQuestIndex = 0;

    private bool dynamiteUsed = false;

    private LevelManager levelManager;

    private void Awake()
    {
        // Establecer la instancia única.
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        levelManager = LevelManager.Instance;

        if (levelManager == null)
        {
            Debug.Log("No se encontro la instancia a LevelManager.");
            return;
        }

        if (Quest1Text != null)
        {
            Quest1Text.SetActive(true);
        }

        if (Quest2Text != null) 
        {
            Quest2Text.SetActive(false);
        }

        if (Quest3Text != null)
        {
            Quest3Text.SetActive(false);
        }
    }

    void Update()
    {
        if (Quest1Text != null && Quest2Text != null && Quest3Text != null)
        {
            CheckQuestProgression();
        }
    }

    // Metodo para mostrar tareas por cumplir en pantalla
    public void CheckQuestProgression()
    {
        if (levelManager == null)
        {
            return;
        }

        switch (currentQuestIndex)
        {
            case 0:
                if (Quest1Text != null)
                {
                    // Mision 1: Matar enemigos
                    Quest1Text.SetActive(true);
                    
                    if (levelManager.usesEnemyQuest && levelManager.EnemyKillCount >= levelManager.EnemyKillChallenge)
                    {
                        Quest1Text.SetActive(false);
                        currentQuestIndex++;
                        CheckQuestProgression();
                    }
                    else if (!levelManager.usesEnemyQuest)
                    {
                        Quest1Text.SetActive(false);
                        currentQuestIndex++;
                        CheckQuestProgression();
                    }
                }
                else
                {
                    currentQuestIndex++;
                    Debug.Log("No existe el Quest1. Omitiendo...");
                }
                    break;

            case 1:
                if (Quest2Text != null)
                {
                    // Mision 2: Usar la dinamita y recoger los cristales
                    Quest2Text.SetActive(true);

                    if (levelManager.usesCrystalQuest && levelManager.CrystalCount >= levelManager.CrystalCollectChallenge)
                    {
                        //if (dynamiteUsed)
                        //{
                            
                        //}

                        Quest2Text.SetActive(false);
                        currentQuestIndex++;
                        CheckQuestProgression();

                        // Iniciar el contador
                        if (levelManager.timerText != null)
                        {
                            levelManager.StartEscapeTimer();
                        }
                    }
                    else if (!levelManager.usesCrystalQuest)
                    {
                        Quest2Text.SetActive(false);
                        currentQuestIndex++;
                        CheckQuestProgression();
                    }
                }
                else
                {
                    currentQuestIndex++;
                    Debug.Log("No existe el Quest2. Omitiendo...");
                }
                    break;

            case 2:
                Quest3Text.SetActive(true);
                break;
        }
    }

    // Metodo para comprobar que haya detonado la dinamita
    public void DynamiteActionCompleted()
    {
        if (!dynamiteUsed)
        {
            dynamiteUsed = true;
            CheckQuestProgression();
        }
    }
}
