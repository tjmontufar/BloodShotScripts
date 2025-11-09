using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Variable para controlar el movimiento del personaje
    public CharacterController characterController;

    // Variable para controlar la velocidad de movimiento
    public float speed = 10f;

    // Variable para almacenar la gravedad
    private float gravity = -9.81f;

    // Variables para comprobar si el personaje esta tocando el suelo
    public Transform groundCheck;
    public float sphereRadius = 0.3f;
    public LayerMask groundMask;
    bool isGrounded;

    // Variables para correr
    public bool isSprinting;
    public float sprintingSpeedMultiplier = 1.5f;
    private float sprintSpeed = 1;

    public float staminaUseAmount = 5;
    private StaminaBar staminaSlider;

    // Inicializar la barra de stamina
    private void Start()
    {
        // FindObjectOfType (original)
        staminaSlider = FindFirstObjectByType<StaminaBar>();
    }

    Vector3 velocity;

    // Valor de salto
    public float jumpHeight = 3;
    public float jumpCooldown = 0.5f;
    private float nextJumpTime = 0f;

    void Update()
    {
        // Identificar si el jugador esta tocando el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Capturar los botones de teclado para desplazar el personaje (WASD o las flechas)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Llamado del metodo para saltar
        JumpCheck();

        // Llamado del metodo para correr
        RunCheck();

        characterController.Move(move * speed * Time.deltaTime * sprintSpeed);

        // Aumento de la velocidad de caida si cae desde un punto mas alto.
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

    public void JumpCheck()
    {
        // Programacion del salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && Time.time > nextJumpTime)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    public void RunCheck()
    {
        // Capturar el movimiento horizontal/vertical para saber si el jugador realmente se mueve
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isMoving = (x != 0 || z != 0);

        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && isMoving;

        bool canSprint = staminaSlider.IsStaminaAvailable();

        if (wantsToSprint && canSprint)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            sprintSpeed = sprintingSpeedMultiplier;
        }
        else
        {
            // Velocidad normal
            sprintSpeed = 1;
        }
    }
}
