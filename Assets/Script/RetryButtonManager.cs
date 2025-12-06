using UnityEngine;

public class RetryButtonManager : MonoBehaviour
{
    [SerializeField] private PlayerMove _playerMove;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private ResaltManager _resaltManager;

    public void OnRetryButtonPressed()
    {
        _playerMove.ResetPosition();
        _cameraManager.ResetCameraPosition();
        _resaltManager.RetryGame();
    }

}
