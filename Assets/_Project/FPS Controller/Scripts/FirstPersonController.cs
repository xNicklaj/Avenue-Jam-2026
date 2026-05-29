using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Riferimenti")]
    [Tooltip("La camera principale attaccata alla testa del giocatore")]
    [SerializeField] private Transform playerCamera;
    private CharacterController characterController;

    [Header("Input (New Input System)")]
    [Tooltip("Azione di tipo Value -> Vector2 per il movimento (es. WASD/Left Stick)")]
    [SerializeField] private InputActionReference moveAction;
    [Tooltip("Azione di tipo Value -> Vector2 per la visuale (es. Mouse Delta/Right Stick)")]
    [SerializeField] private InputActionReference lookAction;

    [Header("Fisica e Inerzia del Movimento")]
    [SerializeField] private float maxSpeed = 4.0f;
    [SerializeField] private float acceleration = 8.0f;
    [SerializeField] private float deceleration = 12.0f;
    
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;

    [Header("Feeling della Visuale")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float gamepadSensitivity = 2.0f;
    [Tooltip("Aggiunge un leggero ritardo per simulare il peso del corpo/testa")]
    [SerializeField] private float cameraSmoothTime = 0.03f; 
    
    private float cameraPitch = 0.0f;
    private float cameraYaw = 0.0f;
    private Vector2 currentLookDelta;
    private Vector2 lookVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
        // Setup iniziale per nascondere e bloccare il cursore al centro dello schermo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
        if (lookAction != null) lookAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
        if (lookAction != null) lookAction.action.Disable();
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 rawLookInput = lookAction.action.ReadValue<Vector2>();
        
        // Smoothing per dare un senso di inerzia meccanica e peso alla visuale
        currentLookDelta = Vector2.SmoothDamp(currentLookDelta, rawLookInput, ref lookVelocity, cameraSmoothTime);

        // Rilevamento dinamico del device per scalare la sensitivity
        bool isMouse = lookAction.action.activeControl?.device is Pointer;
        float currentSensitivity = isMouse ? mouseSensitivity : gamepadSensitivity;

        cameraYaw += currentLookDelta.x * currentSensitivity;
        cameraPitch -= currentLookDelta.y * currentSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -85.0f, 85.0f);

        // Applica la rotazione verticale solo alla camera
        playerCamera.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);
        // Applica la rotazione orizzontale all'intero corpo del player
        transform.rotation = Quaternion.Euler(0.0f, cameraYaw, 0.0f);
    }

    private void HandleMovement()
    {
        Vector2 inputDir = moveAction.action.ReadValue<Vector2>();
        
        // Calcola la direzione basandosi sull'orientamento del corpo
        Vector3 targetDirection = (transform.right * inputDir.x + transform.forward * inputDir.y).normalized;
        targetVelocity = targetDirection * maxSpeed;

        // Gestione dell'inerzia: accelerazione e decelerazione distinte
        float currentAccel = (inputDir.magnitude > 0.1f) ? acceleration : deceleration;
        currentVelocity.x = Mathf.Lerp(currentVelocity.x, targetVelocity.x, currentAccel * Time.deltaTime);
        currentVelocity.z = Mathf.Lerp(currentVelocity.z, targetVelocity.z, currentAccel * Time.deltaTime);

        // Gravità di base per il CharacterController
        if (!characterController.isGrounded)
        {
            currentVelocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else if (currentVelocity.y < 0)
        {
            currentVelocity.y = -2f; // Snap al suolo
        }

        characterController.Move(currentVelocity * Time.deltaTime);
    }
}