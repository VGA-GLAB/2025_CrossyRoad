using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ステージチャンクを生成する責務を持つクラス。
/// - GridManager から「Z座標 index にチャンクを生成してほしい」と要求される。
/// - 内部状態は保持せず、要求に応じて StageData を組み立てて返すだけ。
/// - 難易度制御が必要な場合は、引数の worldZ を参照してパターンを選択する。
///
/// 内部ロジック:
/// - 難易度は worldZ を maxDepth(インスペクタ設定)に正規化して 0〜255 にスケーリング。
/// - チャンク内の配置は「累積難易度 accumulated」が「期待値 expectedTotal」を超えるまで交互に試みる。
/// - 連続障害物にならないよう、配置した連続レーンの後1ラインを Grass に固定する。
///
/// </summary>
public class StageGenerator : MonoBehaviour
{
    private int width;             // マップのタイル幅
    private int chunkSize;         // チャンクのタイルサイズ(Z軸の奥行き)

    // 難易度スケーリング用（インスペクタから指定可能）

    // プレイ時間上限（Zマス数）難易度が上限に到達する座標
    // 上限以降は常にMAX値となる
    [SerializeField] private int maxDepth = 256;

    // チャンクの基礎難易度量
    [SerializeField] private float baseDensity = 1f;

    // 難易度255に向けて増える係数
    [SerializeField] private float densitySlope = 0.05f;

    // 静的障害物スケール用
    // 難易度下限の静的障害物の密度
    [SerializeField] private float minStaticDensity = 0.05f;
    // 難易度上限の静的障害物の密度
    [SerializeField] private float maxStaticDensity = 0.30f;
    // 橋・動的障害物の配置状況に応じてさらに加算される密度
    [SerializeField] private float bonusStaticFactor = 0.10f;

    // 実行時用 Config のキャッシュ
    private readonly List<BridgeSpawnerGroupEntry> bridgeGroupEntries = new List<BridgeSpawnerGroupEntry>();
    private readonly List<DynamicObstaclesGroupEntry> dynamicGroupEntries = new List<DynamicObstaclesGroupEntry>();
    
    public void Initialize(int width, int chunkSize)
    {
        // 重複されて呼ばれてもいいようクリア処理
        Clear();

        // 各種パラメータをメンバーに保持
        this.width = width;
        this.chunkSize = chunkSize;

        // BridgeSpawnerGroupConfigSO をまとめてロード
        var groupSOs = Resources.LoadAll<BridgeSpawnerGroupConfigSO>("BridgeSpawnerGroupConfigs");
        foreach (var groupSO in groupSOs)
        {
            var configs = new List<BridgeSpawnerConfig>();
            foreach (var configSO in groupSO.bridgeConfigs)
            {
                configs.Add(configSO.ToRuntimeConfig());
            }

            var entry = new BridgeSpawnerGroupEntry(groupSO.difficulty, configs);
            bridgeGroupEntries.Add(entry);
        }

        // DynamicObstaclesSpawnerConfigSO も同様にロード
        var dynamicGroupSOs = Resources.LoadAll<DynamicObstaclesSpawnerGroupConfigSO>("DynamicObstaclesSpawnerGroupConfigs");
        foreach (var groupSO in dynamicGroupSOs)
        {
            var configs = new List<DynamicObstaclesSpawnerConfig>();
            foreach (var configSO in groupSO.dynamicObstaclesConfigs)
            {
                configs.Add(configSO.ToRuntimeConfig());
            }
            dynamicGroupEntries.Add(new DynamicObstaclesGroupEntry(groupSO.difficulty, configs));
        }
    }

    /// <summary>
    /// メンバー変数のクリア・解放
    /// </summary>
    public void Clear()
    {
        bridgeGroupEntries.Clear();
        dynamicGroupEntries.Clear();
    }

    /// <summary>
    /// 指定されたワールドZ座標に対応するチャンクの StageData を生成する。
    /// </summary>
    /// <param name="worldZ">チャンクの先頭Z座標（ワールド座標系）</param>
    /// <returns>生成された StageData</returns>
    public StageData GenerateChunk(int worldZ)
    {
        var stageData = new StageData
        {
            width = this.width,
            depth = chunkSize
        };

        // 難易度を進行度に応じて0～255にスケーリングして保持
        int difficulty = CalculateDifficulty(worldZ);

        // チャンク内で想定される合計難易度（密度目標）
        float expectedTotal = baseDensity + densitySlope * difficulty;
        float accumulated = 0f;

        // 1. 初期化（Grassで埋める）
        for (int i = 0; i < chunkSize; i++)
        {
            int z = worldZ + i;
            AddLane(stageData, z, CellType.Grass);
        }

        // 2. 開始ライン固定
        stageData.laneTypes[worldZ] = CellType.Grass;

        // 交互配置ループ：橋と動的障害物を偏りなく配置試行
        // ループは余裕を持って chunkSize 回まで試す（充分に打ち切れる）
        for (int attempt = 0; attempt < chunkSize; attempt++)
        {
            if (accumulated >= expectedTotal) break;

            bool placed = false;
            if (attempt % 2 == 0)
            {
                placed = TryPlaceBridges(stageData, worldZ, difficulty, out float costDelta);
                if (placed) accumulated += costDelta;
            }
            else
            {
                placed = TryPlaceDynamicObstacles(stageData, worldZ, difficulty, out float costDelta);
                if (placed) accumulated += costDelta;
            }

            // どちらも置けない状況が続くなら早期終了
            if (!placed && attempt > 8 && accumulated == 0f)
            {
                // 初期数回の試行で何も置けないなら諦める
                break;
            }
        }

        // 5. 静的障害物の配置
        PlaceStaticObstacles(stageData, worldZ);

        return stageData;
    }

    private void AddLane(StageData stageData, int z, CellType type)
    {
        stageData.laneTypes[z] = type;
    }

    private void AddStaticObstacle(StageData stageData, Vector3Int pos, ObstacleType type)
    {
        stageData.staticObstacles[pos] = type;
    }

    private void AddSpawner(StageData stageData, SpawnerConfigBase config)
    {
        stageData.spawnerConfigs.Add(config);
    }

    /// <summary>
    /// 橋配置の試行。
    /// - 難易度フィルタリング: entry.difficulty <= difficulty
    /// - 候補ライン探索: requiredLines 連続の Grass を確保
    /// - 配置: 連続レーンを River に変更、前後1ラインを Grass 固定、SpawnerConfig 追加
    /// - 戻り値: 配置の成否と累積コストの増分
    /// </summary>
    private bool TryPlaceBridges(StageData stageData, int worldZ, int difficulty, out float costDelta)
    {
        costDelta = 0f;

        // フィルタリング
        var groupCandidates = bridgeGroupEntries.Where(e => e.difficulty <= difficulty).ToList();
        if (groupCandidates.Count == 0) return false;

        // グループ&コンフィグ選択（ランダム）
        var group = groupCandidates[Random.Range(0, groupCandidates.Count)];
        var config = group.bridgeConfigs[Random.Range(0, group.bridgeConfigs.Count)];

        int bridgeWidth = group.bridgeConfigs.Count;    // 橋本体の連続レーン数
        int totalNeeded = bridgeWidth + 1;              // 後ろにGrassバッファを1レーン要求

        // 候補開始Zを収集
        var possibleStarts = FindContiguousGrassStarts(stageData, worldZ, totalNeeded);
        if (possibleStarts.Count == 0) return false;

        // 開始位置選択
        int startZ = possibleStarts[Random.Range(0, possibleStarts.Count)];

        // 配置:
        // 橋本体を River に、直後を Grass に配置
        ApplyLaneTypeWithBuffer(stageData, startZ, bridgeWidth, CellType.River);

        // 橋は構成要素ごとに SpawnerConfig を追加
        int spawnerZ = startZ;
        foreach (var cfg in group.bridgeConfigs)
        {
            // cfg.Position をコピーして、Zだけ startZ に補正
            var pos = new Vector3Int(cfg.Position.x, -1, spawnerZ);
            var newCfg = new BridgeSpawnerConfig(
                pos,
                cfg.SpawnerControllerPrefab,
                cfg.SpawnTargetPrefabs,
                cfg.SpawnInterval,
                cfg.BridgeInterval,
                cfg.BridgeCountPerLane
            );

            AddSpawner(stageData, newCfg);

            spawnerZ++;
        }

        costDelta = group.difficulty;
        return true;
    }

    /// <summary>
    /// 動的障害物配置の試行。
    /// - 難易度フィルタリング
    /// - 候補ライン探索: requiredLines 連続の Grass を確保してから、その区間を Road に変換してスパナーを置く
    ///   （仕様によっては最初から Road 上に置くなら、Road連続区間探索に切り替えてください）
    /// - 配置: 連続レーンを Road に変更、前後1ラインを Grass 固定、SpawnerConfig 追加
    /// - 戻り値: 配置の成否と累積コストの増分
    /// </summary>
    private bool TryPlaceDynamicObstacles(StageData stageData, int worldZ, int difficulty, out float costDelta)
    {
        costDelta = 0f;

        var groupCandidates = dynamicGroupEntries.Where(e => e.difficulty <= difficulty).ToList();
        if (groupCandidates.Count == 0) return false;

        var group = groupCandidates[Random.Range(0, groupCandidates.Count)];
        var config = group.obstacleConfigs[Random.Range(0, group.obstacleConfigs.Count)];

        int laneWidth = group.obstacleConfigs.Count;    // 連続で車道/動的レーンを作る幅。なければ1に変更してもOK。
        int totalNeeded = laneWidth + 1;                // 後ろにGrassバッファを1レーン要求

        // まず安全に確保できる連続Grassを探す（その区間をRoad化する前提）
        var possibleStarts = FindContiguousGrassStarts(stageData, worldZ, totalNeeded);
        if (possibleStarts.Count == 0) return false;

        int startZ = possibleStarts[Random.Range(0, possibleStarts.Count)];
        var pos = new Vector3Int(0, -1, startZ); // ← worldZ + ローカルZを反映

        // 配置:
        // 本体を Road に、直後を Grass に配置
        ApplyLaneTypeWithBuffer(stageData, startZ, laneWidth, CellType.Road);

        // 動的障害物も幅に応じて複数 SpawnerConfig を追加
        int spawnerZ = startZ;
        foreach (var cfg in group.obstacleConfigs)
        {
            var posSpawner = new Vector3Int(cfg.Position.x, -1, spawnerZ);

            var newCfg = new DynamicObstaclesSpawnerConfig(
                posSpawner,
                cfg.SpawnerControllerPrefab,
                cfg.SpawnTargetPrefabs,
                cfg.MoveSpeed,
                cfg.MoveRight,
                cfg.BaseSpawnInterval,
                cfg.SpawnIntervalJitter,
                cfg.MinBatchCount,
                cfg.MaxBatchCount,
                cfg.BatchSpacing,
                cfg.LifeTime
            );

            AddSpawner(stageData, newCfg);

            spawnerZ++;
        }

        costDelta = group.difficulty;
        return true;
    }

    private void PlaceStaticObstacles(StageData stageData, int worldZ)
    {
        // 難易度を0〜1に正規化
        int difficulty = CalculateDifficulty(worldZ);
        float t = difficulty / 255f;

        // 基本密度を補間
        float baseDensity = Mathf.Lerp(minStaticDensity, maxStaticDensity, t);

        // 動的障害物と橋の影響を考慮
        int dynamicCount = stageData.spawnerConfigs.OfType<DynamicObstaclesSpawnerConfig>().Count();
        int bridgeCount = stageData.spawnerConfigs.OfType<BridgeSpawnerConfig>().Count();

        // 合計をチャンクサイズで割って比率化
        int totalObstacleLanes = dynamicCount + bridgeCount;
        float obstacleRatio = Mathf.Clamp01(totalObstacleLanes / (float)chunkSize);

        // 実効密度
        float effectiveDensity = baseDensity * (1f - obstacleRatio);
        effectiveDensity += baseDensity * bonusStaticFactor;

        // 経路確保: ランダムに1列を除外
        int safeColumn = Random.Range(0, width);

        // Grass 上に障害物を配置
        int startZ = (worldZ == 0 ? worldZ + 1 : worldZ);   // ゲーム開始時のスタートレーンは除外する
        for (int z = startZ; z < worldZ + chunkSize; z++)
        {
            if (stageData.laneTypes[z] != CellType.Grass) continue;

            for (int x = 0; x < width; x++)
            {
                if (x == safeColumn) continue; // 経路確保

                if (Random.value < effectiveDensity)
                {
                    var pos = new Vector3Int(x, 1, z);
                    stageData.staticObstacles[pos] = ObstacleType.Tree;
                }
            }
        }
    }

    /// <summary>
    /// worldZ に応じた難易度を 0〜255 で返す。
    /// </summary>
    private int CalculateDifficulty(int worldZ)
    {
        if (worldZ >= maxDepth)
            return 255;

        // 0〜maxDepth を 0〜255 にスケーリング
        float ratio = (float)worldZ / maxDepth;
        return Mathf.Clamp(Mathf.FloorToInt(ratio * 255f), 0, 255);
    }

    /// <summary>
    /// チャンク内で requiredLines 連続の Grass が確保できる開始Z候補を列挙。
    /// チャンク先頭(worldZ)は安全地帯のため開始候補から除外。
    /// </summary>
    private List<int> FindContiguousGrassStarts(StageData stageData, int worldZ, int requiredLines)
    {
        var starts = new List<int>();
        int firstZ = worldZ + 1; // 先頭は安全地帯
        int lastStartZ = worldZ + chunkSize - requiredLines;

        for (int z = firstZ; z <= lastStartZ; z++)
        {
            bool ok = true;
            for (int i = 0; i < requiredLines; i++)
            {
                int checkZ = z + i;
                if (!stageData.laneTypes.TryGetValue(checkZ, out var cell) || cell != CellType.Grass)
                {
                    ok = false;
                    break;
                }
            }
            if (ok) starts.Add(z);
        }
        return starts;
    }

    /// <summary>
    /// 連続 requiredLines を laneType に設定し、その後1ラインを Grass に固定して連続障害物を防ぐ。
    /// チャンク境界は安全に無視される（存在チェック込み）。
    /// </summary>
    private void ApplyLaneTypeWithBuffer(StageData stageData, int startZ, int segmentWidth, CellType laneType)
    {
        // 本体設定
        for (int i = 0; i < segmentWidth; i++)
        {
            int z = startZ + i;
            stageData.laneTypes[z] = laneType;
        }

        // 後バッファ（Grass固定）
        // Note: 前バッファも必要であればコメントを外す
        //int beforeZ = startZ - 1;
        int afterZ = startZ + segmentWidth;

        //if (stageData.laneTypes.ContainsKey(beforeZ))
        //    stageData.laneTypes[beforeZ] = CellType.Grass;

        if (stageData.laneTypes.ContainsKey(afterZ))
            stageData.laneTypes[afterZ] = CellType.Grass;
    }


}
