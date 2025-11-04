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
        // StageGenerator をアタッチしている前提
        stageGenerator = GetComponent<StageGenerator>();
        stageGenerator.Initialize(gridWidth, gridChunkSize);

        // 最初のチャンクを生成
        GenerateChunkAt(0);
        lastGeneratedZ += gridChunkSize;

        // プレイヤー初期セルを登録
        Vector3Int startCell = new Vector3Int(1, 0, 1);
        UpdatePlayerCell(startCell);
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

        // スポナープレハブの破棄
        foreach (var entry in spawnerEntries)
        {
            if (entry.Instance != null)
            {
                Destroy(entry.Instance);
                entry.Instance = null;
            }
        }
        spawnerEntries.Clear();

        // プレイヤー情報のクリア
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
        int x = Mathf.FloorToInt((worldPos.x - cellSize / 2f) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - cellSize / 2f) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - cellSize / 2f) / cellSize);

        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// グリッド座標(Vector3Int)をワールド座標(Vector3)に変換する。
    /// セルの中心座標を返す。
    /// </summary>
    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        // グリッド座標をワールド座標に変換し返却
        float x = gridPos.x * cellSize + (cellSize / 2f);
        float y = gridPos.y * cellSize + (cellSize / 2f);
        float z = gridPos.z * cellSize + (cellSize / 2f);

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
    /// スポナー生成
    /// </summary>
    private void CreateSpawnerInstance(SpawnerEntry entry)
    {
        if (entry.Instance != null)
        {
            return; // 既に生成済みなら何もしない
        }

        Vector3 worldPos = GridToWorld(entry.GridPos);
        entry.Instance = Instantiate(entry.Prefab, worldPos, Quaternion.identity, this.transform);

        // ToDo:Spawnerの種類が増えたら追加する
        // 共通インターフェース ISpawner を導入する手もあるが柔軟性を失うのでこのスタイルでいいかも

        // 橋のスポナー
        if (entry.Config is BridgeSpawnerConfig bridgeConfig)
        {
            var bridgeSpawn = entry.Instance.GetComponent<BridgeSpawn>();
            if (bridgeSpawn != null)
            {
                bridgeSpawn.Initialize(bridgeConfig);
            }
        }

        // 動的障害物のスポナー
        if (entry.Config is DynamicObstaclesSpawnerConfig dynamicConfig)
        {
            var dynamicSpawner = entry.Instance.GetComponent<DynamicObstaclesSpawner>();
            if (dynamicSpawner != null)
            {
                dynamicSpawner.Initialize(dynamicConfig);
            }
        }

    }

    /// <summary>
    /// スポナー破棄
    /// </summary>
    private void DestroySpawnerInstance(SpawnerEntry entry)
    {
        if (entry.Instance != null)
        {
            Destroy(entry.Instance);
            entry.Instance = null;
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

        // スポナーレイヤーを反映
        foreach (var config in data.spawnerConfigs)
        {
            spawnerEntries.Add(new SpawnerEntry(config));
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

                // スポナーがこのセルにある場合は生成
                foreach (var entry in spawnerEntries)
                {
                    // Note: 補正の件で、Y=-1で検索。煩雑化してるので後ほど仕様整理して修正する。
                    // スポナーは左右端に常に存在する必要があるのでX軸は判定から外す
                    pos.y = -1;
                    if (entry.GridPos.z == pos.z && entry.GridPos.y == pos.y)
                    {
                        CreateSpawnerInstance(entry);
                        newVisible.Add(entry.GridPos);
                    }
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

        // 範囲外になったセルを削除（スポナー）
        for (int i = 0; i < spawnerEntries.Count; i++)
        {
            var entry = spawnerEntries[i];
            // 可視範囲チェックは Y=0 基準
            if (!newVisible.Contains(entry.GridPos))
            {
                if (entry.Instance != null)
                {
                    Destroy(entry.Instance);
                    entry.Instance = null;
                }
            }
        }
    }

    /// <summary>
    /// 指定した worldZ のチャンクを生成し、内部データに統合してシーンに反映する
    /// </summary>
    private void GenerateChunkAt(int worldZ)
    {
        // StageData を生成
        StageData data = stageGenerator.GenerateChunk(worldZ);

        // 内部データに統合
        foreach (var lane in data.laneTypes)
        {
            for (int x = 0; x < data.width; x++)
            {
                Vector3Int gridPos = new Vector3Int(x, 0, lane.Key);
                terrainCells[gridPos] = lane.Value;
            }
        }

        foreach (var obstacle in data.staticObstacles)
        {
            staticObstacleCells[obstacle.Key] = obstacle.Value;
        }

        foreach (var config in data.spawnerConfigs)
        {
            spawnerEntries.Add(new SpawnerEntry(config));
        }

        // シーンに反映
        BuildChunkVisuals(data, worldZ);
    }

    /// <summary>
    /// StageData をもとにシーンにプレハブを配置する
    /// </summary>
    private void BuildChunkVisuals(StageData data, int worldZ)
    {
        // 地形
        foreach (var lane in data.laneTypes)
        {
            int z = lane.Key;
            CellType type = lane.Value;
            for (int x = 0; x < data.width; x++)
            {
                Vector3Int gridPos = new Vector3Int(x, 0, z);
                CreateTerrainPrefab(gridPos, type);
            }
        }

        // 静的障害物
        foreach (var obstacle in data.staticObstacles)
        {
            Vector3Int gridPos = obstacle.Key;
            CreateObstaclePrefab(gridPos, obstacle.Value);
        }

        // スポナー
        foreach (var entry in spawnerEntries)
        {
            if (entry.Instance == null)
            {
                CreateSpawnerInstance(entry);
            }
        }
    }

    /// <summary>
    /// プレイヤーの進行に応じてチャンク生成と描画更新を行う
    /// </summary>
    public void UpdateStageFlow()
    {
        // プレイヤーの現在セル位置を更新
        //playerCell = WorldToGrid(player.transform.position);

        // 可視範囲のZ座標を計算
        int needMinZ = playerCell.z - renderDepthBackward;
        int needMaxZ = playerCell.z + renderDepthForward;

        // 生成済み範囲をチェックし、不足があればチャンク生成
        while (lastGeneratedZ <= needMaxZ)
        {
            GenerateChunkAt(lastGeneratedZ);
            lastGeneratedZ += gridChunkSize;
        }

        // 可視範囲を更新
        UpdateRenderArea();
    }

    //==================================================
    // 内部状態
    //==================================================
    // プレイヤーの現在セル位置
    private Vector3Int playerCell = Vector3Int.zero;

    // 直近で生成したチャンクの末尾Z位置
    private int lastGeneratedZ = 0;

    // プレイヤーの参照
    private GameObject player;

    // ステージ自動生成の参照
    private StageGenerator stageGenerator;

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

    // グリッドのX軸に対する幅(マス単位)
    [SerializeField] private int gridWidth = 10;

    // チャンクサイズ(マス単位)
    [SerializeField] private int gridChunkSize = 16;

    // 地形レイヤー（プレハブ）
    private Dictionary<Vector3Int, GameObject> terrainPrefabs = new Dictionary<Vector3Int, GameObject>();
    
    // 静的障害物レイヤー（プレハブ）
    private Dictionary<Vector3Int, GameObject> staticObstaclePrefabs = new Dictionary<Vector3Int, GameObject>();

    // スポナーレイヤー （プレハブ）
    private Dictionary<Vector3Int, GameObject> spawnerPrefabs = new Dictionary<Vector3Int, GameObject>();

    //
    // スポナー管理
    //
    private List<SpawnerEntry> spawnerEntries = new List<SpawnerEntry>();

    /// <summary>
    /// GridManager 内部で管理するスポナーインスタンス情報。
    /// StageData から生成時に変換され、UpdateRenderAreaで可視範囲制御する。
    /// </summary>
    private class SpawnerEntry
    {
        public Vector3Int GridPos { get; }          // スポナーのグリッド位置
        public GameObject Prefab { get; }           // スポナー本体のプレハブ
        public SpawnerConfigBase Config { get; }    // スポナー設定データ
        public GameObject Instance { get; set; }    // シーンに配置されたスポナーインスタンス（未生成時は null）

        /// <summary>
        /// コンストラクタ
        /// スポナーインスタンス情報を保持する。
        /// </summary>
        public SpawnerEntry(SpawnerConfigBase config)
        {
            GridPos = config.Position;
            Prefab = config.SpawnerControllerPrefab;
            Config = config;
            Instance = null;
        }
    }
}

