using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Variable para controlar el movimiento del personaje
    public CharacterController characterController;

    // Variable para controlar la velocidad de movimiento
    public float speed = 10f;
    Vector3 moveInAir;

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

    // ======== Controles Moviles ========
    [Header("Mobile Controls")]
    public GameObject mobileControls;
    public VirtualJoystick virtualJoystick;
    // ====================================

    // Inicializar la barra de stamina
    private void Start()
    {
        // FindObjectOfType (original)
        staminaSlider = FindFirstObjectByType<StaminaBar>();

#if UNITY_ANDROID || UNITY_IOS
        mobileControls.SetActive(true);
#else
        mobileControls.SetActive(false);
#endif
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

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x;
        float z;

#if UNITY_ANDROID || UNITY_IOS
        // Capturar los valores del joystick virtual para desplazar el personaje
        x = virtualJoystick.direction.x;
        z = virtualJoystick.direction.y;
#else
        // Capturar los botones de teclado para desplazar el personaje (WASD o las flechas)
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
#endif

        Vector3 move = transform.right * x + transform.forward * z;

        // Llamado del metodo para saltar
        JumpCheck();

        // Llamado del metodo para correr
        RunCheck(x, z);

        // Si el jugador esta en el aire, moverse de acuerdo a la ultima entrada
        if (!isGrounded)
        {
            characterController.Move(moveInAir * speed * Time.deltaTime * sprintSpeed);
        }
        // Caso contrario, obtener la ultima entrada para moverse en el aire, pero el jugador seguira moviendose normalmente en el suelo
        else
        {
            moveInAir = move;
            characterController.Move(move * speed * Time.deltaTime * sprintSpeed);
        }

        // Aumento de la velocidad de caida si cae desde un punto mas alto.
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    // Metodo para ser llamado desde un boton de UI en movil
    public void PerformJump()
    {
        if (isGrounded && Time.time > nextJumpTime)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    public void JumpCheck()
    {
        // Programacion del salto para PC
#if !UNITY_ANDROID && !UNITY_IOS
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformJump();
        }
#endif
    }

    public void RunCheck(float x, float z)
    {
        bool isMoving = (x != 0 || z != 0);

#if UNITY_ANDROID || UNITY_IOS
        // En movil, el sprint podria ser un boton aparte o basado en la magnitud del joystick.
        // Por ahora, lo dejaremos simple y asumimos que no hay sprint en movil a menos que agregues un boton.
        // Para este ejemplo, el sprint solo funcionara con teclado.
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && isMoving;
#else
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) && isMoving;
#endif

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