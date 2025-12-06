using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// プレイヤーの移動を制御するクラス。
/// CrossyRoadのようなマス目単位の移動を実現する。
/// ・移動はグリッドの1マスごと
/// ・移動開始時にジャンプアニメーション（上昇のみ）を再生
/// ・川では橋の上にいなければ死亡
/// </summary>
public class PlayerMove : MonoBehaviour
{
    /// <summary>スコアが上昇したときに呼ばれるアクション</summary>
    public Action OnScoreUpAction;

    /// <summary>プレイヤーが死亡したときに呼ばれるアクション（UI表示など）</summary>
    public Action OnPlayerDeathAction;

    /// <summary>現在移動中かどうか</summary>
    public bool IsMoving { get; private set; } = false;
    /// <summary>死亡状態かどうか</summary>
    public bool IsDead { get; private set; } = false;

    [SerializeField] GameObject _deadEffect;
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
    [SerializeField] private float animTime;

    [Header("ジャンプ設定")]
    [SerializeField] private float jumpHight;
    private bool isJumping;              // ジャンプ中かどうか
    private bool isInputReservation;     // 入力予約があるか
    private Animator _animator;
    private Vector3Int inputReservation; // 予約された移動方向

    /// <summary>プレイヤーの現在グリッド座標</summary>
    private Vector3Int _currentGridPos;
    /// <summary>移動先のワールド座標</summary>
    private Vector3 _targetWorldPos;
    /// <summary>初期グリッド座標</summary>
    private Vector3Int _startCell;
    /// <summary>現在乗っている橋（乗っていない場合は null）</summary>
    private Transform _currentBridge = null;
    /// <summary>スコア判定用：最後に到達したセル</summary>
    private Vector3Int _currentCellScore;
    private Vector3 _startScale;
    private string _animBoolDie = "isDeading";
    private string _animBoolMove = "isMoving";

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
        // 初期位置をグリッド座標に変換し、少し前方にずらしてスタート
        _currentGridPos = _gridManager.WorldToGrid(transform.position);
        _currentGridPos += new Vector3Int(2, 0, 2);
        // ワールド座標へ変換して反映
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;
        _startCell = _currentGridPos;
        IsDead = false;
        _startScale = transform.localScale;
        _animator = GetComponent<Animator>();
        _deadEffect.SetActive(false);
    }

    /// <summary>
    /// Bridgeタグのコライダーに触れた際、プレイヤーを橋の子として登録する
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            // Bridgeオブジェクトの親を取得（橋の root を想定）
            Transform bridgeRoot = other.transform.parent != null ? other.transform.parent : other.transform;
            _currentBridge = bridgeRoot;
            transform.SetParent(_currentBridge);
            
            SoundManager.instance.PlaySE("橋に乗る");
        }
        // 障害物に触れた場合は死亡処理
        else if (other.CompareTag("Obstacle"))
        {
            IsDead = true;
            DeadEffect();
            SquashEffect(transform, _duration)  // 潰れるエフェクト
                .ContinueWith(t => Debug.LogError(t.Exception), TaskContinuationOptions.OnlyOnFaulted); // 非同期処理結果の例外はエラーメッセージを出力

            OnPlayerDeathAction?.Invoke();
        }
    }

    /// <summary>
    /// Bridgeタグを離れたとき、橋との関連を解除する
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bridge"))
        {
            transform.SetParent(null);
            _currentBridge = null;
        }
    }

    /// <summary>
    /// 外部からプレイヤーを死亡させたいときに呼ぶ公開メソッド。
    /// 内部の死亡処理を一元化し、外部から直接 IsDead を触らせない。
    /// </summary>
    public void Kill()
    {
        if (IsDead) return; // すでに死亡している場合は何もしない

        IsDead = true; // 内部状態を更新
        DeadEffect();  // 爆発エフェクトなどの演出
        SquashEffect(transform, _duration)  // つぶれる演出
            .ContinueWith(t => Debug.LogError(t.Exception), TaskContinuationOptions.OnlyOnFaulted); // 非同期処理結果の例外はエラーメッセージを出力
        OnPlayerDeathAction?.Invoke(); // UIやスコア処理など外部通知
    }

    /// <summary>
    /// プレイヤーの移動入力処理。
    /// Vector2 の入力をグリッド方向に変換して移動を試みる。
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
        
        //入力方向にプレイヤーを向けさせる
        var playerDir = new Vector3(input.x, 0, input.y).normalized;
        transform.rotation = Quaternion.LookRotation(playerDir);
        if (IsMoving) //移動中
        {
            if (!isInputReservation) //入力予約がされていないとき、受け付ける
            {
                isInputReservation = true;
                inputReservation = dir;
                Debug.Log("入力予約");
            }
            
            return;
        }

        // 次のセルのタイプを取得
        Vector3Int nextGridPos = _currentGridPos + dir;
        var cellType = _gridManager.GetCellType(nextGridPos);
        // 移動できないセルは無視
        if (cellType == CellType.Occupied || cellType == CellType.Empty)
        {
            return;
        }
        // 橋上にいる場合は一旦親子関係を解除
        if (_currentBridge != null)
        {
            transform.SetParent(null);
            _currentBridge = null;
        }
        // ワールド座標に変換して移動開始
        _targetWorldPos = _gridManager.GridToWorld(nextGridPos);
        _targetWorldPos.y = _fixedY;
        IsMoving = true;
        // スコア判定：前方（+Z）に進んだらスコアアップ
        if (_currentGridPos.z > _currentCellScore.z)
        {
            _currentCellScore = _currentGridPos;
            OnScoreUpAction?.Invoke();
        }

        // ジャンプ中なら入力を予約する
        if (isJumping && !isInputReservation)
        {
            isInputReservation = true;
            inputReservation = dir;
        }
    }

    private void Update()
    {
        if (IsDead) return;
        _animator?.SetBool(_animBoolMove, IsMoving);
        // 橋上で停止中はグリッド座標を橋の位置に合わせて更新
        if (_currentBridge != null && !IsMoving)
        {
            _currentGridPos = _gridManager.WorldToGrid(transform.position);
        }

        if (!IsMoving) return;

        // --- 通常の移動処理 ---
        Vector3 worldMoveDir = _targetWorldPos - transform.position;
        Vector3 step = worldMoveDir.normalized * _moveSpeed * Time.deltaTime;

        if (worldMoveDir.magnitude <= step.magnitude)
        {
            // 移動完了
            transform.position = _targetWorldPos;
            IsMoving = false;
            isJumping = false;
            // 到達したグリッド座標を更新
            _currentGridPos = _gridManager.WorldToGrid(_targetWorldPos);
            // 川に落ちたか判定
            var cellType = _gridManager.GetCellType(_currentGridPos);
            if (cellType == CellType.River && _currentBridge == null)
            {
                IsDead = true;
                PlayRiverDeath();
                OnPlayerDeathAction?.Invoke();
                return;
            }

            // 川を抜けたタイミングで橋との親子関係を解除
            if (cellType != CellType.River && _currentBridge != null)
            {
                transform.SetParent(null);
                _currentBridge = null;
            }

            // プレイヤー位置更新とステージスクロール処理
            _gridManager.UpdatePlayerCell(_currentGridPos);
            _gridManager.UpdateStageFlow();

            // 入力予約がある場合は、その方向に移動を再開
            if (isInputReservation)
            {
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

    public void ResetPosition()
    {
        _currentGridPos = _startCell;
        _targetWorldPos = _gridManager.GridToWorld(_currentGridPos);
        _targetWorldPos.y = _fixedY;
        transform.position = _targetWorldPos;
        IsMoving = false;
        IsDead = false;
        transform.SetParent(null);
        _currentBridge = null;
        _currentCellScore = _startCell;
        ScoreManager.instance.ResetScore();
        transform.localScale = _startScale;
        _animator?.SetBool(_animBoolDie, IsDead);
        _deadEffect.SetActive(false);
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// DoTween を使用したジャンプアニメーションの開始
    /// </summary>
    private void Jumping()
    {
        if (!isJumping)
        {
            isJumping = true;
            transform.DOMoveY(transform.position.y + jumpHight, animTime)
                .SetEase(Ease.OutQuint);
            
            //SEを再生する
            SoundManager.instance.PlaySE("移動");
        }
    }

    [ContextMenu("爆発")]
    private void DeadEffect()
    {
        for (int i = 0; i < _pieceCount; i++)
        {
            // 破片を生成
            GameObject piece = Instantiate(_blockEffectPrefab, transform.position, Random.rotation);

            // ランダムな大きさ
            float s = UnityEngine.Random.Range(_pieceScaleRange.x, _pieceScaleRange.y);
            piece.transform.localScale = new Vector3(s, s, s);
            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ランダム方向に力を加える
                Vector3 dir = UnityEngine.Random.onUnitSphere;
                rb.AddForce(dir * _explosionForce, ForceMode.Impulse);
                rb.AddTorque(UnityEngine.Random.onUnitSphere * _explosionForce, ForceMode.Impulse);
            }

            Destroy(piece, 3f);
        }
    }

    private async Task SquashEffect(Transform player, float duration)
    {
        Vector3 startScale = _startScale;
        Vector3 endScale = new Vector3(startScale.x * 1.3f, 0.1f, startScale.z);

        await player.DOScale(endScale, duration)
            .SetEase(_ease)
            .AsyncWaitForCompletion();
    }

    private void PlayRiverDeath()
    {
        if (_animator == null) return;
        this.gameObject.SetActive(false);
        _deadEffect.transform.position = this.transform.position;
        _deadEffect.SetActive(true);
        _animator.SetBool(_animBoolDie, IsDead);
    }
}
