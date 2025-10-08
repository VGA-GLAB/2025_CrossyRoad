using System;
using System.Collections;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public Action DeathAction;
    public bool _isMoving { get; private set; } = false;
    [SerializeField] private float _gridSpace = 1.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    private GridManager _gridManager;
    /// <summary>
    /// �����̃O���b�h���W
    /// </summary>
    private Vector3Int _currentCell;
    /// <summary>
    /// �X�^�[�g���̃O���b�h���W
    /// </summary>
    private Vector3Int _startCell;

    private void Start()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
        _gridManager.RegisterPlayer(gameObject);
        _currentCell = _gridManager.WorldToGrid(transform.position);
        _startCell = _currentCell;
    }

    /// <summary>
    /// �v���C���[���ړ�������
    /// </summary>
    /// <param name="input"></param>
    public void Move(Vector2 input)
    {
        if(_isMoving) return;
        //���̓x�N�g�����O���b�h�ړ��ɕϊ�
        Vector3Int moveDirection = new Vector3Int(
            Mathf.RoundToInt(input.x),
            0,
            Mathf.RoundToInt(input.y)
        );
        //���̈ړ���Z�����v�Z
        Vector3Int nextCell = _currentCell + moveDirection;
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
        _isMoving = false;
    }
}
