using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private Transform _cameraFollowTransform;
    [SerializeField] private Transform _playerPosition;
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private float _cameraZUpdateSpeed = 0.5f;
    [SerializeField] private Button _retryButton;
    private GameManager _gameManager;
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

        if (_gameManager == null)
        {
            _gameManager = FindAnyObjectByType<GameManager>();
        }
        //_startPos = _cameraFollowTransform.position;
        _startPos = _cameraFollowTransform.position;
        _startPos.x = _playerPosition.position.x;
        _cameraFollowTransform.position = _startPos;

        if (_camera != null && _playerPosition != null)
        {
            // Inspectorで FollowTarget にプレイヤーを設定済みと仮定
            // 初期位置をコードで明示的に調整
            _camera.transform.position = _playerPosition.position + new Vector3(0f, 12f, -6f);
            _camera.transform.LookAt(_playerPosition); // 必要なら注視方向も設定
        }
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
        else if (_gameManager.IsInGamePlay)
        {
            //徐々にカメラを前進させる
            _camTargetPos.z += _cameraZUpdateSpeed * Time.deltaTime;
        }
        _camTargetPos.x = _playerPosition.position.x;
        _camTargetPos.y = _playerPosition.position.y;
        _cameraFollowTransform.position = _camTargetPos;
    }

    private void LateUpdate()
    {
        //画面外に出たら死亡処理
        if (!IsInScrean(Camera.main, _playerPosition.position))
        {
            _playerMove.OnPlayerDeathAction();
        }
    }
    private void ResetCameraPosition()
    {
        //カメラを初期位置にリセット
        _cameraFollowTransform.position = _startPos;
        _camTargetPos = _startPos;
        Debug.Log("CameraReset");
    }

    private bool IsInScrean(Camera cam, Vector3 pos)
    {
        var viewPos = cam.WorldToViewportPoint(pos);
        return (0 <= viewPos.x && viewPos.x <= 1) && (0 <= viewPos.y && viewPos.y <= 1);
    }
}
