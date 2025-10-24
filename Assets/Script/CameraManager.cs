using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _cameraFollowTransform;
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private float _cameraZUpdateSpeed = 0.5f;
    [SerializeField] private Button _retryButton;
    private Vector3 _startPos;
    private Vector3 _camTargetPos;

    private void Awake()
    {
        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(ResetCameraPosition);
        }
    }

    private void Start()
    {
        if (_playerMove == null)
        {
            _playerMove = FindAnyObjectByType<PlayerMove>();
        }
        _startPos = _cameraFollowTransform.position;
    }

    private void Update()
    {
        if (_playerMove != null && _playerMove.IsDead) return;
        _camTargetPos = _cameraFollowTransform.position;
        if (_camTargetPos.z < _playerPosition.position.z)
        {
            //追い越したときカメラをプレイヤーの位置に合わせる
            _camTargetPos.z = _playerPosition.position.z;
        }
        else
        {
            //徐々にカメラを前進させる
            _camTargetPos.z += _cameraZUpdateSpeed * Time.deltaTime;
        }
        _camTargetPos.x = _playerPosition.position.x;
        _camTargetPos.y = _playerPosition.position.y;
        _cameraFollowTransform.position = _camTargetPos;
    }

    private void ResetCameraPosition()
    {
        //カメラを初期位置にリセット
        _cameraFollowTransform.position = _startPos;
        _camTargetPos = _startPos;
        Debug.Log("CameraReset");
    }
}
