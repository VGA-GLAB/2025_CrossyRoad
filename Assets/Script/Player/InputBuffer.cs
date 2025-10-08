using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]
public class InputBuffer : MonoBehaviour
{
    private const string MOVE_ACTION = "Move";

    public InputAction MoveAction => _moveAction;
    private InputAction _moveAction;

    private void Awake()
    {
        if (TryGetComponent<PlayerInput>(out var playerInput))
        {
            _moveAction = playerInput.actions[MOVE_ACTION];
        }
    }
}
