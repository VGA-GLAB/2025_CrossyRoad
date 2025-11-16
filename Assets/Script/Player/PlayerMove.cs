using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �v���C���[�̈ړ��𐧌䂷��N���X�B
/// CrossyRoad�̂悤�ȃ}�X�P�ʂ̈ړ���z��B
/// �E�ړ��̓O���b�h�P��
/// �E�ړ��J�n���ɃW�����v�A�j���[�V�������Đ��i�Ȃ߂炩�ȓ����j
/// �E��ł͋��̏�ɂ��Ȃ��Ǝ��S
/// </summary>
public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// �X�R�A���Z�̃A�N�V����
    /// </summary>
    public Action OnScoreUpAction;

    /// <summary>
    /// �v���C���[���S���̃A�N�V�����i��ɗ������Ȃǁj
    /// </summary>
    public Action OnPlayerDeathAction;

    /// <summary>
    /// ���݈ړ������ǂ���
    /// </summary>
    public bool IsMoving { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _fixedY = 0.55f;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Button _retryButton;

    [Header("DoTween�̃A�j���[�V�����ݒ�")]
    [SerializeField] private float animTime; // ��: 0.3f ���x
    [Header("�W�����v�ݒ�")]
    [SerializeField] private float jumpHight; // ��: 0.3f ���x
    private bool isJumping; // �W�����v���t���O
    private bool isInputReservation; // ���͗\��t���O
    private Vector3Int inputReservation; // �\�񂳂ꂽ�ړ�����

    /// <summary>
    /// ���݂̃O���b�h���W
    /// </summary>
    private Vector3Int _currentGridPos;
    /// <summary>
    /// �ړ���̃��[���h���W
    /// </summary>
    private Vector3 _targetWorldPos;
    /// <summary>
    /// �X�^�[�g���̃O���b�h���W
    /// </summary> 
    private Vector3Int _startCell;
    /// <summary>
    /// ���ݏ���Ă��鋴�i����Ă��Ȃ��ꍇ��null�j
    /// </summary>
    private Transform _currentBridge = null;
    private Vector3Int _currentCellScore;

    private void Awake()
    {
        if (_retryButton != null)
        {
            _retryButton.onClick.AddListener(ResetPosition);
        }
    }

    private void Start()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
        _gridManager.RegisterPlayer(gameObject);

        // �O�����l�����āA�����ʒu��1�}�X�O�ƉE�ɐi�߂ăX�^�[�g
        _currentGridPos = _gridManager.WorldToGrid(transform.position);
        _currentGridPos += new Vector3Int(2, 0, 2);

        // ���[���h���W�ɕϊ����Ĕz�u
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;

        _startCell = _currentGridPos;
        IsDead = false;
    }

    /// <summary>
    /// ��(Bridge)��Trigger�ɓ������Ƃ��ɋ��̎q�ɐݒ肷��
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("���ɏ����");

            // Bridge�I�u�W�F�N�g�̐e���擾�iroot bridge�j
            Transform bridgeRoot = other.transform.parent != null ? other.transform.parent : other.transform;

            _currentBridge = bridgeRoot;
            transform.SetParent(_currentBridge);
        }
        // ���I��Q���̏Փ˔���
        else if (other.CompareTag("Obstacle"))
        {
            Debug.Log("��Q���ɏՓ� �� ���S");

            IsDead = true;
            OnPlayerDeathAction?.Invoke();
        }
    }

    /// <summary>
    /// ��(Bridge)��Trigger����o���Ƃ��ɐe�q�֌W������
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("�����痣�ꂽ");

            transform.SetParent(null);
            _currentBridge = null;
        }
    }

    /// <summary>
    /// �v���C���[�̈ړ�����
    /// ���̓x�N�g�����O���b�h�����ɕϊ����āA�ړ����ݒ肷��
    /// </summary>
    public void TryMove(Vector2 input)
    {
        if (IsDead) return;

        // ���͕������x�N�g���ɕϊ�
        Vector3Int dir = Vector3Int.zero;
        if (input.y > 0) dir = Vector3Int.forward;
        else if (input.y < 0) dir = Vector3Int.back;
        else if (input.x < 0) dir = Vector3Int.left;
        else if (input.x > 0) dir = Vector3Int.right;

        if (dir == Vector3Int.zero) return;

        // ���̃}�X���W���v�Z
        Vector3Int nextGridPos = _currentGridPos + dir;
        var cellType = _gridManager.GetCellType(nextGridPos);

        // �Z�����؂܂��͋�Ȃ���̓L�����Z��
        if (cellType == CellType.Occupied || cellType == CellType.Empty)
        {
            return;
        }

        // ���̏�ɂ���ꍇ�͈�U����
        if (_currentBridge != null)
        {
            transform.SetParent(null);
            _currentBridge = null;
        }

        // ���[���h���W�ɕϊ�
        _targetWorldPos = _gridManager.GridToWorld(nextGridPos);
        _targetWorldPos.y = _fixedY;

        //_currentGridPos = nextGridPos;
        IsMoving = true;

        // �X�R�A�A�b�v����
        if (_currentGridPos.z > _currentCellScore.z)
        {
            _currentCellScore = _currentGridPos;
            OnScoreUpAction?.Invoke();
            Debug.Log("�X�R�A�A�b�v�I");
        }

        // ���͗\��
        if (isJumping && !isInputReservation)
        {
            isInputReservation = true;
            inputReservation = dir;
        }
    }

    private void Update()
    {
        if (IsDead) return;

        if (_currentBridge != null && !IsMoving)
        {
            _currentGridPos = _gridManager.WorldToGrid(transform.position);
        }

        if (!IsMoving) return;

        // --- �ʏ�̈ړ����� ---
        Vector3 worldMoveDir = _targetWorldPos - transform.position;
        Vector3 step = worldMoveDir.normalized * _moveSpeed * Time.deltaTime;

        if (worldMoveDir.magnitude <= step.magnitude)
        {
            // ���B
            transform.position = _targetWorldPos;
            IsMoving = false;

            isJumping = false; // �W�����v�I��
            //isInputReservation = false;
            //移動が完了したときに、現在のグリッドを更新する
            _currentGridPos = _gridManager.WorldToGrid(_targetWorldPos);

            var cellType = _gridManager.GetCellType(_currentGridPos);
            if (cellType == CellType.River && _currentBridge == null)
            {
                IsDead = true;
                OnPlayerDeathAction?.Invoke();
                Debug.Log("��ɗ��� �� ���S");
                return;
            }

            // ���n�Ɉړ������狴���痣��
            if (cellType != CellType.River && _currentBridge != null)
            {
                transform.SetParent(null);
                _currentBridge = null;
                Debug.Log("�����痣�ꂽ�i���n�ɓ��B�j");
            }

            // �v���C���[�ʒu�A�\���͈͂��X�V
            _gridManager.UpdatePlayerCell(_currentGridPos);
            _gridManager.UpdateStageFlow();
            
            if (isInputReservation) //入力予約があった場合、入力予約時の方向に移動する
            {
                //予約方向をVector２に変換する
                var input2D = new Vector2(inputReservation.x, inputReservation.z);
                TryMove(input2D);
                isInputReservation = false;
            }
        }
        else
        {
            transform.position += step;
            Jumping();
        }
    }

    private void ResetPosition()
    {
        _currentGridPos = _startCell;
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;
        IsMoving = true;
        IsMoving = false;
        IsDead = false;
        transform.SetParent(null);
        _currentBridge = null;
        _currentCellScore = _startCell;
        ScoreManager.instance.ResetScore();
    }

    /// <summary>
    /// DoTween�ɂ��W�����v�A�j���[�V����
    /// </summary>
    private void Jumping()
    {
        if (!isJumping) // ���łɃW�����v���łȂ����
        {
            isJumping = true;
            // �W�����v���o
            transform.DOMoveY(transform.position.y + jumpHight, animTime).SetEase(Ease.OutQuint);
        }
    }
}
