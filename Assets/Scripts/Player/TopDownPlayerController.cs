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
    [SerializeField] float _sheepBaseSpeed;
    [SerializeField] float _sheepSprintSpeed;
    [SerializeField] float _maxStamina = 5f;
    [SerializeField] Slider _slider;
    private float _currentStamina;
    Rigidbody _rigidbody;
    bool _isSprinting;
    bool _isCarryingSheep;
    public Sheep _sheepInContact;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
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

    void OnInteract(InputValue value)
    {
        if (!_isCarryingSheep && _sheepInContact != null)
        {
            _sheepInContact.OnPickup(transform);
            _isCarryingSheep = true;
        }
        else if (_isCarryingSheep)
        {
            _sheepInContact.OnDrop();
            _isCarryingSheep = false;

        }
    }

    


    void Update()
    {
        _slider.value = _currentStamina;

        if (_currentStamina < 0f) _isSprinting = false;

        if (_isSprinting && _currentStamina > 0f)
        {
            _moveSpeed = _isCarryingSheep ? _sheepSprintSpeed : _sprintSpeed;
            if (_movementInput != Vector2.zero)
            {
                _currentStamina -= Time.deltaTime;
            }
        }
        else
        {
            if (_currentStamina < _maxStamina)  _currentStamina += Time.deltaTime;
            _moveSpeed = _isCarryingSheep ? _sheepBaseSpeed : _baseSpeed;
        }
            

        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        _rigidbody.linearVelocity = movement * _moveSpeed;

    }
}

