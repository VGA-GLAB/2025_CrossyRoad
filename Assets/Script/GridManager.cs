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
    // 地面タイルのプレハブ群
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject riverPrefab;

    // 道路タイルのプレハブ群
    [System.Serializable]
    public struct RoadPrefabEntry
    {
        public CellType cellType;
        public GameObject roadPrefab;
    }
    public List<RoadPrefabEntry> roadPrefabEntries;
    private Dictionary<CellType, GameObject> roadPrefabMap;

    // 静的障害物のプレハブ群
    [SerializeField] private GameObject tankPrefab;
    [SerializeField] private GameObject controlPanelPrefab;
    [SerializeField] private GameObject pressMachinePrefab;
    [SerializeField] private GameObject cardboardboxPrefab;

    // 川の両端におく不可視のコリジョン(当たったらゲームオーバーとなる)
    [SerializeField] private GameObject invisibleObstaclePrefab;
    // ゲームオーバー判定用の不可視障害物
    private Dictionary<Vector3Int, GameObject> invisibleObstaclesPrefabs = new Dictionary<Vector3Int, GameObject>();

    //==================================================
    // Unity標準イベント
    //==================================================

    /// <summary>
    /// Awake()
    /// CellType と道路Prefabの対応関係を辞書に構築する
    /// </summary>
    void Awake()
    {

        roadPrefabMap = new Dictionary<CellType, GameObject>();
        foreach (var entry in roadPrefabEntries)
        {
            if (!roadPrefabMap.ContainsKey(entry.cellType))
            {
                roadPrefabMap.Add(entry.cellType, entry.roadPrefab);
            }
        }
    }

    /// <summary>
    /// Start()
    /// - StageGeneratorの初期化
    /// - 最初のチャンクを生成
    /// - プレイヤー初期セルを登録
    /// </summary>
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
    /// OnDestroy()
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
        cellEntitiesMap.Clear();

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

    /// <summary>
    /// ワールド座標をグリッド座標(GridPos2D)に変換する。
    /// Yは無視してX,Zのみを返す。
    /// </summary>
    public GridPos2D WorldToGrid2D(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - cellSize / 2f) / cellSize);
        int z = Mathf.FloorToInt((worldPos.z - cellSize / 2f) / cellSize);
        return new GridPos2D(x, z);
    }

    /// <summary>
    /// グリッド座標(GridPos2D)をワールド座標(Vector3)に変換する。
    /// セルの中心座標を返す。Yは呼び出し側で補正する。
    /// </summary>
    public Vector3 GridToWorld2D(GridPos2D gridPos, float y = 0f)
    {
        float x = gridPos.X * cellSize + (cellSize / 2f);
        float z = gridPos.Z * cellSize + (cellSize / 2f);
        return new Vector3(x, y, z);
    }

    //==================================================
    // 2. 占有管理系（静的障害物用）
    //==================================================

    /// <summary>
    /// 指定セルが空いているかどうかを返す。
    /// 静的障害物の入力キャンセル判定に使用。
    /// - 障害物が存在しなければ true
    /// - 複数障害物があっても「占有」とみなす
    /// </summary>
    public bool IsCellFree(GridPos2D pos)
    {
        if (cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            return entities.Obstacles.Count == 0;
        }
        return true; // cellEntitiesMapに存在しないセルは空きとみなす

    }

    /// <summary>
    /// 指定セルが空いているかどうかを返す（Vector3Int版）。
    /// Yは無視して X,Z のみを利用する。
    /// </summary>
    public bool IsCellFree(Vector3Int pos3D)
    {
        return IsCellFree(new GridPos2D(pos3D.x, pos3D.z));
    }

    /// <summary>
    /// 指定セルを「埋まっている」と登録する。
    /// 静的障害物配置時に呼ばれる。
    /// </summary>
    public void OccupyCell(GridPos2D pos, ObstacleType type)
    {
        if (!cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            entities = new CellEntities();
            cellEntitiesMap[pos] = entities;
        }

        entities.Obstacles.Add(type);

        // Prefab生成
        CreateObstaclePrefab(pos, entities);
    }

    /// <summary>
    /// 指定セルを解放する。
    /// 静的障害物削除時に呼ばれる。
    /// </summary>
    public void ReleaseCell(GridPos2D pos)
    {
        if (cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            entities.Obstacles.Clear();

            // Prefab破棄
            var gridPos = new Vector3Int(pos.X, 1, pos.Z);
            if (staticObstaclePrefabs.TryGetValue(gridPos, out var obj))
            {
                UnityEngine.Object.Destroy(obj);
                staticObstaclePrefabs.Remove(gridPos);
            }
        }
    }

    /// <summary>
    /// 指定セルを解放する。（Vector3Int版）。
    /// 静的障害物削除時に呼ばれる。
    /// </summary>
    public void ReleaseCell(Vector3Int pos3D)
    {
        ReleaseCell(new GridPos2D(pos3D.x, pos3D.z));
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
    /// Vector3Int のグリッド座標からセル種別を返す。
    /// Yは無視して X,Z のみを利用する。
    /// </summary>
    public CellType GetCellType(Vector3Int pos3D)
    {
        return GetCellType(new GridPos2D(pos3D.x, pos3D.z));
    }

    /// <summary>
    /// 指定セルの状態を返す。
    /// - 障害物が1つでもあれば Occupied を返す
    /// - 障害物がなければ Terrains の先頭を返す（複数ある場合は不定で先頭の要素を返却）
    /// - Terrains が空なら Empty を返す
    /// </summary>
    public CellType GetCellType(GridPos2D pos)
    {
        if (cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            // 障害物優先
            if (entities.Obstacles.Count > 0)
            {
                return CellType.Occupied;
            }

            // Terrainが存在すれば先頭を返す（複数ある場合は不定）
            if (entities.Terrains.Count > 0)
            {
                return entities.Terrains[0];
            }
        }

        return CellType.Empty;
    }


    /// <summary>
    /// 指定セルの地形タイプをすべて返す。
    /// - 通常は1つだけだが、複数積層している場合は全て返却する。
    /// - 呼び出し側で優先順位を決めて利用すること。
    /// </summary>
    public List<CellType> GetTerrainCellType(GridPos2D pos)
    {
        if (cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            // Terrainsリストをそのまま返す（複数積層対応）
            return new List<CellType>(entities.Terrains);
        }

        // 該当セルが存在しない場合は空リスト
        return new List<CellType>();
    }

    /// <summary>
    /// 指定セルの地形タイプをすべて返す。（Vector3Int版）。
    /// - 通常は1つだけだが、複数積層している場合は全て返却する。
    /// - 呼び出し側で優先順位を決めて利用すること。
    /// </summary>
    public List<CellType> GetTerrainCellType(Vector3Int pos3D)
    {
        return GetTerrainCellType(new GridPos2D(pos3D.x, pos3D.z));
    }
    //==================================================
    // 4. 配置・生成系
    //==================================================
    /// <summary>
    /// 指定セルに地形Prefabを配置する。
    /// すでに描画済みなら何もしない。
    /// </summary>
    private void CreateTerrainPrefab(GridPos2D pos2D, CellEntities entities)
    {
        foreach (var type in entities.Terrains)
        {
            // 既に生成済みならスキップ
            if (terrainPrefabs.ContainsKey(new Vector3Int(pos2D.X, 0, pos2D.Z)))
                continue;

            GameObject prefab = null;
            switch (type)
            {
                case CellType.Grass: prefab = grassPrefab; break;
                case CellType.River:
                    // 川の地形タイル
                    prefab = riverPrefab;

                    // 両端InvisibleObstacle生成（端セルのみ）
                    if (invisibleObstaclePrefab != null)
                    {
                        // 左端コリジョンのためのプレハブ生成
                        if (pos2D.X == 0)
                        {
                            var leftPos = new Vector3Int(0, 1, pos2D.Z);
                            if (!invisibleObstaclesPrefabs.ContainsKey(leftPos))
                            {
                                Vector3 worldPos = GridToWorld(leftPos);
                                worldPos.y = 0.5f * cellSize;
                                var leftObj = Instantiate(invisibleObstaclePrefab, worldPos, Quaternion.identity, this.transform);
                                invisibleObstaclesPrefabs[leftPos] = leftObj;
                            }
                        }
                        // 右端コリジョンのためのプレハブ生成
                        else if (pos2D.X == gridWidth - 1)
                        {
                            var rightPos = new Vector3Int(gridWidth - 1, 1, pos2D.Z);
                            if (!invisibleObstaclesPrefabs.ContainsKey(rightPos))
                            {
                                Vector3 worldPos = GridToWorld(rightPos);
                                worldPos.y = 0.5f * cellSize;
                                var rightObj = Instantiate(invisibleObstaclePrefab, worldPos, Quaternion.identity, this.transform);
                                invisibleObstaclesPrefabs[rightPos] = rightObj;
                            }
                        }
                    }

                    break;

                case CellType.Empty: prefab = null; break;
                // RoadGear、RoadRobotの場合
                case CellType.RoadGear:
                case CellType.RoadRobot:
                    if (roadPrefabMap.TryGetValue(type, out var road))
                        prefab = road;
                    break;
            }

            if (prefab != null)
            {
                Vector3 worldPos = GridToWorld(new Vector3Int(pos2D.X, 0, pos2D.Z));
                worldPos.y = -0.5f * cellSize; // キューブの上面がゼロ座標になるよう、地形の高さ補正
                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);

                // TerrainはY=0で管理
                terrainPrefabs[new Vector3Int(pos2D.X, 0, pos2D.Z)] = obj;
            }
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
    private void CreateObstaclePrefab(GridPos2D pos2D, CellEntities entities)
    {
        foreach (var type in entities.Obstacles)
        {
            var gridPos = new Vector3Int(pos2D.X, 1, pos2D.Z); // 障害物はY=1で管理

            if (staticObstaclePrefabs.ContainsKey(gridPos))
                continue;

            GameObject prefab = null;
            switch (type)
            {
                case ObstacleType.Tank :            prefab = tankPrefab;            break;
                case ObstacleType.ControlPanel :    prefab = controlPanelPrefab;    break;
                case ObstacleType.PressMachine:     prefab = pressMachinePrefab;    break;
                case ObstacleType.Cardboardbox:     prefab = cardboardboxPrefab;    break;
            }

            if (prefab != null)
            {
                Vector3 worldPos = GridToWorld(gridPos);
                worldPos.y = 0.5f * cellSize; // キューブの下面がゼロ座標となるようY座標を調整
                GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);

                staticObstaclePrefabs[gridPos] = obj;
            }
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
    private void CreateSpawnerInstance(GridPos2D pos2D, CellEntities entities)
    {

        foreach (var config in entities.Spawners)
        {
            var gridPos = config.Position; // PositionにはX,Zが含まれる（Y=-1固定）

            // 既に生成済みならスキップ
            if (spawnerPrefabs.ContainsKey(gridPos))
                continue;

            GameObject prefab = config.SpawnerControllerPrefab;
            if (prefab == null) continue;

            Vector3 worldPos = GridToWorld(gridPos);
            GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);

            spawnerPrefabs[gridPos] = instance;

            // Spawnerの初期化
            // ToDo:Spawnerの種類が増えたら追加する
            // 共通インターフェース ISpawner を導入する手もあるが柔軟性を失うのでこのスタイルでいいかも

            // 橋のスポナー
            if (config is BridgeSpawnerConfig bridgeConfig)
            {
                var bridgeSpawn = instance.GetComponent<BridgeSpawn>();
                bridgeSpawn?.Initialize(bridgeConfig);
            }

            // 動的障害物のスポナー
            else if (config is DynamicObstaclesSpawnerConfig dynamicConfig)
            {
                var dynamicSpawner = instance.GetComponent<DynamicObstaclesSpawner>();
                dynamicSpawner?.Initialize(dynamicConfig);
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
    public void PlaceObstacleCell(GridPos2D pos, ObstacleType type)
    {
        // セル内にオブジェクトを生成し、占有情報を登録する
        if (!cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            entities = new CellEntities();
            cellEntitiesMap[pos] = entities;
        }

        entities.Obstacles.Add(type);

        // Prefab生成
        CreateObstaclePrefab(pos, entities);
    }

    /// <summary>
    /// 指定セルに障害物を配置する（Vector3Int版）。
    /// Yは無視して X,Z のみを利用する。
    /// </summary>
    public void PlaceObstacleCell(Vector3Int pos3D, ObstacleType type)
    {
        PlaceObstacleCell(new GridPos2D(pos3D.x, pos3D.z), type);
    }

    /// <summary>
    /// 指定セルの障害物を削除する。
    /// - 全削除か、特定タイプのみ削除かを選べるようにオーバーロード
    /// </summary>
    public void ClearObstacleCell(GridPos2D pos)
    {
        if (cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            entities.Obstacles.Clear();

            // Prefab破棄
            var gridPos = new Vector3Int(pos.X, 1, pos.Z);
            if (staticObstaclePrefabs.TryGetValue(gridPos, out var obj))
            {
                UnityEngine.Object.Destroy(obj);
                staticObstaclePrefabs.Remove(gridPos);
            }
        }
    }

    /// <summary>
    /// 指定セルの障害物を削除する。（Vector3Int版）。
    /// Yは無視して X,Z のみを利用する。
    /// </summary>
    public void ClearObstacleCell(Vector3Int pos3D)
    {
        ClearObstacleCell(new GridPos2D(pos3D.x, pos3D.z));
    }

    /// <summary>
    /// 指定セルから特定タイプの障害物だけ削除する。
    /// - 複数障害物がある場合に部分削除可能
    /// </summary>
    public void ClearObstacleCell(GridPos2D pos, ObstacleType type)
    {
        if (cellEntitiesMap.TryGetValue(pos, out var entities))
        {
            if (entities.Obstacles.Remove(type))
            {
                // Prefab破棄（単純化のため全削除→再生成でもよい）
                var gridPos = new Vector3Int(pos.X, 1, pos.Z);
                if (staticObstaclePrefabs.TryGetValue(gridPos, out var obj))
                {
                    UnityEngine.Object.Destroy(obj);
                    staticObstaclePrefabs.Remove(gridPos);
                }

                // 残っている障害物があれば再生成
                if (entities.Obstacles.Count > 0)
                {
                    CreateObstaclePrefab(pos, entities);
                }
            }
        }
    }

    /// <summary>
    /// 指定セルから特定タイプの障害物だけ削除する。（Vector3Int版）。
    /// - 複数障害物がある場合に部分削除可能
    /// Yは無視して X,Z のみを利用する。
    /// </summary>
    public void ClearObstacleCell(Vector3Int pos3D, ObstacleType type)
    {
        ClearObstacleCell(new GridPos2D(pos3D.x, pos3D.z), type);
    }

    /// <summary>
    /// StageData を受け取り、マップ全体のセル情報を cellEntitiesMap に構築する。
    /// </summary>
    public void BuildStage(StageData data)
    {
        // 既存データをクリア
        cellEntitiesMap.Clear();

        // 地形レーンを登録
        foreach (var lane in data.laneTypes)
        {
            int z = lane.Key;
            CellType type = lane.Value;

            for (int x = 0; x < data.width; x++)
            {
                var pos2D = new GridPos2D(x, z);

                if (!cellEntitiesMap.TryGetValue(pos2D, out var entities))
                {
                    entities = new CellEntities();
                    cellEntitiesMap[pos2D] = entities;
                }

                // Terrainを追加（複数積層対応）
                entities.Terrains.Add(type);
            }
        }

        // 静的障害物を登録
        foreach (var obstacle in data.staticObstacles)
        {
            var gridPos = obstacle.Key;
            var pos2D = new GridPos2D(gridPos.x, gridPos.z);

            if (!cellEntitiesMap.TryGetValue(pos2D, out var entities))
            {
                entities = new CellEntities();
                cellEntitiesMap[pos2D] = entities;
            }

            entities.Obstacles.Add(obstacle.Value);
        }

        // Spawner を登録
        foreach (var config in data.spawnerConfigs)
        {
            var pos2D = new GridPos2D(config.Position.x, config.Position.z);

            if (!cellEntitiesMap.TryGetValue(pos2D, out var entities))
            {
                entities = new CellEntities();
                cellEntitiesMap[pos2D] = entities;
            }

            entities.Spawners.Add(config);
        }
    }

    /// <summary>
    /// プレイヤー位置を基準に描画範囲を更新する
    /// </summary>
    public void UpdateRenderArea()
    {
        var center = playerCell;
        var newVisible = new HashSet<GridPos2D>();

        // 幅方向: -renderWidth ～ +renderWidth
        // 奥行き方向: -renderDepthBackward ～ +renderDepthForward
        for (int dx = -renderWidth; dx <= renderWidth; dx++)
        {
            for (int dz = -renderDepthBackward; dz <= renderDepthForward; dz++)
            {
                var pos2D = new GridPos2D(center.x + dx, center.z + dz);
                newVisible.Add(pos2D);
                if (cellEntitiesMap.TryGetValue(pos2D, out var entities))
                {
                    // 地形生成
                    CreateTerrainPrefab(pos2D, entities);

                    // 障害物生成
                    CreateObstaclePrefab(pos2D, entities);

                    // Spawner生成
                    CreateSpawnerInstance(pos2D, entities);
                }
            }
        }

        // 範囲外になったセルを破棄
        // 地形を破棄
        foreach (var kv in new List<Vector3Int>(terrainPrefabs.Keys))
        {
            var pos2D = new GridPos2D(kv.x, kv.z);
            if (!newVisible.Contains(pos2D))
            {
                DestroyTerrainPrefab(kv);
            }
        }

        // 静的障害物を破棄
        foreach (var kv in new List<Vector3Int>(staticObstaclePrefabs.Keys))
        {
            var pos2D = new GridPos2D(kv.x, kv.z);
            if (!newVisible.Contains(pos2D))
            {
                DestroyObstaclePrefab(kv);
            }
        }

        // スポナーを破棄
        foreach (var kv in new List<Vector3Int>(spawnerPrefabs.Keys))
        {
            var pos2D = new GridPos2D(kv.x, kv.z);
            if (!newVisible.Contains(pos2D))
            {
                Destroy(spawnerPrefabs[kv]);
                spawnerPrefabs.Remove(kv);
            }
        }

        // 川両端の不可視のコリジョンを破棄
        foreach (var kv in new List<Vector3Int>(invisibleObstaclesPrefabs.Keys))
        {
            var pos2D = new GridPos2D(kv.x, kv.z);
            if (!newVisible.Contains(pos2D))
            {
                Destroy(invisibleObstaclesPrefabs[kv]);
                invisibleObstaclesPrefabs.Remove(kv);
            }
        }

    }

    /// <summary>
    /// 指定した worldZ のチャンクを生成し、cellEntitiesMap に統合する。
    /// Prefab生成は BuildChunkVisuals に委譲する。
    /// </summary>
    private void GenerateChunkAt(int worldZ)
    {
        // StageData を生成
        StageData data = stageGenerator.GenerateChunk(worldZ);

        // StageData を cellEntitiesMap に反映
        foreach (var lane in data.laneTypes)
        {
            int z = lane.Key;
            CellType type = lane.Value;

            for (int x = 0; x < data.width; x++)
            {
                var pos2D = new GridPos2D(x, z);

                if (!cellEntitiesMap.TryGetValue(pos2D, out var entities))
                {
                    entities = new CellEntities();
                    cellEntitiesMap[pos2D] = entities;
                }

                entities.Terrains.Add(type);
            }
        }

        foreach (var obstacle in data.staticObstacles)
        {
            var gridPos = obstacle.Key;
            var pos2D = new GridPos2D(gridPos.x, gridPos.z);

            if (!cellEntitiesMap.TryGetValue(pos2D, out var entities))
            {
                entities = new CellEntities();
                cellEntitiesMap[pos2D] = entities;
            }

            entities.Obstacles.Add(obstacle.Value);
        }

        foreach (var config in data.spawnerConfigs)
        {
            var pos2D = new GridPos2D(config.Position.x, config.Position.z);

            if (!cellEntitiesMap.TryGetValue(pos2D, out var entities))
            {
                entities = new CellEntities();
                cellEntitiesMap[pos2D] = entities;
            }

            entities.Spawners.Add(config);
        }

        // Prefab生成は別メソッドに委譲しシーンに反映する
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
            for (int x = 0; x < data.width; x++)
            {
                var pos2D = new GridPos2D(x, z);
                if (cellEntitiesMap.TryGetValue(pos2D, out var entities))
                {
                    CreateTerrainPrefab(pos2D, entities);
                }
            }
        }

        // 静的障害物
        foreach (var obstacle in data.staticObstacles)
        {
            var pos2D = new GridPos2D(obstacle.Key.x, obstacle.Key.z);
            if (cellEntitiesMap.TryGetValue(pos2D, out var entities))
            {
                CreateObstaclePrefab(pos2D, entities);
            }
        }

        // スポナー
        foreach (var config in data.spawnerConfigs)
        {
            var pos2D = new GridPos2D(config.Position.x, config.Position.z);
            if (cellEntitiesMap.TryGetValue(pos2D, out var entities))
            {
                CreateSpawnerInstance(pos2D, entities);
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
    private Dictionary<GridPos2D, CellEntities> cellEntitiesMap = new Dictionary<GridPos2D, CellEntities>();
    
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

    /// <summary>
    /// グリッド座標を2D (X,Z) で表現する構造体。
    /// Vector3Int の Y補正をなくすために導入。
    /// </summary>
    public struct GridPos2D
    {
        public int X;
        public int Z;

        public GridPos2D(int x, int z)
        {
            X = x;
            Z = z;
        }

        // Dictionaryキーとして使うためのEquals/GetHashCodeを実装
        public override bool Equals(object obj)
        {
            if (!(obj is GridPos2D)) return false;
            var other = (GridPos2D)obj;
            return X == other.X && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return (X, Z).GetHashCode();
        }
    }

    /// <summary>
    /// 1セルに存在するエンティティ群をまとめるクラス。
    /// - Terrain: 草原/道路/川などの地形
    /// - Obstacles: 木などの静的障害物（複数可）
    /// - Spawners: 橋や動的障害物のスパナー（複数可）
    /// </summary>
    public class CellEntities
    {
        // 地形レイヤー
        public List<CellType> Terrains { get; } = new List<CellType>();
        // 静的障害物レイヤー
        public List<ObstacleType> Obstacles { get; } = new List<ObstacleType>();
        // スポナーレイヤー
        public List<SpawnerConfigBase> Spawners { get; } = new List<SpawnerConfigBase>();
    }

}

