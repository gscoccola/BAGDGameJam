using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownPlayerController : MonoBehaviour
{
    Vector2 _movementInput;
    [SerializeField] float _moveSpeed = 5f;
    Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
        
    }

    void Update()
    {
        //if (_movementInput == null) _rigidbody.linearVelocity = Vector3.zero;
        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        _rigidbody.linearVelocity = movement * _moveSpeed;
    }
}
