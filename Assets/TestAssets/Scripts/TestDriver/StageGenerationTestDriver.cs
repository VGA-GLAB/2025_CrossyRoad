using UnityEngine;

/// <summary>
/// ステージ生成のテスト用ドライバ。
/// 固定ルールで StageData を生成する。
/// PhaseBeta後にステージ自動生成に差し替え予定。
/// </summary>
public class StageGenerationTestDriver
{
    // 生成したステージデータを保持（必要に応じて外部から参照可能）
    public StageData data = null;

    // ScriptableObjectからロードした実行時用Config
    private BridgeSpawnerConfig bridgeSpawnerConfig;
    private DynamicObstaclesSpawnerConfig dynamicObstaclesSpawnerConfig;


    /// <summary>
    /// テスト用のステージデータを生成して返す。
    /// - 幅20, 奥行き100
    /// - XZ両方向の外周は Empty
    /// - Grass レーンにはランダムで Tree を配置
    /// - Road レーンは一律 Road
    /// - River レーンは一律 River
    /// </summary>
    public void Initialize()
    {
        // Resources/SpawnerConfigs/BridgeSpawnerConfigSO_Default.asset をロード
        var configBridgeSO = Resources.Load<BridgeSpawnerConfigSO>("SpawnerConfigs/BridgeSpawnerConfigSO_Default");
        if (configBridgeSO != null)
        {
            bridgeSpawnerConfig = configBridgeSO.ToRuntimeConfig();
        }
        else
        {
            Debug.LogError("BridgeSpawnerConfigSO_Default がロードできませんでした。");
        }

        // Resources/SpawnerConfigs/DynamicObstaclesSpawnerConfigSO_Default.asset をロード
        var configDynamicObstaclesSO = Resources.Load<DynamicObstaclesSpawnerConfigSO>("SpawnerConfigs/DynamicObstaclesSpawnerConfigSO_Default");
        if (configDynamicObstaclesSO != null)
        {
            dynamicObstaclesSpawnerConfig = configDynamicObstaclesSO.ToRuntimeConfig();
        }
        else
        {
            Debug.LogError("DynamicObstaclesConfigSO_Default がロードできませんでした。");
        }
    }

    /// <summary>
    /// テスト用のステージデータを生成して返す。
    /// - 幅20, 奥行き100
    /// - XZ両方向の外周は Empty
    /// - Grass レーンにはランダムで Tree を配置
    /// - Road レーンは一律 Road
    /// - River レーンは一律 River
    /// </summary>
    public StageData GenerateTestStage()
    {
        if (data == null)
        {
            data = new StageData();
        }
        data.width = 20;
        data.depth = 100;

        // レーンごとの地形を決定
        int roadLaneIndex = 0;
        for (int z = 0; z < data.depth; z++)
        {
            // Z方向の外周は Empty
            if (z == 0 || z == data.depth - 1)
            {
                data.laneTypes[z] = CellType.Empty;
                continue;
            }

            // サンプルルール: 3レーンごとに Grass, Road, River を繰り返す
            if (z % 3 == 0)
                data.laneTypes[z] = CellType.Grass;
            else if (z % 3 == 1)
            {
                data.laneTypes[z] = dynamicObstaclesSpawnerConfig.RoadCellType;

                // ↓Roadレーンなら DynamicObstaclesSpawnerConfig を登録↓
                var pos = new Vector3Int(0, -1, z); // Y=-1はSpawner配置用の慣例

                // 交互に左右に配置
                bool isMoveRight = (roadLaneIndex % 2 == 1);
                roadLaneIndex++;

                if (!isMoveRight)
                {
                    pos += new Vector3Int(data.width - 1, 0, 0); // 右レーンは右端に配置
                }

                var spawner = new DynamicObstaclesSpawnerConfig(
                    pos,
                    dynamicObstaclesSpawnerConfig.SpawnerControllerPrefab,
                    dynamicObstaclesSpawnerConfig.SpawnTargetPrefabs,
                    dynamicObstaclesSpawnerConfig.MoveSpeed,
                    isMoveRight,
                    dynamicObstaclesSpawnerConfig.BaseSpawnInterval,
                    dynamicObstaclesSpawnerConfig.SpawnIntervalJitter,
                    dynamicObstaclesSpawnerConfig.MinBatchCount,
                    dynamicObstaclesSpawnerConfig.MaxBatchCount,
                    dynamicObstaclesSpawnerConfig.BatchSpacing,
                    dynamicObstaclesSpawnerConfig.LifeTime,
                    dynamicObstaclesSpawnerConfig.RoadCellType
                );

                data.spawnerConfigs.Add(spawner);
                // ↑登録完了↑
                
            }
            else
            {
                data.laneTypes[z] = CellType.River;

                // ↓川レーンなら BridgeSpawnerConfig を登録↓
                var pos = new Vector3Int(0, -1, z);      // Note: 座標はマップ自動生成で確定する
                var spawner = new BridgeSpawnerConfig(
                    pos,
                    bridgeSpawnerConfig.SpawnerControllerPrefab,
                    bridgeSpawnerConfig.SpawnTargetPrefabs,
                    bridgeSpawnerConfig.SpawnInterval,
                    bridgeSpawnerConfig.BridgeInterval,
                    bridgeSpawnerConfig.BridgeCountPerLane,
                    bridgeSpawnerConfig.MoveRight
                );

                data.spawnerConfigs.Add(spawner);
                // ↑登録完了↑
            }
        }

        // Grass レーンにランダムで Tree を配置
        for (int z = 1; z < data.depth - 1; z++)
        {
            if (data.laneTypes[z] == CellType.Grass)
            {
                for (int x = 0; x < data.width; x++)
                {
                    // 20% の確率で木を配置
                    if (Random.value < 0.2f)
                    {
                        // Note: y座標は1固定（地形の上に置く想定）
                        // マップ自動生成や動的障害物配置の際に調整したりY軸をどうするか規定する必要あり
                        Vector3Int pos = new Vector3Int(x, 1, z);
                        data.staticObstacles[pos] = ObstacleType.Tank;
                    }
                }
            }
        }

        return data;
    }
}
