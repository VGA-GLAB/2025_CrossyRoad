using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// プレイヤーの移動処理を担当するクラス
/// 本家CrossyRoad準拠の挙動を再現するため、
/// ・移動はグリッド単位
/// ・移動開始時に橋から切り離す（慣性なし）
/// ・橋の上では親子付けして追従
/// を実装している
/// </summary>
public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// スコア加算時のアクション
    /// </summary>
    public Action OnScoreUpAction;

    /// <summary>
    /// 死亡時のアクション（川に落下した場合などに呼ばれる）
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

    [Header("DoTweenのアニメーション時間")]
    [SerializeField] private float animTime; //0.3fがちょうどいい
    [Header("飛ぶ高さ")]
    [SerializeField] private float jumpHight; //0.3fがちょうどいい
    private bool isJumping; //飛んでいるアニメーション中
    
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
    /// 現在乗っている橋（なければnull）
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

        // 外枠を考慮して1マス進める
        _currentGridPos = _gridManager.WorldToGrid(transform.position);
        _currentGridPos += new Vector3Int(1, 0, 2);

        // 初期位置をグリッドに揃える
        //currentGridPos = gridManager.WorldToGrid(transform.position);
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;
        _startCell = _currentGridPos;
        IsDead = false;
    }

    /// <summary>
    /// 橋のTriggerに入ったときに親子付けする
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("橋に乗る");

            // Judgment に当たった場合でも、1階層上の Bridge 本体を親にする
            Transform bridgeRoot = other.transform.parent != null ? other.transform.parent : other.transform;

            _currentBridge = bridgeRoot;
            transform.SetParent(_currentBridge);
        }
    }

    /// <summary>
    /// 橋のTriggerから出たときに親子関係を解除する
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            Debug.Log("橋から降りる");

            // ワールド座標を保持してから親子関係を解除
            transform.SetParent(null);
            _currentBridge = null;
        }
    }

    /// <summary>
    /// プレイヤーを移動させる
    /// 入力ベクトルをグリッド移動に変換し、移動先セルを決定する
    /// </summary>
    public void TryMove(Vector2 input)
    {
        if (IsMoving||IsDead) return;

        // 入力ベクトルをグリッド方向に変換
        Vector3Int dir = Vector3Int.zero;
        if (input.y > 0) dir = Vector3Int.forward;
        else if (input.y < 0) dir = Vector3Int.back;
        else if (input.x < 0) dir = Vector3Int.left;
        else if (input.x > 0) dir = Vector3Int.right;

        if (dir == Vector3Int.zero) return;

        // 本家準拠の挙動：
        // 移動開始時に橋から切り離すことで「慣性なし」にする
        if (_currentBridge != null)
        {
            transform.SetParent(null);
            _currentBridge = null;
        }

        // 次の移動先セルを計算
        Vector3Int nextGridPos = _currentGridPos + dir;
        var cellType = _gridManager.GetCellType(nextGridPos);

        // 移動先セルをワールド座標に変換
        _targetWorldPos = _gridManager.GridToWorld(nextGridPos);
        _targetWorldPos.y = _fixedY;

        _currentGridPos = nextGridPos;
        IsMoving = true;

        if (_currentGridPos.z >_currentCellScore.z)
        {
            _currentCellScore = _currentGridPos;
            OnScoreUpAction?.Invoke();
            Debug.Log("スコアアップ！");
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
       
        // --- 通常の自前移動 ---
        Vector3 worldMoveDir = _targetWorldPos - transform.position;
        Vector3 step = worldMoveDir.normalized * _moveSpeed * Time.deltaTime;

        if (worldMoveDir.magnitude <= step.magnitude)
        {
            transform.position = _targetWorldPos;
            IsMoving = false;

            isJumping = false; //飛ぶアニメーションを終了

            var cellType = _gridManager.GetCellType(_currentGridPos);
            if (cellType == CellType.River && _currentBridge == null)
            {
                IsDead = true;
                OnPlayerDeathAction?.Invoke();
                //_currentGridPos = _startCell;
                Debug.Log("落下！");
                return;
            }

            // 川セル以外に移動したら親子付け解除
            if (cellType != CellType.River && _currentBridge != null)
            {
                //Vector3 worldPos = transform.position;
                transform.SetParent(null);
                //transform.position = worldPos;
                _currentBridge = null;
                Debug.Log("橋から降りる（セル判定）");
            }

            // 描画範囲の更新
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
    /// DoTweenによる飛ぶアニメーション
    /// </summary>
    private void Jumping()
    {
        if (!isJumping) //飛んでいないとき
        {
            isJumping = true;

            //飛ぶアニメーション
            transform.DOMoveY(transform.position.y + jumpHight, animTime).SetEase(Ease.OutQuint);
        }
    }
}
