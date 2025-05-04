using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 10f;

    private Vector2 moveInput;
    private Vector3 currentVelocity;

    private Vector3 initialPosition;

    private void Awake()
    {
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (state == GameState.MainMenu)
            ResetPosition();
    }

    public void OnMoveCamera(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        Vector3 targetVelocity = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, acceleration * Time.deltaTime);
        transform.position += currentVelocity * Time.deltaTime;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
        currentVelocity = Vector3.zero;
    }
}
