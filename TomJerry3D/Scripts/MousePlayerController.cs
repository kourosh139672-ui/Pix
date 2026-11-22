using UnityEngine;

/// <summary>
/// First-person controller for the mouse. Add a CharacterController to this object
/// and assign a child camera pivot at the mouse's eye height.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class MousePlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float gravity = -18f;

    [Header("Look")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private float lookSensitivity = 0.12f;
    [SerializeField] private float maxLookAngle = 80f;

    private CharacterController controller;
    private Vector2 uiMove;
    private Vector2 uiLookDelta;
    private float verticalVelocity;
    private float pitch;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraPivot == null && Camera.main != null)
            cameraPivot = Camera.main.transform;
    }

    private void Update()
    {
        Vector2 keyboardMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 moveInput = uiMove.sqrMagnitude > 0.001f
            ? uiMove
            : Vector2.ClampMagnitude(keyboardMove, 1f);

        Vector3 direction = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;

        verticalVelocity += gravity * Time.deltaTime;
        Vector3 velocity = direction * moveSpeed;
        velocity.y = verticalVelocity;
        controller.Move(velocity * Time.deltaTime);

        // Touch look input is accumulated from drag events and consumed once per frame.
        Vector2 lookInput = uiLookDelta;
        uiLookDelta = Vector2.zero;
        if (lookInput.sqrMagnitude < 0.001f)
            lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        transform.Rotate(Vector3.up, lookInput.x * lookSensitivity, Space.World);
        if (cameraPivot != null)
        {
            pitch = Mathf.Clamp(pitch - lookInput.y * lookSensitivity, -maxLookAngle, maxLookAngle);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    public void MoveFromUI(Vector2 value) => uiMove = Vector2.ClampMagnitude(value, 1f);
    public void LookFromUI(Vector2 delta) => uiLookDelta += delta;
    public void StopMoveFromUI() => uiMove = Vector2.zero;
    public void StopLookFromUI() => uiLookDelta = Vector2.zero;
}
