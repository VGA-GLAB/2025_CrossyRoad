using System.Collections.Generic;
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
    // インスペクター設定項目
    //==================================================
    // 1セルのワールドサイズ。
    [SerializeField] private float cellSize = 1.0f;

    // 幅（左右方向）の描画範囲
    [SerializeField] private int renderWidth = 10;
    // 奥行き（前後方向）の描画範囲
    [SerializeField] private int renderDepthForward = 20;
    [SerializeField] private int renderDepthBackward = 5;
 
    // 環境情報の各種Prefab
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject riverPrefab;
    [SerializeField] private GameObject treePrefab;

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

    /// <summary>
    /// 解放時、内部データと描画オブジェクトをすべて解放する。
    /// </summary>
    private void OnDestroy()
    {
        ClearAll();
    }

    //==================================================
    // 制御メソッド
    //==================================================
    /// <summary>
    /// 内部データと描画オブジェクトをすべて解放する。
    /// ステージ終了時やシーン破棄時に呼ぶ。
    /// </summary>
    public void ClearAll()
    {
        // 論理データのクリア
        terrainCells.Clear();
        staticObstacleCells.Clear();

        // 地形プレハブの破棄
        foreach (var obj in terrainPrefabs.Values)
        {
            if (obj != null) Destroy(obj);
        }
        terrainPrefabs.Clear();

        // 障害物プレハブの破棄
        foreach (var obj in staticObstaclePrefabs.Values)
        {
            if (obj != null) Destroy(obj);
        }
        staticObstaclePrefabs.Clear();

        playerCell = Vector3Int.zero;
        player = null;
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
        // ワールド座標をセルサイズに基づいてグリッド座標に変換し返却
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        int z = Mathf.FloorToInt(worldPos.z / cellSize);

        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// グリッド座標(Vector3Int)をワールド座標(Vector3)に変換する。
    /// セルの中心座標を返す。
    /// </summary>
    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        // グリッド座標をワールド座標に変換し返却
        float x = gridPos.x * cellSize;
        float y = gridPos.y * cellSize;
        float z = gridPos.z * cellSize;

        return new Vector3(x, y, z);
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
        // 地形が存在しないセルは不可
        if (!terrainCells.ContainsKey(gridPos)) return false; 

        // 障害物レイヤーを確認して返す
        // Note: 静的障害物の配置は、Y軸についてキューブの下面がゼロとなるよう補正していため
        // ここではY=1で検索する。あまり綺麗な実装ではないのでY軸の扱いを整理したうえで要改善。
        var obstaclePos = new Vector3Int(gridPos.x, 1, gridPos.z);
        return !staticObstacleCells.ContainsKey(obstaclePos);

    }

    /// <summary>
    /// 指定セルを「埋まっている」と登録する。
    /// 静的障害物配置時に呼ばれる。
    /// </summary>
    public void OccupyCell(Vector3Int gridPos, ObstacleType type)
    {
        staticObstacleCells[gridPos] = type;
    }

    /// <summary>
    /// 指定セルを解放する。
    /// 静的障害物削除時に呼ばれる。
    /// </summary>
    public void ReleaseCell(Vector3Int gridPos)
    {
        staticObstacleCells.Remove(gridPos);
    }

    //==================================================
    // 3. プレイヤー位置系
    //==================================================

    /// <summary>
    /// プレイヤーを登録する（初期化時に呼ぶ）。
    /// </summary>
    public void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }

    /// <summary>
    /// プレイヤーの現在セルを更新する。
    /// プレイヤー移動ごとに呼ばれる。
    /// </summary>
    public void UpdatePlayerCell(Vector3Int gridPos)
    {
        playerCell = gridPos;
    }

    /// <summary>
    /// プレイヤーの現在セルを返す。
    /// カメラ追従やスクロール処理、スコア計算に利用。
    /// </summary>
    public Vector3Int GetPlayerCell()
    {
        return playerCell;
    }

    /// <summary>
    /// セルの最終的な状態を返す。
    /// - 障害物があれば Occupied
    /// - なければ地形レイヤーの種類
    /// - 未登録なら Empty
    /// </summary>
    public CellType GetCellType(Vector3Int gridPos)
    {
        if (staticObstacleCells.ContainsKey(gridPos))
            return CellType.Occupied;

        if (terrainCells.TryGetValue(gridPos, out var type))
            return type;

        return CellType.Empty;
    }

    /// <summary>
    /// セルの地形レイヤーのみを返す。
    /// 障害物は無視して純粋に地形タイプを返す。
    /// </summary>
    public CellType GetTerrainCellType(Vector3Int gridPos)
    {
        if (terrainCells.TryGetValue(gridPos, out var type))
            return type;

        return CellType.Empty;
    }

    //==================================================
    // 4. 配置・生成系
    //==================================================
    /// <summary>
    /// 指定セルに地形Prefabを配置する。
    /// すでに描画済みなら何もしない。
    /// </summary>
    private void CreateTerrainPrefab(Vector3Int gridPos, CellType type)
    {
        if (terrainPrefabs.ContainsKey(gridPos))
            return;

        GameObject prefab = null;
        switch (type)
        {
            case CellType.Grass: prefab = grassPrefab; break;
            case CellType.Road: prefab = roadPrefab; break;
            case CellType.River: prefab = riverPrefab; break;
            case CellType.Empty: prefab = null; break;
        }

        if (prefab != null)
        {
            Vector3 worldPos = GridToWorld(gridPos);
            worldPos.y = -0.5f * cellSize;      // キューブの上面がゼロ座標となるようY座標を調整
            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);
            terrainPrefabs[gridPos] = obj;
        }
    }

    /// <summary>
    /// 指定セルの地形Prefabを削除する。
    /// </summary>
    private void DestroyTerrainPrefab(Vector3Int gridPos)
    {
        if (terrainPrefabs.TryGetValue(gridPos, out var obj))
        {
            Destroy(obj);
            terrainPrefabs.Remove(gridPos);
        }
    }

    /// <summary>
    /// 指定セルに障害物Prefabを配置する。
    /// すでに障害物がある場合は何もしない。
    /// </summary>
    private void CreateObstaclePrefab(Vector3Int gridPos, ObstacleType type)
    {
        if (staticObstaclePrefabs.ContainsKey(gridPos))
        {
            return; // すでに生成済みなら何もしない
        }

        GameObject prefab = null;
        switch (type)
        {
            case ObstacleType.Tree: prefab = treePrefab; break;
        }

        if (prefab != null)
        {
            Vector3 worldPos = GridToWorld(gridPos);
            worldPos.y = 0.5f * cellSize;      // キューブの下面がゼロ座標となるようY座標を調整

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);
            staticObstaclePrefabs[gridPos] = obj;
        }
    }

    /// <summary>
    /// 指定セル内のオブジェクトを削除する。
    /// </summary>
    private void DestroyObstaclePrefab(Vector3Int gridPos)
    {
        if (staticObstaclePrefabs.TryGetValue(gridPos, out var obj))
        {
            Destroy(obj);
            staticObstaclePrefabs.Remove(gridPos);
        }
    }

    /// <summary>
    /// 指定セルに障害物Prefabを配置し、占有情報を更新する。
    /// すでに障害物がある場合は何もしない。
    /// 利用者：静的障害物担当、ステージ生成担当
    /// </summary>
    public void PlaceObstacleCell(Vector3Int gridPos, ObstacleType type)
    {
        // セル内にオブジェクトを生成し、占有情報を登録する
        OccupyCell(gridPos, type);
        CreateObstaclePrefab(gridPos, type);
    }

    /// <summary>
    /// 指定セル内のオブジェクトを削除し、占有情報を解放する。
    /// 地形レイヤーは変更しない。
    /// </summary>
    public void ClearObstacleCell(Vector3Int gridPos)
    {
        // セル内のオブジェクトを削除し、占有情報を解放する
        ReleaseCell(gridPos);
        DestroyObstaclePrefab(gridPos);
    }

    /// <summary>
    /// StageData を受け取り、マップ全体のレイヤーを構築する。
    /// </summary>
    public void BuildStage(StageData data)
    {
        terrainCells.Clear();
        staticObstacleCells.Clear();

        // 地形レイヤーを反映
        foreach (var lane in data.laneTypes)
        {
            int z = lane.Key;
            CellType type = lane.Value;

            for (int x = 0; x < data.width; x++)
            {
                Vector3Int gridPos = new Vector3Int(x, 0, z);
                terrainCells[gridPos] = type;
            }
        }

        // 静的障害物レイヤーを反映
        foreach (var obstacle in data.staticObstacles)
        {
            Vector3Int gridPos = obstacle.Key;
            ObstacleType type = obstacle.Value;
            staticObstacleCells[gridPos] = type;
        }
    }

    /// <summary>
    /// プレイヤー位置を基準に描画範囲を更新する
    /// </summary>
    public void UpdateRenderArea()
    {
        var center = playerCell;
        var newVisible = new HashSet<Vector3Int>();

        // 幅方向: -renderWidth ～ +renderWidth
        // 奥行き方向: -renderDepthBackward ～ +renderDepthForward
        for (int dx = -renderWidth; dx <= renderWidth; dx++)
        {
            for (int dz = -renderDepthBackward; dz <= renderDepthForward; dz++)
            {
                var pos = new Vector3Int(center.x + dx, 0, center.z + dz);
                newVisible.Add(pos);

                // 地形が存在すれば描画
                if (terrainCells.TryGetValue(pos, out var terrainType))
                    CreateTerrainPrefab(pos, terrainType);

                // 障害物が存在すれば描画
                // Note: 静的障害物の配置は、Y軸についてキューブの下面がゼロとなるよう補正していため
                // ここではY=1で検索する。あまり綺麗な実装ではないのでY軸の扱いを整理したうえで要改善。
                pos.y = 1;
                if (staticObstacleCells.TryGetValue(pos, out var obstacleType))
                {
                    CreateObstaclePrefab(pos, obstacleType);
                    newVisible.Add(pos);
                }
            }
        }

        // 範囲外になったセルを削除
        foreach (var kv in new List<Vector3Int>(terrainPrefabs.Keys))
        {
            if (!newVisible.Contains(kv))
                DestroyTerrainPrefab(kv);
        }
        foreach (var kv in new List<Vector3Int>(staticObstaclePrefabs.Keys))
        {
            // Note: 静的障害物の配置は、Y軸についてキューブの下面がゼロとなるよう補正していため
            // ここでもY=1で検索する。要改善。
            var comparePos = new Vector3Int(kv.x, 1, kv.z);
            
            if (!newVisible.Contains(comparePos))
                DestroyObstaclePrefab(kv);
        }
    }

    //==================================================
    // 内部状態
    //==================================================
    // プレイヤーの現在セル位置
    private Vector3Int playerCell = Vector3Int.zero;

    // プレイヤーの参照
    private GameObject player;

    //-------------------------------------------------- 
    // 環境情報レイヤー
    //-------------------------------------------------- 

    //
    // マップ全体
    //

    // 地形レイヤー（セル単位）
    private Dictionary<Vector3Int, CellType> terrainCells = new Dictionary<Vector3Int, CellType>();
    
    // 静的障害物レイヤー（セル単位）
    private Dictionary<Vector3Int, ObstacleType> staticObstacleCells = new Dictionary<Vector3Int, ObstacleType>();

    //
    // 描画領域
    //

    // 地形レイヤー（プレハブ）
    private Dictionary<Vector3Int, GameObject> terrainPrefabs = new Dictionary<Vector3Int, GameObject>();
    
    // 静的障害物レイヤー（プレハブ）
    private Dictionary<Vector3Int, GameObject> staticObstaclePrefabs = new Dictionary<Vector3Int, GameObject>();
}

