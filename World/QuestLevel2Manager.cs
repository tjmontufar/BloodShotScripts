using System.Collections;
using UnityEngine;

public class QuestLevel2Manager : MonoBehaviour
{
    public static QuestLevel2Manager Instance { get; private set; }

    // Textos en pantalla para mostrar los objetivos que el jugador debe realizar
    [Header("Textos de Mision")]
    public GameObject Quest1Text;
    public GameObject Quest2Text;

    private int currentQuestIndex = 0;

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
        
    }

    void Update()
    {
        if (Quest1Text != null && Quest2Text != null)
        {
            CheckQuestProgression();
        }
    }

    public void CheckQuestProgression()
    {
        switch (currentQuestIndex)
        {
            case 0:
                Quest1Text.SetActive(true);
                // Mision 1: 
                break;
            case 1:
                break;
        }
    }
}
