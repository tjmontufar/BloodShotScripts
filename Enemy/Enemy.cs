using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    private Transform playerTransform;

    private Animator animator;

    // Distancia para que el enemigo comience a atacar
    public float attackRange = 5.0f;

    public float attackCoolDown = 1.5f; // Retardo de ataques
    public float lastAttackTime;

    // Banderin para saber si el jugador esta en el campo de vision
    private bool isPlayerInRange;

    // Valor inicial de salud del enemigo
    public int health = 100;

    // Variables para establecer patrullaje
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float waypointTolerance = 1f;
    private int currentPatrolIndex;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;

        isPlayerInRange = false;

        // Logica del patrullaje
        currentPatrolIndex = 0;
        navMeshAgent.speed = patrolSpeed;
    }

    void Update()
    {
        // Cuando el jugador entre en la vision del enemigo, este lo seguira
        if (isPlayerInRange)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
            // El enemigo se detiene
            // navMeshAgent.isStopped = true;
            // Ejecutar animaciones de movimiento y desactivar las de ataque
            // animator.SetBool("IsAttacking", false);
            // Ejecuta animacion de reposo
            // animator.SetFloat("XAxis", 0f);
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
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < waypointTolerance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;

            setNextPatrolPoint();
        }
    }

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
        // Detenerse y atacar al jugador
        navMeshAgent.isStopped = true;

        // Ejecutar animacion de ataque
        animator.SetBool("IsAttacking", true);

        // Retardo para que el enemigo no ataque cada frame
        if (Time.time > lastAttackTime + attackCoolDown)
        {
            Debug.Log("El enemigo ataca al jugador");
            GameManager.Instance.LoseHealth(10);
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
    public void LoseEnemyHealth(int healthToReduce)
    {
        health -= healthToReduce;
    }

    public bool CheckEnemyHealth()
    {
        if(health ==  0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Funcion para animar y eliminar el enemigo si muere
    public void Die()
    {
        // El enemigo se detiene por completo
        navMeshAgent.isStopped = true;
        isPlayerInRange = false;
        animator.SetBool("IsAttacking", false);

        // Inicia la animacion de muerte
        animator.SetBool("IsDead", true);

        StartCoroutine(DestroyAfterAnimation());

        GameManager.Instance.EnemyKillCount++;
    }

    // Eliminar el enemigo del mapa despues de la animacion de muerte
    private IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }
}
