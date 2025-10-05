using System.Collections;
using UnityEngine;
public class PlayerMove : MonoBehaviour
{
    public bool _isMoving { get; private set; } = false;
    [SerializeField] private float _gridSpace = 1.0f;
    [SerializeField] private float _moveSpeed = 5.0f;
    private GridManager _gridManager;
    /// <summary>
    /// 現座のグリッド座標
    /// </summary>
    private Vector3Int _currentCell;

    private void Start()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
        _gridManager.RegisterPlayer(gameObject);
        _currentCell = _gridManager.WorldToGrid(transform.position);
    }

    /// <summary>
    /// プレイヤーを移動させる
    /// </summary>
    /// <param name="input"></param>
    public void Move(Vector2 input)
    {
        if(_isMoving) return;
        //入力ベクトルをグリッド移動に変換
        Vector3Int moveDirection = new Vector3Int(
            Mathf.RoundToInt(input.x),
            0,
            Mathf.RoundToInt(input.y)
        );
        //次の移動先セルを計算
        Vector3Int next =_currentCell + moveDirection;
        //移動先セルが空いているか確認
        if (!_gridManager.IsCellFree(next)) return;
        StartCoroutine(MovePlayer(moveDirection));
    }

    /// <summary>
    /// プレイヤーを次のセルに移動させるコルーチン
    /// </summary>
    /// <param name="next"></param>
    /// <returns></returns>
    private IEnumerator MovePlayer(Vector3Int next)
    {
        _isMoving = true;
        // 現在位置と目的地（ワールド座標）を取得
        Vector3 start = transform.position;
        Vector3 end = _gridManager.GridToWorld(next);
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
        _currentCell = next;
        _gridManager.UpdatePlayerCell(next);
        _isMoving = false;
    }
}
