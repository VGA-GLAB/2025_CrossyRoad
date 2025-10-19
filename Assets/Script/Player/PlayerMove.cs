using System;
using UnityEngine;

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
    /// 死亡時のアクション（川に落下した場合などに呼ばれる）
    /// </summary>
    public Action DeathAction;

    /// <summary>
    /// 現在移動中かどうか
    /// </summary>
    public bool IsMoving { get; private set; } = false;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float fixedY = 0.55f;
    [SerializeField] private GridManager gridManager;

    /// <summary>
    /// 現在のグリッド座標
    /// </summary>
    private Vector3Int currentGridPos;
    /// <summary>
    /// 移動先のワールド座標
    /// </summary>
    private Vector3 targetWorldPos;
    /// <summary>
    /// 現在乗っている橋（なければnull）
    /// </summary>
    private Transform currentBridge = null;

    private void Start()
    {

        gridManager = FindAnyObjectByType<GridManager>();
        gridManager.RegisterPlayer(gameObject);

        // 外枠を考慮して1マス進める
        currentGridPos = gridManager.WorldToGrid(transform.position);
        currentGridPos += new Vector3Int(1, 0, 2);

        // 初期位置をグリッドに揃える
        //currentGridPos = gridManager.WorldToGrid(transform.position);
        targetWorldPos = gridManager.GridToWorld(currentGridPos);
        targetWorldPos.y = fixedY;
        transform.position = targetWorldPos;
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

            currentBridge = bridgeRoot;
            transform.SetParent(currentBridge);
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
            currentBridge = null;
        }
    }

    /// <summary>
    /// プレイヤーを移動させる
    /// 入力ベクトルをグリッド移動に変換し、移動先セルを決定する
    /// </summary>
    public void TryMove(Vector2 input)
    {
        if (IsMoving) return;

        // 入力ベクトルをグリッド方向に変換
        Vector3Int dir = Vector3Int.zero;
        if (input.y > 0) dir = Vector3Int.forward;
        else if (input.y < 0) dir = Vector3Int.back;
        else if (input.x < 0) dir = Vector3Int.left;
        else if (input.x > 0) dir = Vector3Int.right;

        if (dir == Vector3Int.zero) return;

        // ★ 本家準拠の挙動：
        // 移動開始時に橋から切り離すことで「慣性なし」にする
        if (currentBridge != null)
        {
            transform.SetParent(null);
            currentBridge = null;
        }

        // 次の移動先セルを計算
        Vector3Int nextGridPos = currentGridPos + dir;
        var cellType = gridManager.GetCellType(nextGridPos);

        // 移動先セルをワールド座標に変換
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
       
        // --- 通常の自前移動 ---
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
                Debug.Log("落下！");
            }

            // 川セル以外に移動したら親子付け解除
            if (cellType != CellType.River && currentBridge != null)
            {
                //Vector3 worldPos = transform.position;
                transform.SetParent(null);
                //transform.position = worldPos;
                currentBridge = null;
                Debug.Log("橋から降りる（セル判定）");
            }
        }
        else
        {
            transform.position += step;
        }
    }
}
