using UnityEngine;

/// <summary>
/// プレイヤーの位置変化に応じてステージの描画範囲を更新するクラス。
/// 
/// GridManager が保持する「現在のプレイヤーセル」を監視し、
/// 位置が変化した場合のみ UpdateRenderArea() を呼び出す。
/// 
/// このスクリプトを空の GameObject にアタッチしておくことで、
/// PlayerMove の移動処理に手を加えず描画更新を自動化できる。
/// </summary>
public class StageRendererUpdater : MonoBehaviour
{
    private GridManager _gridManager;
    private PlayerMove _player;
    // 前フレームでのプレイヤーのセル座標
    private Vector3Int _lastPlayerCell;

    private void Start()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
        _player = FindAnyObjectByType<PlayerMove>();
        if (_gridManager == null || _player == null)
        {
            Debug.LogError("[StageRendererUpdater] GridManager または PlayerMove が見つかりません。");
            enabled = false;
            return;
        }
        // 初期セル位置を記録しておく
        _lastPlayerCell = _gridManager.GetPlayerCell();
    }

    private void Update()
    {
        // 安全確認：参照が途中で破棄されていないかチェック
        if (_player == null || _gridManager == null) return;
        // 現在のプレイヤーのセル位置を取得
        Vector3Int currentCell = _gridManager.GetPlayerCell();
        // プレイヤーが別のセルに移動していた場合のみ描画更新を行う
        if (currentCell != _lastPlayerCell)
        {
            // 新しい描画範囲を更新
            _gridManager.UpdateRenderArea();
            // 前回位置を更新
            _lastPlayerCell = currentCell;
        }
    }
}
