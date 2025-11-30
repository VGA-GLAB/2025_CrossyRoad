using DG.Tweening;
using NUnit.Framework.Internal;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// プレイヤーの移動を制御するクラス。
/// CrossyRoadのようなマス制移動を実現。
/// ・移動方向はグリッド1マス単位
/// ・移動開始時にジャンプアニメーションが入る（ただし実際には上昇のみ）
/// ・川では橋の上でなければ死亡
/// </summary>
public class PlayerMove : MonoBehaviour
{
    /// <summary>スコアが上がったときのアクション</summary>
    public Action OnScoreUpAction;

    /// <summary>プレイヤー死亡時のアクション（UI表示など）</summary>
    public Action OnPlayerDeathAction;

    /// <summary>現在移動中かどうか</summary>
    public bool IsMoving { get; private set; } = false;
    /// <summary>死亡状態かどうか</summary>
    public bool IsDead { get; private set; } = false;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _fixedY = 0.55f;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Button _retryButton;
    [SerializeField] private GameObject _blockEffectPrefab;
    [SerializeField] private float _explosionForce = 5f;
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private int _pieceCount = 12;
    [SerializeField] private Vector2 _pieceScaleRange = new Vector2(0.2f, 0.5f);
    [SerializeField] private Ease _ease;

    [Header("DoTweenジャンプアニメーション設定")]
    [SerializeField] private float animTime; // ��: 0.3f ���x
    [Header("ジャンプ設定")]
    [SerializeField] private float jumpHight; // ��: 0.3f ���x
    private bool isJumping; // �W�����v���t���O
    private bool isInputReservation; // ���͗\��t���O
    private Vector3Int inputReservation; // �\�񂳂ꂽ�ړ�����

    /// <summary>現在のグリッド座標</summary>
    private Vector3Int _currentGridPos;
    /// <summary>移動先のワールド座標</summary>
    private Vector3 _targetWorldPos;
    /// <summary>スタート時のグリッド座標</summary>
    private Vector3Int _startCell;
    /// <summary>現在乗っている橋（乗っていない場合はnull）</summary>
    private Transform _currentBridge = null;
    /// <summary>スコア判定用の最後に到達したセル</summary>
    private Vector3Int _currentCellScore;
    private Vector3 _startScale;

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
        _startScale = transform.localScale;
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
            DeadEffect();
            SquashEffect(transform, _duration);
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
                DeadEffect();
                SquashEffect(transform, _duration);
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
        transform.localScale = _startScale;
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

    [ContextMenu("爆発")]
    private void DeadEffect()
    {
        for (int i = 0; i < _pieceCount; i++)
        {
            // ブロックの破片を生成
            GameObject piece = Instantiate(_blockEffectPrefab, transform.position, Random.rotation);
            // ランダムなスケールを設定
            float s = UnityEngine.Random.Range(_pieceScaleRange.x, _pieceScaleRange.y);
            piece.transform.localScale = new Vector3(s, s, s);
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ランダムな方向に力を加える
                Vector3 dir = UnityEngine.Random.onUnitSphere;
                rb.AddForce(dir * _explosionForce, ForceMode.Impulse);
                rb.AddTorque(UnityEngine.Random.onUnitSphere * _explosionForce, ForceMode.Impulse);
            }

            Destroy(piece, 3f);
        }
    }

    [ContextMenu("縮む")]
    private async Task SquashEffect(Transform player, float duration)
    {
        Vector3 startScale = _startScale;
        Vector3 endScale = new Vector3(
            startScale.x * 1.3f,
            0.1f,
            startScale.z);
        await player.DOScale(endScale, duration)
            .SetEase(_ease)
            .AsyncWaitForCompletion();
    }
}
