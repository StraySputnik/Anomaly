using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 100.0f;
    [SerializeField] private float acceleration = 50.0f;
    [SerializeField] private float friction = 10.0f;
    [SerializeField] private float gravity = -20.0f;
    [SerializeField] private float jumpHeight = 1.5f;

    [SerializeField] private Vector2 mouseSensitivity = new(0.5f, 0.5f);
    [SerializeField] private float lookUpLimit = 80.0f;
    [SerializeField] private float lookDownLimit = 89.0f;

    [SerializeField] private float footstepInterval = 1.0f;

    private CharacterController _cc;
    private Camera _cam;
    private Vector3 _xVelocity;
    private float _yVelocity;
    private float _pitch;
    private float _footstepTimer;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _cam = GetComponentInChildren<Camera>();
        if (_cam == null)
            Debug.LogError("No Camera found on this object or its children");

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdatePosition(Time.deltaTime);
        UpdateRotation();
    }

    private void UpdatePosition(float dt)
    {
        var kb = Keyboard.current;
        var input = GetMoveInput();
        var wishDir = transform.right * input.x + transform.forward * input.y;

        _xVelocity += wishDir * (acceleration * dt);
        _xVelocity = Vector3.ClampMagnitude(_xVelocity, maxSpeed);
        _xVelocity *= Mathf.Exp(-friction * dt);

        bool grounded = _cc.isGrounded;
        if (grounded && _yVelocity < 0.0f)
            _yVelocity = -2.0f;

        if (grounded && kb != null && kb.spaceKey.wasPressedThisFrame)
            _yVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);

        _yVelocity += gravity * dt;

        var motion = _xVelocity + Vector3.up * _yVelocity;
        _cc.Move(motion * dt);

        if (input == Vector2.zero) return;

        _footstepTimer += Time.deltaTime;
        if (!(_footstepTimer >= footstepInterval)) return;

        _footstepTimer -= footstepInterval;
        AudioManager.Instance.PlaySound("footstep");
    }

    private void UpdateRotation()
    {
        if (Mouse.current == null) return;
        var delta = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up, delta.x * mouseSensitivity.x, Space.World);
        _pitch = Mathf.Clamp(_pitch - delta.y * mouseSensitivity.y, -lookUpLimit, lookDownLimit);
        _cam.transform.localRotation = Quaternion.Euler(_pitch, 0.0f, 0.0f);
    }

    private static Vector2 GetMoveInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return Vector2.zero;

        var dir = new Vector2(
            (kb.dKey.isPressed ? 1 : 0) - (kb.aKey.isPressed ? 1 : 0),
            (kb.wKey.isPressed ? 1 : 0) - (kb.sKey.isPressed ? 1 : 0)
        );

        return dir.normalized;
    }
}