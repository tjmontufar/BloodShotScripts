using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private Transform playerTransform;

    private Animator animator;

    // Daño que el enemigo hace al jugador
    public int enemyPowerDamange = 10;

    // Distancia para que el enemigo comience a atacar
    public float attackRange = 5.0f;

    public float attackCoolDown = 1.5f; // Retardo de ataques
    public float lastAttackTime;

    // Banderin para saber si el jugador esta en el campo de vision
    private bool isPlayerInRange;

    // Valor inicial de salud del enemigo
    public int health = 100;
    private bool isDead = false;
    private Collider myCollider;

    // Puntos por recibir al eliminar el enemigo;
    public int EnemyKillScore = 0;

    // Variables para establecer patrullaje
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float waypointTolerance = 1f;
    private int currentPatrolIndex;

    // Efectos de daño
    public float flashDuration = 0.1f;
    public Color damangeColor = new Color(1f, 0f, 0f, 0.5f);
    private Color originalColor;
    private Renderer enemyRenderer;

    public GameObject bloodDecalPrefab;

    void Awake()
    {
        myCollider = GetComponent<Collider>();
        if (myCollider == null)
        {
            myCollider = GetComponentInChildren<Collider>();
        }
    }

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;

        isPlayerInRange = false;

        // Logica del patrullaje
        currentPatrolIndex = 0;
        navMeshAgent.speed = patrolSpeed;

        enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
        else
        {

        }

    }

    void Update()
    {
        // Comprobar si el enemigo ha muerto
        if (isDead)
        {
            return;
        }

        // Cuando el jugador entre en la vision del enemigo, este lo seguira
        if (isPlayerInRange)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }
    }

    // Funcion para manejo de la logica del patrullaje desde un punto a otro
    void setNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        navMeshAgent.destination = patrolPoints[currentPatrolIndex].position;

        animator.SetBool("IsAttacking", false);
        animator.SetFloat("XAxis", 0.75f);

        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.isStopped = false;
    }

    // Funcion que verifica si el enemigo llego al destino
    void Patrol()
    {
        navMeshAgent.speed = patrolSpeed;
        animator.SetBool("IsAttacking", false);
        animator.SetFloat("XAxis", 0.75f);

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

            setNextPatrolPoint();
        }
    }

    // Metodo para perseguir el jugador
    void FollowPlayer()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > attackRange)
        {
            // Velocidad de persecucion
            navMeshAgent.speed = 3.5f;

            // Perseguir al jugador
            navMeshAgent.isStopped = false;

            // Ejecutar animaciones de movimiento y desactivar las de ataque
            animator.SetBool("IsAttacking", false);
            animator.SetFloat("XAxis", 1f);

            navMeshAgent.destination = playerTransform.position;

        }
        else
        {
            AttackPlayer();
        }
    }

    // Metodo para ejecutar ataques
    void AttackPlayer()
    {
        // Si el enemigo ha muerto, omitir esta funcion
        if (isDead)
        {
            return;
        }

        // Forzar la rotacion hacia el jugador si no ha completado la transicion de ataque
        if (playerTransform != null)
        {
            // Calcular la dirección hacia el jugador
            Vector3 lookDirection = playerTransform.position - transform.position;
            lookDirection.y = 0; // Asegurarse de que el giro es solo horizontal (en el plano Y)

            // Crear la rotación de destino
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

            // Aplicar la rotación INSTANTÁNEAMENTE para que mire al jugador
            transform.rotation = targetRotation;
        }

        // Detenerse y atacar al jugador
        navMeshAgent.isStopped = true;

        // Ejecutar animacion de ataque
        animator.SetBool("IsAttacking", true);

        // Retardo para que el enemigo no ataque cada frame
        if (Time.time > lastAttackTime + attackCoolDown)
        {
            Debug.Log("El enemigo ataca al jugador");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseHealth(enemyPowerDamange);
            }

            lastAttackTime = Time.time;
        }
    }

    // Al mantenerse dentro del Trigger
    private void OnTriggerEnter(Collider other)
    {
        // Cuando el jugador entre en el campo de vision del enemigo
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Cuando el jugador salga del campo de vision del enemigo
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    // Metodo para contrarrestar vida del enemigo
    public void LoseEnemyHealth(int damange)
    {
        if (isDead)
        {
            return;
        }

        health -= damange;
        Debug.Log("Vida restante del enemigo: " + health);

        if (health <= 0)
        {
            Die();
        }

        if (enemyRenderer != null)
        {
            StartCoroutine(FlashDamangeEffect());
        }
    }

    // Corotuine para efecto de paradeo rojo al recibir daño
    private IEnumerator FlashDamangeEffect()
    {
        enemyRenderer.material.color = damangeColor;

        yield return new WaitForSeconds(flashDuration);

        enemyRenderer.material.color = originalColor;
    }

    // Funcion para animar y eliminar el enemigo si muere
    public void Die()
    {
        isDead = true;
        Debug.Log("Enemigo derrotado.");

        StopAllCoroutines();

        if (myCollider != null)
        {
            myCollider.enabled = false;
        }

        // Colocar una calcomania para charco de sangre
        if (bloodDecalPrefab != null)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y += 0.05f;

            Instantiate(bloodDecalPrefab, spawnPosition, Quaternion.identity);
        }

        GameManager.Instance.EnemyKillCount += 1;
        GameManager.Instance.CurrentScore += EnemyKillScore;
        GameManager.Instance.CheckQuestProgression();

        // El enemigo se detiene por completo
        navMeshAgent.isStopped = true;
        isPlayerInRange = false;
        animator.SetBool("IsAttacking", false);

        // Inicia la animacion de muerte
        animator.SetBool("IsDead", true);

        StartCoroutine(DestroyAfterAnimation());

        
    }

    // Eliminar el enemigo del mapa despues de la animacion de muerte
    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }
}
