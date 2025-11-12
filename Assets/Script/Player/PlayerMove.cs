using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// �v���C���[�̈ړ�������S������N���X
/// �{��CrossyRoad�����̋������Č����邽�߁A
/// �E�ړ��̓O���b�h�P��
/// �E�ړ��J�n���ɋ�����؂藣���i�����Ȃ��j
/// �E���̏�ł͐e�q�t�����ĒǏ]
/// ���������Ă���
/// </summary>
public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// �X�R�A���Z���̃A�N�V����
    /// </summary>
    public Action OnScoreUpAction;

    /// <summary>
    /// ���S���̃A�N�V�����i��ɗ��������ꍇ�ȂǂɌĂ΂��j
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

    [Header("DoTween�̃A�j���[�V��������")]
    [SerializeField] private float animTime; //0.3f�����傤�ǂ���
    [Header("��ԍ���")]
    [SerializeField] private float jumpHight; //0.3f�����傤�ǂ���
    private bool isJumping; //���ł���A�j���[�V������
    private bool isInputReservation; //���͗\��
    private Vector3Int inputReservation; //���͗\�񎞂̈ړ�������ێ�

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
    /// ���ݏ���Ă��鋴�i�Ȃ����null�j
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

        // �O�g���l������1�}�X�i�߂�
        _currentGridPos = _gridManager.WorldToGrid(transform.position);
        _currentGridPos += new Vector3Int(1, 0, 2);

        // �����ʒu���O���b�h�ɑ�����
        //currentGridPos = gridManager.WorldToGrid(transform.position);
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;
        _startCell = _currentGridPos;
        IsDead = false;
    }

    /// <summary>
    /// ����Trigger�ɓ������Ƃ��ɐe�q�t������
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("���ɏ��");

            // Judgment �ɓ��������ꍇ�ł��A1�K�w��� Bridge �{�̂�e�ɂ���
            Transform bridgeRoot = other.transform.parent != null ? other.transform.parent : other.transform;

            _currentBridge = bridgeRoot;
            transform.SetParent(_currentBridge);
        }
    }

    /// <summary>
    /// ����Trigger����o���Ƃ��ɐe�q�֌W����������
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("������~���");

            // ���[���h���W��ێ����Ă���e�q�֌W������
            transform.SetParent(null);
            _currentBridge = null;
        }
    }

    /// <summary>
    /// �v���C���[���ړ�������
    /// ���̓x�N�g�����O���b�h�ړ��ɕϊ����A�ړ���Z�������肷��
    /// </summary>
    public void TryMove(Vector2 input)
    {
        //if (IsMoving||IsDead) return;

        if (IsDead) return;

        // ���̓x�N�g�����O���b�h�����ɕϊ�
        Vector3Int dir = Vector3Int.zero;
        if (input.y > 0) dir = Vector3Int.forward;
        else if (input.y < 0) dir = Vector3Int.back;
        else if (input.x < 0) dir = Vector3Int.left;
        else if (input.x > 0) dir = Vector3Int.right;

        if (dir == Vector3Int.zero) return;

        // �{�Ə����̋����F
        // �ړ��J�n���ɋ�����؂藣�����ƂŁu�����Ȃ��v�ɂ���
        if (_currentBridge != null)
        {
            transform.SetParent(null);
            _currentBridge = null;
        }

        // ���̈ړ���Z�����v�Z
        Vector3Int nextGridPos = _currentGridPos + dir;
        var cellType = _gridManager.GetCellType(nextGridPos);

        // �ړ���Z�������[���h���W�ɕϊ�
        _targetWorldPos = _gridManager.GridToWorld(nextGridPos);
        _targetWorldPos.y = _fixedY;

        _currentGridPos = nextGridPos;
        IsMoving = true;

        if (_currentGridPos.z >_currentCellScore.z)
        {
            _currentCellScore = _currentGridPos;
            OnScoreUpAction?.Invoke();
            Debug.Log("�X�R�A�A�b�v�I");
        }

        //���͗\��
        if (isJumping && !isInputReservation)
        {
            isInputReservation = true;
            inputReservation = dir;
            //Debug.Log("�A�j���[�V�������ɓ��͗\��");
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
       
        // --- �ʏ�̎��O�ړ� ---
        Vector3 worldMoveDir = _targetWorldPos - transform.position;
        Vector3 step = worldMoveDir.normalized * _moveSpeed * Time.deltaTime;

        if (isInputReservation) //���͗\��̏���
        {
            //��O�̃Z�����擾����
            //��̃Z����_currentGridPos�ɓ����Ă邩��A�O��̓��͂������ďグ��
            var cellType = _gridManager.GetCellType(_currentGridPos - inputReservation);
            //Raycast�ŉ��̃R���C�_�[���`�F�b�N����
            if (Physics.Raycast(transform.position + Vector3.down * 0.5f, Vector3.down, out RaycastHit hit, 2f))
            {
                if (hit.collider.CompareTag("Bridge"))
                {
                    if (cellType != CellType.River && _currentBridge != null)
                    {
                        isInputReservation = false;
                        transform.SetParent(null);
                        _currentBridge = null;
                        Debug.Log("������~���i�Z������j");
                    }
                }
                else //������Ȃ������Ƃ�
                {
                    if (cellType == CellType.River && _currentBridge == null)
                    {
                        isInputReservation = false;
                        IsDead = true;
                        OnPlayerDeathAction?.Invoke();
                        //Debug.Log("���͗\��ɂ�闎��");
                        return;
                    }
                }
            }
        }

        if (worldMoveDir.magnitude <= step.magnitude)
        {
            transform.position = _targetWorldPos;
            IsMoving = false;

            isJumping = false; //��ԃA�j���[�V�������I��
            isInputReservation = false;

            var cellType = _gridManager.GetCellType(_currentGridPos);
            if (cellType == CellType.River && _currentBridge == null)
            {
                IsDead = true;
                OnPlayerDeathAction?.Invoke();
                //_currentGridPos = _startCell;
                Debug.Log("�����I");
                return;
            }

            // ��Z���ȊO�Ɉړ�������e�q�t������
            if (cellType != CellType.River && _currentBridge != null)
            {
                //Vector3 worldPos = transform.position;
                transform.SetParent(null);
                //transform.position = worldPos;
                _currentBridge = null;
                Debug.Log("������~���i�Z������j");
            }

            // �`��͈͂̍X�V
            _gridManager.UpdateRenderArea();
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
    /// DoTween�ɂ���ԃA�j���[�V����
    /// </summary>
    private void Jumping()
    {
        if (!isJumping) //���ł��Ȃ��Ƃ�
        {
            isJumping = true;

            //��ԃA�j���[�V����
            transform.DOMoveY(transform.position.y + jumpHight, animTime).SetEase(Ease.OutQuint);
        }
    }
}
