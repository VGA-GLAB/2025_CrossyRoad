using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    private InputBuffer _inputBuffer;
    private PlayerMove _playerMove;
    private GameManager _gameManager;

    private void Awake()
    {
        _inputBuffer = GetComponent<InputBuffer>();
        _playerMove = GetComponent<PlayerMove>();
        if (_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }
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
        if (_gameManager.IsInGamePlay)
        {
            _playerMove.TryMove(input);
        }
    }
}
