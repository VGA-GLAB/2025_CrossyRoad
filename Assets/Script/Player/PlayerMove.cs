using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// プレイヤーの移動を制御するクラス。
/// CrossyRoadのようなマス単位の移動を想定。
/// ・移動はグリッド単位
/// ・移動開始時にジャンプアニメーションを再生（なめらかな動き）
/// ・川では橋の上にいないと死亡
/// </summary>
public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// スコア加算のアクション
    /// </summary>
    public Action OnScoreUpAction;

    /// <summary>
    /// プレイヤー死亡時のアクション（川に落ちたなど）
    /// </summary>
    public Action OnPlayerDeathAction;

    /// <summary>
    /// 現在移動中かどうか
    /// </summary>
    public bool IsMoving { get; private set; } = false;
    public bool IsDead { get; private set; } = false;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _fixedY = 0.55f;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private Button _retryButton;

    [Header("DoTweenのアニメーション設定")]
    [SerializeField] private float animTime; // 例: 0.3f 程度
    [Header("ジャンプ設定")]
    [SerializeField] private float jumpHight; // 例: 0.3f 程度
    private bool isJumping; // ジャンプ中フラグ
    private bool isInputReservation; // 入力予約フラグ
    private Vector3Int inputReservation; // 予約された移動方向

    /// <summary>
    /// 現在のグリッド座標
    /// </summary>
    private Vector3Int _currentGridPos;
    /// <summary>
    /// 移動先のワールド座標
    /// </summary>
    private Vector3 _targetWorldPos;
    /// <summary>
    /// スタート時のグリッド座標
    /// </summary> 
    private Vector3Int _startCell;
    /// <summary>
    /// 現在乗っている橋（乗っていない場合はnull）
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

        // 初期位置を1マス前に進めてスタート
        _currentGridPos = _gridManager.WorldToGrid(transform.position);
        _currentGridPos += new Vector3Int(1, 0, 2);

        // ワールド座標に変換して配置
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;

        _startCell = _currentGridPos;
        IsDead = false;
    }

    /// <summary>
    /// 橋(Bridge)のTriggerに入ったときに橋の子に設定する
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("橋に乗った");

            // Bridgeオブジェクトの親を取得（root bridge）
            Transform bridgeRoot = other.transform.parent != null ? other.transform.parent : other.transform;

            _currentBridge = bridgeRoot;
            transform.SetParent(_currentBridge);
        }
    }

    /// <summary>
    /// 橋(Bridge)のTriggerから出たときに親子関係を解除
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("橋から離れた");

            transform.SetParent(null);
            _currentBridge = null;
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// 入力ベクトルをグリッド方向に変換して、移動先を設定する
    /// </summary>
    public void TryMove(Vector2 input)
    {
        if (IsDead) return;

        // 入力方向をベクトルに変換
        Vector3Int dir = Vector3Int.zero;
        if (input.y > 0) dir = Vector3Int.forward;
        else if (input.y < 0) dir = Vector3Int.back;
        else if (input.x < 0) dir = Vector3Int.left;
        else if (input.x > 0) dir = Vector3Int.right;

        if (dir == Vector3Int.zero) return;

        // 次のマス座標を計算
        Vector3Int nextGridPos = _currentGridPos + dir;
        var cellType = _gridManager.GetCellType(nextGridPos);

        // セルが木または空なら入力キャンセル
        if (cellType == CellType.Occupied || cellType == CellType.Empty)
        {
            return;
        }

        // 橋の上にいる場合は一旦解除
        if (_currentBridge != null)
        {
            transform.SetParent(null);
            _currentBridge = null;
        }

        // ワールド座標に変換
        _targetWorldPos = _gridManager.GridToWorld(nextGridPos);
        _targetWorldPos.y = _fixedY;

        _currentGridPos = nextGridPos;
        IsMoving = true;

        // スコアアップ判定
        if (_currentGridPos.z > _currentCellScore.z)
        {
            _currentCellScore = _currentGridPos;
            OnScoreUpAction?.Invoke();
            Debug.Log("スコアアップ！");
        }

        // 入力予約
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

        // --- 通常の移動処理 ---
        Vector3 worldMoveDir = _targetWorldPos - transform.position;
        Vector3 step = worldMoveDir.normalized * _moveSpeed * Time.deltaTime;

        if (isInputReservation) // 入力予約処理中
        {
            // 前のマスを取得して状態を確認
            var cellType = _gridManager.GetCellType(_currentGridPos - inputReservation);
            // Raycastで下にBridgeがあるかチェック
            if (Physics.Raycast(transform.position + Vector3.down * 0.5f, Vector3.down, out RaycastHit hit, 2f))
            {
                if (hit.collider.CompareTag("Bridge"))
                {
                    if (cellType != CellType.River && _currentBridge != null)
                    {
                        isInputReservation = false;
                        transform.SetParent(null);
                        _currentBridge = null;
                        Debug.Log("橋から離れた（陸地に移動）");
                    }
                }
                else // 下に橋がない場合
                {
                    if (cellType == CellType.River && _currentBridge == null)
                    {
                        isInputReservation = false;
                        IsDead = true;
                        OnPlayerDeathAction?.Invoke();
                        return;
                    }
                }
            }
        }

        if (worldMoveDir.magnitude <= step.magnitude)
        {
            // 到達
            transform.position = _targetWorldPos;
            IsMoving = false;

            isJumping = false; // ジャンプ終了
            isInputReservation = false;

            var cellType = _gridManager.GetCellType(_currentGridPos);
            if (cellType == CellType.River && _currentBridge == null)
            {
                IsDead = true;
                OnPlayerDeathAction?.Invoke();
                Debug.Log("川に落下 → 死亡");
                return;
            }

            // 陸地に移動したら橋から離す
            if (cellType != CellType.River && _currentBridge != null)
            {
                transform.SetParent(null);
                _currentBridge = null;
                Debug.Log("橋から離れた（陸地に到達）");
            }

            // プレイヤー位置、表示範囲を更新
            _gridManager.UpdatePlayerCell(_currentGridPos);
            _gridManager.UpdateStageFlow();
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
    /// DoTweenによるジャンプアニメーション
    /// </summary>
    private void Jumping()
    {
        if (!isJumping) // すでにジャンプ中でなければ
        {
            isJumping = true;
            // ジャンプ演出
            transform.DOMoveY(transform.position.y + jumpHight, animTime).SetEase(Ease.OutQuint);
        }
    }
}
