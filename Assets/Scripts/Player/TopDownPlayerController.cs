using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TopDownPlayerController : MonoBehaviour
{
    Vector2 _movementInput;
    [SerializeField] float _baseSpeed;
    float _moveSpeed;
    [SerializeField] float _sprintSpeed;
    [SerializeField] float _maxStamina = 5f;
    [SerializeField] Slider _slider;
    private float _currentStamina;
    Rigidbody _rigidbody;
    bool _isSprinting;

    PlayerControls _controls;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _controls = new PlayerControls();
        _currentStamina = _maxStamina;
        _slider.maxValue = _maxStamina;
    }

    void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    void OnRun(InputValue value)
    {
        _isSprinting = value.Get<float>() == 1f && _currentStamina > 0f;
    }

    


    void Update()
    {
        _slider.value = _currentStamina;

        if (_currentStamina < 0f) _isSprinting = false;

        if (_isSprinting && _currentStamina > 0f)
        {
            _moveSpeed = _sprintSpeed;
            if (_movementInput != Vector2.zero)
            {
                _currentStamina -= Time.deltaTime;
            }
        }
        else
        {
            _currentStamina += Time.deltaTime;
            _moveSpeed = _baseSpeed;
        }
            

        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        _rigidbody.linearVelocity = movement * _moveSpeed;

    }
}

