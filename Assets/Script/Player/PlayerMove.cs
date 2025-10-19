using System;
using System.Collections;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// �X�R�A�A�b�v�̃A�N�V����
    /// </summary>
    public Action OnScoreUpAction;
    /// <summary>
    /// ���S���̃A�N�V����
    /// </summary>
    public Action OnPlayerDeathAction;
    public bool _isMoving { get; private set; } = false;
    [SerializeField] private float _gridSpace = 1.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GroundCheck _groundCheck;
    /// <summary>
    /// �����̃O���b�h���W
    /// </summary>
    private Vector3Int _currentCell;
    /// <summary>
    /// �X�^�[�g���̃O���b�h���W
    /// </summary>
    private Vector3Int _startCell;
    private Vector3Int _currentCellScore;

    private void Awake()
    {
        if (_groundCheck == null)
        {
            _groundCheck = FindAnyObjectByType<GroundCheck>();
        }
        _groundCheck.OnPlayerRiverDie += OnRiverDeathAction;
    }

    private void Start()
    {
        if (_gridManager == null)
        {
            _gridManager = FindAnyObjectByType<GridManager>();
        }
        _gridManager.RegisterPlayer(gameObject);
        _currentCell = _gridManager.WorldToGrid(transform.position);
        _startCell = _currentCell;
    }

    private void OnDestroy()
    {
        _groundCheck.OnPlayerRiverDie -= OnRiverDeathAction;
    }

    /// <summary>
    /// �v���C���[���ړ�������
    /// </summary>
    /// <param name="input"></param>
    public void Move(Vector2 input)
    {
        if (_isMoving) return;
        //���̓x�N�g�����O���b�h�ړ��ɕϊ�
        Vector3Int moveDirection = new Vector3Int(
            Mathf.RoundToInt(input.x),
            0,
            Mathf.RoundToInt(input.y)
        );
        //���̈ړ���Z�����v�Z
        Vector3Int nextCell = _currentCell + moveDirection;
        //TODOnextCell������x,y���玟�̃Z���ɂ���
        //�ړ���Z�����󂢂Ă��邩�m�F
        if (!_gridManager.IsCellFree(nextCell)) return;
        StartCoroutine(MovePlayer(nextCell));
    }

    /// <summary>
    /// �v���C���[�����̃Z���Ɉړ�������R���[�`��
    /// </summary>
    private IEnumerator MovePlayer(Vector3Int targetCell)
    {
        _isMoving = true;
        // ���݈ʒu�ƖړI�n�i���[���h���W�j���擾
        Vector3 start = transform.position;
        //�ړI�n�Z����ϊ�
        Vector3 end = _gridManager.GridToWorld(targetCell);
        float t = 0f;
        while (t < _gridSpace)
        {
            transform.position = Vector3.Lerp(start, end, t / _gridSpace);
            t += Time.deltaTime * _moveSpeed;
            yield return null;
        }
        // �ŏI�I�ɖړI�n�ɐ��m�ɐݒ�
        transform.position = end;
        // ���݂̃Z�������X�V
        _currentCell = targetCell;
        _gridManager.UpdatePlayerCell(targetCell);
        //if (_gridManager.GetCellType(_currentCell) == CellType.River)
        //{
        //    OnPlayerDeathAction?.Invoke();
        //    _currentCell = _startCell;
        //    transform.position = _gridManager.GridToWorld(_startCell);
        //    _gridManager.UpdatePlayerCell(_startCell);
        //    _isMoving = false;
        //    yield break;
        //}
        if (_currentCell.z > _currentCellScore.z)
        {
            _currentCellScore = _currentCell;
            OnScoreUpAction?.Invoke();
        }
        _isMoving = false;
    }

    private void OnRiverDeathAction()
    {
        Debug.Log("Player Dead");
        OnPlayerDeathAction?.Invoke();
        _currentCell = _startCell;
        transform.position = _gridManager.GridToWorld(_startCell);
        _gridManager.UpdatePlayerCell(_startCell);
        _isMoving = false;
    }
}
