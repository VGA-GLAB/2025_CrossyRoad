using System;
using UnityEngine;

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
    /// ���S���̃A�N�V�����i��ɗ��������ꍇ�ȂǂɌĂ΂��j
    /// </summary>
    public Action DeathAction;

    /// <summary>
    /// ���݈ړ������ǂ���
    /// </summary>
    public bool IsMoving { get; private set; } = false;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fixedY = 0.55f;
    [SerializeField] private GridManager gridManager;

    /// <summary>
    /// ���݂̃O���b�h���W
    /// </summary>
    private Vector3Int currentGridPos;
    /// <summary>
    /// �ړ���̃��[���h���W
    /// </summary>
    private Vector3 targetWorldPos;
    /// <summary>
    /// ���ݏ���Ă��鋴�i�Ȃ����null�j
    /// </summary>
    private Transform currentBridge = null;

    private void Start()
    {

        gridManager = FindAnyObjectByType<GridManager>();
        gridManager.RegisterPlayer(gameObject);

        // �O�g���l������1�}�X�i�߂�
        currentGridPos = gridManager.WorldToGrid(transform.position);
        currentGridPos += new Vector3Int(1, 0, 2);

        // �����ʒu���O���b�h�ɑ�����
        //currentGridPos = gridManager.WorldToGrid(transform.position);
        targetWorldPos = gridManager.GridToWorld(currentGridPos);
        targetWorldPos.y = fixedY;
        transform.position = targetWorldPos;
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

            currentBridge = bridgeRoot;
            transform.SetParent(currentBridge);
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
            currentBridge = null;
        }
    }

    /// <summary>
    /// �v���C���[���ړ�������
    /// ���̓x�N�g�����O���b�h�ړ��ɕϊ����A�ړ���Z�������肷��
    /// </summary>
    public void TryMove(Vector2 input)
    {
        if (IsMoving) return;

        // ���̓x�N�g�����O���b�h�����ɕϊ�
        Vector3Int dir = Vector3Int.zero;
        if (input.y > 0) dir = Vector3Int.forward;
        else if (input.y < 0) dir = Vector3Int.back;
        else if (input.x < 0) dir = Vector3Int.left;
        else if (input.x > 0) dir = Vector3Int.right;

        if (dir == Vector3Int.zero) return;

        // �� �{�Ə����̋����F
        // �ړ��J�n���ɋ�����؂藣�����ƂŁu�����Ȃ��v�ɂ���
        if (currentBridge != null)
        {
            transform.SetParent(null);
            currentBridge = null;
        }

        // ���̈ړ���Z�����v�Z
        Vector3Int nextGridPos = currentGridPos + dir;
        var cellType = gridManager.GetCellType(nextGridPos);

        // �ړ���Z�������[���h���W�ɕϊ�
        targetWorldPos = gridManager.GridToWorld(nextGridPos);
        targetWorldPos.y = fixedY;

        currentGridPos = nextGridPos;
        IsMoving = true;
    }

    private void Update()
    {
        if (currentBridge != null && !IsMoving)
        {
            currentGridPos = gridManager.WorldToGrid(transform.position);
        }

        if (!IsMoving) return;
       
        // --- �ʏ�̎��O�ړ� ---
        Vector3 worldMoveDir = targetWorldPos - transform.position;
        Vector3 step = worldMoveDir.normalized * moveSpeed * Time.deltaTime;

        if (worldMoveDir.magnitude <= step.magnitude)
        {
            transform.position = targetWorldPos;
            IsMoving = false;

            var cellType = gridManager.GetCellType(currentGridPos);
            if (cellType == CellType.River && currentBridge == null)
            {
                DeathAction?.Invoke();
                Debug.Log("�����I");
            }

            // ��Z���ȊO�Ɉړ�������e�q�t������
            if (cellType != CellType.River && currentBridge != null)
            {
                //Vector3 worldPos = transform.position;
                transform.SetParent(null);
                //transform.position = worldPos;
                currentBridge = null;
                Debug.Log("������~���i�Z������j");
            }
        }
        else
        {
            transform.position += step;
        }
    }
}
