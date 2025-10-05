using UnityEngine;

/// <summary>
/// グリッド全体を管理するクラス。
/// - 座標変換（ワールド座標とグリッド座標の相互変換）
/// - 占有管理（静的障害物の配置状態）
/// - プレイヤー位置の登録・参照
/// - 環境情報（セル種別の問い合わせ）
/// - 障害物の配置・削除
/// を一元的に扱う。
/// </summary>
public class GridManager : MonoBehaviour
{
    //==================================================
    // 列挙型定義
    //==================================================

    /// <summary>
    /// セルの種類を表す列挙型。
    /// 将来的に種類を増やす場合はここに追加する。
    /// </summary>
    public enum CellType
    {
        Grass,      // 草原：通常の移動可能マス
        Road,       // 道路：車が出現するレーン
        River,      // 川：丸太に乗っていなければ死亡
        Occupied,   // 静的障害物で埋まっている
        Empty       // 何もない
    }

    //==================================================
    // Unity標準イベント
    //==================================================

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初期化処理をここに記述予定
    }

    // Update is called once per frame
    void Update()
    {
        // 毎フレームの更新処理をここに記述予定
    }

    //==================================================
    // 1. 座標変換系
    //==================================================

    /// <summary>
    /// ワールド座標をグリッド座標(Vector3Int)に変換する。
    /// 利用者：プレイヤー担当（移動位置計算）、カメラ担当（追従位置計算）、動的障害物担当（移動処理）
    /// </summary>
    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        // TODO: ワールド座標をセルサイズに基づいてグリッド座標に変換する処理を実装
        return Vector3Int.zero;
    }

    /// <summary>
    /// グリッド座標(Vector3Int)をワールド座標(Vector3)に変換する。
    /// セルの中心座標を返す。
    /// </summary>
    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        // TODO: グリッド座標をワールド座標に変換する処理を実装
        return Vector3.zero;
    }

    //==================================================
    // 2. 占有管理系（静的障害物用）
    //==================================================

    /// <summary>
    /// 指定セルが空いているかどうかを返す。
    /// 静的障害物の入力キャンセル判定に使用。
    /// </summary>
    public bool IsCellFree(Vector3Int gridPos)
    {
        // TODO: 占有状態を確認して返す
        return true;
    }

    /// <summary>
    /// 指定セルを「埋まっている」と登録する。
    /// 静的障害物配置時に呼ばれる。
    /// </summary>
    public void OccupyCell(Vector3Int gridPos, GameObject obj)
    {
        // TODO: 占有情報を登録する処理を実装
    }

    /// <summary>
    /// 指定セルを解放する。
    /// 静的障害物削除時に呼ばれる。
    /// </summary>
    public void ReleaseCell(Vector3Int gridPos)
    {
        // TODO: 占有情報を解放する処理を実装
    }

    //==================================================
    // 3. プレイヤー位置系
    //==================================================

    /// <summary>
    /// プレイヤーを登録する（初期化時に呼ぶ）。
    /// </summary>
    public void RegisterPlayer(GameObject player)
    {
        // TODO: プレイヤー参照を保持する処理を実装
    }

    /// <summary>
    /// プレイヤーの現在セルを更新する。
    /// プレイヤー移動ごとに呼ばれる。
    /// </summary>
    public void UpdatePlayerCell(Vector3Int gridPos)
    {
        // TODO: プレイヤーの現在セルを更新する処理を実装
    }

    /// <summary>
    /// プレイヤーの現在セルを返す。
    /// カメラ追従やスクロール処理、スコア計算に利用。
    /// </summary>
    public Vector3Int GetPlayerCell()
    {
        // TODO: プレイヤーの現在セルを返す処理を実装
        return Vector3Int.zero;
    }

    /// <summary>
    /// 指定セルの種類を返す。
    /// Grass / Road / River / Occupied / Empty など。
    /// </summary>
    public CellType GetCellType(Vector3Int gridPos)
    {
        // TODO: セル種別を判定して返す処理を実装
        return CellType.Empty;
    }

    //==================================================
    // 4. 配置・生成系
    //==================================================

    /// <summary>
    /// 指定セルに障害物Prefabを配置し、占有情報を更新する。
    /// 利用者：静的障害物担当、ステージ生成担当
    /// </summary>
    public void PlaceObstacle(Vector3Int gridPos, ObstacleType type)
    {
        // TODO: Prefabを生成し、占有情報を更新する処理を実装
    }

    /// <summary>
    /// 指定セル内のオブジェクトを削除し、占有情報を解放する。
    /// </summary>
    public void ClearCell(Vector3Int gridPos)
    {
        // TODO: セル内のオブジェクトを削除し、占有情報を解放する処理を実装
    }
}

/// <summary>
/// 障害物の種類を表す列挙型（例）。
/// 実際のゲーム仕様に応じて拡張する。
/// </summary>
public enum ObstacleType
{
    Tree,
}
