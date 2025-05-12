using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private Transform cameraHolder;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float verticalClamp = 80f;

    private CharacterController controller;
    private float verticalLookRotation = 0;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked; // Verrouille le curseur au centre de l'écran
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (controller == null) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Mouvement vers l'avant
        Vector3 move = transform.right * input.x * moveSpeed + transform.forward * input.y * moveSpeed;
        controller.SimpleMove(move);
    }

    private void HandleRotation()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>() * rotationSpeed * Time.deltaTime;

        // Rotation horizontale (gauche/droite)
        transform.Rotate(Vector3.up * look.x);

        // Rotation verticale (haut/bas) avec clamp
        verticalLookRotation -= look.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -verticalClamp, verticalClamp);

        cameraHolder.localEulerAngles = new Vector3(verticalLookRotation, 0f, 0f);
    }
}
