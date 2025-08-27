using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownPlayerController : MonoBehaviour
{
    Vector2 _movementInput;
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _maxStamina = 5f;
    private float _currentStamina;
    Rigidbody _rigidbody;
    bool _isSprinting;

    PlayerControls _controls;

    public bool IsMakingNoise;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _controls = new PlayerControls();
        _currentStamina = _maxStamina;
    }

    void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    void OnRun(InputValue value)
    {
        _moveSpeed = value.Get<float>() == 1f ? 10f : 5f;
        _isSprinting = value.Get<float>() == 1f;
    }
   

    void Update()
    {
        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        _rigidbody.linearVelocity = movement * _moveSpeed;
    }
}

public enum NoiseState
{
    Silent,
    Walking,
    Running
}
