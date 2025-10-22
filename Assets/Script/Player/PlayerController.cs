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
         _inputBuffer.MoveAction.started += OnMove;
    }

    private void OnDestroy()
    {
        _inputBuffer.MoveAction.started -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = _inputBuffer.ReadMoveInput();
        _playerMove.TryMove(input);
    }
}
