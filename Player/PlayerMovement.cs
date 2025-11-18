using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; // Asegurarse de que este 'using' este presente

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

    // ======== Variable para el boton de Correr (movil) ========
    [HideInInspector] public bool isSprintingButtonPressed = false;

    public float staminaUseAmount = 5;
    private StaminaBar staminaSlider;

    // ======== Controles Moviles ========
    [Header("Mobile Controls")]
    public GameObject mobileControls;
    public VirtualJoystick virtualJoystick;
    public bool forceMobileControlsInEditor = false;
    // ====================================

    // Inicializar la barra de stamina
    private void Start()
    {
        staminaSlider = FindFirstObjectByType<StaminaBar>();

        bool activateMobile = false;
#if UNITY_ANDROID || UNITY_IOS
        activateMobile = true;
#elif UNITY_EDITOR
        activateMobile = forceMobileControlsInEditor;
#endif
        mobileControls.SetActive(activateMobile);
    }

    Vector3 velocity;

    // Valor de salto
    public float jumpHeight = 3;
    public float jumpCooldown = 0.5f;
    private float nextJumpTime = 0f;

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, sphereRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x;
        float z;

#if UNITY_ANDROID || UNITY_IOS
        x = virtualJoystick.direction.x;
        z = virtualJoystick.direction.y;
#elif UNITY_EDITOR
        if (forceMobileControlsInEditor)
        {
            x = virtualJoystick.direction.x;
            z = virtualJoystick.direction.y;
        }
        else
        {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
        }
#else
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
#endif

        Vector3 move = transform.right * x + transform.forward * z;

        JumpCheck();
        RunCheck(x, z);

        if (!isGrounded)
        {
            characterController.Move(moveInAir * speed * Time.deltaTime * sprintSpeed);
        }
        else
        {
            moveInAir = move;
            characterController.Move(move * speed * Time.deltaTime * sprintSpeed);
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

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
        bool isMobileForced = false;
        #if UNITY_EDITOR
            isMobileForced = forceMobileControlsInEditor;
        #endif

        // Programacion del salto para PC (solo si no estamos en modo movil)
        if (!Application.isMobilePlatform && !isMobileForced)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PerformJump();
            }
        }
    }

    public void RunCheck(float x, float z)
    {
        bool isMoving = (x != 0 || z != 0);
        bool wantsToSprint = false;

        bool isMobileForced = false;
        #if UNITY_EDITOR
            isMobileForced = forceMobileControlsInEditor;
        #endif

        if ((Application.isMobilePlatform || isMobileForced))
        {
            // Logica para movil (o forzado en editor)
            wantsToSprint = isSprintingButtonPressed && isMoving;
        }
        else
        {
            // Logica para PC
            wantsToSprint = Input.GetKey(KeyCode.LeftShift) && isMoving;
        }

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
            sprintSpeed = 1;
        }
    }

    // ======== Metodos para el boton de Correr (movil) ========
    public void StartSprinting()
    {
        isSprintingButtonPressed = true;
    }

    public void StopSprinting()
    {
        isSprintingButtonPressed = false;
    }
    // ==========================================================
}