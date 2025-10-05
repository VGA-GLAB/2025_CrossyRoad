using System.Collections;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public bool _isMoving { get; private set; } = false;
    [SerializeField] private float _gridSpace = 1.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    private Vector3 _targetPosition;
    public void Move(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        StartCoroutine(MovePlayer(moveDirection));
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        _isMoving = true;
        Vector3 start = transform.position;
        _targetPosition = start + direction;
        float elapsed = 0f;
        while (elapsed < _gridSpace)
        {
            transform.position = Vector3.Lerp(start, _targetPosition, elapsed / _gridSpace);
            elapsed += Time.deltaTime * _moveSpeed;
            yield return null;
        }
        transform.position = _targetPosition;
        _isMoving = false;
    }
}
