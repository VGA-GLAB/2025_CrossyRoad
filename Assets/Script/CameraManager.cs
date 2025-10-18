using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _cameraFollowTransform;
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private float _cameraZUpdateSpeed = 0.5f;
    private Transform _startPos;
    private Vector3 _camTargetPos;
    private void Start()
    {
        if (_playerMove == null)
        {
            _playerMove = FindAnyObjectByType<PlayerMove>();
        }

        _playerMove.OnPlayerDeathAction += OnPlayerDeath;
        _startPos = _playerPosition;
    }

    private void OnDestroy()
    {
        if (_playerMove != null)
        {
            _playerMove.OnPlayerDeathAction -= OnPlayerDeath;
        }
    }

    private void Update()
    {
        _camTargetPos = _cameraFollowTransform.position;
        if (_camTargetPos.z < _playerPosition.position.z)
        {
            _camTargetPos.z = _playerPosition.position.z;
        }
        else
        {
            _camTargetPos.z += _cameraZUpdateSpeed * Time.deltaTime;
        }
        _camTargetPos.x = _playerPosition.position.x;
        _camTargetPos.y = _playerPosition.position.y;
        _cameraFollowTransform.position = _camTargetPos;
    }

    private void OnPlayerDeath()
    {
        _cameraFollowTransform.position = _startPos.position;
        _camTargetPos = _startPos.position;
        Debug.Log("CameraReset");
    }
}
