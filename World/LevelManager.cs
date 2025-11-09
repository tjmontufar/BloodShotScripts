using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    // Contador de enemigos eliminados
    [Header("Contadores Generales")]
    public GameObject EnemyGroup;
    public bool usesEnemyQuest = false;
    public Text EnemyKillText;
    public int EnemyKillCount = 0;

    // Contador de cristales recogidos
    public GameObject CrystalGroup;
    public bool usesCrystalQuest = false;
    public Text CrystalText;
    public int CrystalCount = 0;

    // Textos en pantalla para mostrar los objetivos que el jugador debe realizar
    [Header("Textos de Mision")]
    public GameObject Quest1Text;
    public GameObject Quest2Text;
    public GameObject Quest3Text;

    private int currentQuestIndex = 0;

    private bool dynamiteUsed = false;

    // Variables para los objetivos del nivel (se ajustan en las escenas)
    [Header("Objetivos del Nivel")]
    public int EnemyKillChallenge = 0;
    public int CrystalCollectChallenge = 0;
    private bool challengesCompleted = false;

    // Llamar el Script para manejo del descenso del helicoptero al lograr las metas
    [Header("Helicoptero de Escape")]
    public bool usesHelicopter = false;
    public HelicopterEscape helicopterScript;

    // Variables para el temporizador de escape
    [Header("Temporizador de escape")]
    public Text timerText;
    public float maxEscapeTime = 60f;
    private float currentEscapeTime;
    private bool isTimerRunning = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(EnemyGroup != null)
        {
            if(!usesEnemyQuest)
            {
                EnemyGroup.SetActive(false);
            }
        }

        if (CrystalGroup != null)
        {
            if (!usesCrystalQuest)
            {
                CrystalGroup.SetActive(false);
            }
        }
    }

    void Update()
    {
        EnemyKillText.text = EnemyKillCount.ToString() + " / " + EnemyKillChallenge.ToString();
        CrystalText.text = CrystalCount.ToString() + " / " + CrystalCollectChallenge.ToString();

        CheckChallenges();
        CheckQuestProgression();

        if (isTimerRunning)
        {
            currentEscapeTime -= Time.deltaTime;

            DisplayTime(currentEscapeTime);

            if (currentEscapeTime <= 0)
            {
                currentEscapeTime = 0;
                isTimerRunning = false;
                GameOverByTimeOut();
            }
        }
    }

    // Metodo para mostrar tareas por cumplir en pantalla
    public void CheckQuestProgression()
    {
        switch (currentQuestIndex)
        {
            case 0:
                Quest1Text.SetActive(true);
                // Mision 1: Matar enemigos
                if (usesEnemyQuest && EnemyKillCount >= EnemyKillChallenge)
                {
                    Quest1Text.SetActive(false);
                    currentQuestIndex++;
                    CheckQuestProgression();
                }
                else if (!usesEnemyQuest)
                {
                    Quest1Text.SetActive(false);
                    currentQuestIndex++;
                    CheckQuestProgression();
                }
                break;
            case 1:
                // Mision 2: Usar la dinamita y recoger los cristales
                Quest2Text.SetActive(true);

                if (usesCrystalQuest && CrystalCount >= CrystalCollectChallenge)
                {
                    if(dynamiteUsed)
                    {
                        Quest2Text.SetActive(false);
                        currentQuestIndex++;
                        CheckQuestProgression();

                        currentEscapeTime = maxEscapeTime;
                        isTimerRunning = true;
                        timerText.gameObject.SetActive(true);
                    }
                }
                else if (!usesCrystalQuest) 
                {
                    Quest2Text.SetActive(false);
                    currentQuestIndex++;
                    CheckQuestProgression();
                }
                break;
            case 2:
                Quest3Text.SetActive(true);
                break;
        }
    }

    // Metodo para verificar si se han cumplido las metas
    public void CheckChallenges()
    {
        if (challengesCompleted)
        {
            return;
        }

        if (usesEnemyQuest && usesCrystalQuest)
        {
            if (EnemyKillCount >= EnemyKillChallenge && CrystalCount >= CrystalCollectChallenge)
            {
                challengesCompleted = true;

                Debug.Log("Cumpliste la meta. (Avanza el siguiente nivel)");

                if (usesHelicopter && helicopterScript != null)
                {
                    helicopterScript.StartDescent();
                }
            }
        }
        else if (!usesEnemyQuest || !usesCrystalQuest)
        {
            if (usesHelicopter && helicopterScript != null)
            {
                helicopterScript.StartDescent();
            }
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

    // Metodo para formatear y mostrar el tiempo
    void DisplayTime(float timeToDisplay)
    {
        // Asegura que no se muestren números negativos
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        // Calcula los minutos y segundos (formato MM:SS)
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Metodo para cargar pantalla de Game Over si el tiempo se ha agotado
    void GameOverByTimeOut()
    {
        Debug.Log("¡Tiempo agotado!");

        Time.timeScale = 0f;

        SceneManager.LoadScene("GameOverScene");
    }
}
