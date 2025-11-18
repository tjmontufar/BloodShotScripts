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

    [Header("Referencias")]
    public QuestManager questManager;

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

        if (questManager != null)
        {
            questManager.CheckQuestProgression();
        }

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

    // Metodo para verificar si se han cumplido las metas
    public void CheckChallenges()
    {
        if (challengesCompleted)
        {
            return;
        }

        // Si ambas tareas estan activas
        if (usesEnemyQuest && usesCrystalQuest)
        {
            // Si se cumplen ambas tareas
            if (EnemyKillCount >= EnemyKillChallenge && CrystalCount >= CrystalCollectChallenge)
            {
                challengesCompleted = true;

                Debug.Log("Cumpliste la meta. (Avanza el siguiente nivel)");

                StartDescendHelicopter();
            }
        }
        // Si solo esta activa la tarea de eliminar enemigos
        else if (usesEnemyQuest && !usesCrystalQuest)
        {
            // Si se cumple la tarea de eliminar enemigos
            if (EnemyKillCount >= EnemyKillChallenge)
            {
                challengesCompleted = true;

                Debug.Log("Cumpliste la meta. (Avanza el siguiente nivel)");

                StartDescendHelicopter();
            }
        }
        // Si solo esta activa la tarea de recolectar cristales
        else if (!usesEnemyQuest && usesCrystalQuest)
        {
            // Si se cumple la tarea de recoger cristales
            if (CrystalCount >= CrystalCollectChallenge)
            {
                challengesCompleted = true;

                Debug.Log("Cumpliste la meta. (Avanza el siguiente nivel)");

                StartDescendHelicopter();
            }
        }
        // Si ninguna de las tareas esta activa
        else if (!usesEnemyQuest && !usesCrystalQuest)
        {
            StartDescendHelicopter();
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

    // Iniciar el tiempo
    public void StartEscapeTimer()
    {
        currentEscapeTime = maxEscapeTime;
        isTimerRunning = true;
        timerText.gameObject.SetActive(true);
    }

    public void StartDescendHelicopter()
    {
        if (usesHelicopter && helicopterScript != null)
        {
            helicopterScript.StartDescent();
        }
    }
}
