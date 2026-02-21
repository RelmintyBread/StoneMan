using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Player Movement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 5f;
    [Header("References")]
    [SerializeField] private Rigidbody2D _rb;
    private Vector2 _movementInput;

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(_movementInput.x * _speed, _rb.linearVelocity.y);
        _rb.linearVelocity = movement;

        if (_movementInput.y > 0 && InputSystem.GetDevice<Keyboard>().spaceKey.isPressed && Mathf.Abs(_rb.linearVelocity.y) < 0.001f)
        {
            _rb.AddForce(_movementInput * _jumpForce, ForceMode2D.Impulse);
        }
    }

    #region Unity Methods
    void OnMove(InputValue value)
    {
        _movementInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        _movementInput.y = value.Get<float>();
    }
    #endregion
}
