using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private InputBuffer _inputBuffer;
    private PlayerMove _playerMove;

    private void Awake()
    {
        _inputBuffer = GetComponent<InputBuffer>();
        _playerMove = GetComponent<PlayerMove>();
    }

    private void Start()
    {
        _inputBuffer.MoveAction.started += Move;
    }

    private void OnDestroy()
    {
        _inputBuffer.MoveAction.started -= Move;
    }

    private void Move(InputAction.CallbackContext context)
    {
        _playerMove.Move(context.ReadValue<Vector2>());
    }
}
