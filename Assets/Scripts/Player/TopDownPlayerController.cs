using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownPlayerController : MonoBehaviour
{
    Vector2 _movementInput;
    [SerializeField] float _moveSpeed = 5f;

    void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    void Update()
    {
        Vector3 movement = new Vector3(_movementInput.x, 0, _movementInput.y);
        transform.Translate(movement * Time.deltaTime * _moveSpeed);
    }
}
