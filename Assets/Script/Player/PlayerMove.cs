using System;
using System.Collections;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    /// <summary>
    /// スコアアップのアクション
    /// </summary>
    public Action OnScoreUpAction;
    /// <summary>
    /// 死亡時のアクション
    /// </summary>
    public Action OnPlayerDeathAction;
    public bool _isMoving { get; private set; } = false;
    [SerializeField] private float _gridSpace = 1.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GroundCheck _groundCheck;
    /// <summary>
    /// 現座のグリッド座標
    /// </summary>
    private Vector3Int _currentCell;
    /// <summary>
    /// スタート時のグリッド座標
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
    /// プレイヤーを移動させる
    /// </summary>
    /// <param name="input"></param>
    public void Move(Vector2 input)
    {
        if (_isMoving) return;
        //入力ベクトルをグリッド移動に変換
        Vector3Int moveDirection = new Vector3Int(
            Mathf.RoundToInt(input.x),
            0,
            Mathf.RoundToInt(input.y)
        );
        //次の移動先セルを計算
        Vector3Int nextCell = _currentCell + moveDirection;
        //TODOnextCellを今のx,yから次のセルにする
        //移動先セルが空いているか確認
        if (!_gridManager.IsCellFree(nextCell)) return;
        StartCoroutine(MovePlayer(nextCell));
    }

    /// <summary>
    /// プレイヤーを次のセルに移動させるコルーチン
    /// </summary>
    private IEnumerator MovePlayer(Vector3Int targetCell)
    {
        _isMoving = true;
        // 現在位置と目的地（ワールド座標）を取得
        Vector3 start = transform.position;
        //目的地セルを変換
        Vector3 end = _gridManager.GridToWorld(targetCell);
        float t = 0f;
        while (t < _gridSpace)
        {
            transform.position = Vector3.Lerp(start, end, t / _gridSpace);
            t += Time.deltaTime * _moveSpeed;
            yield return null;
        }
        // 最終的に目的地に正確に設定
        transform.position = end;
        // 現在のセル情報を更新
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
