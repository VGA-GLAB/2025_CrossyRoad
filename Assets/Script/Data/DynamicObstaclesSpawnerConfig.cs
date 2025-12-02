using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動的障害物スポナーの実行時設定データ。
/// StageData に格納され、GridManager が解釈して DynamicObstaclesSpawner を生成する。
/// </summary>
public class DynamicObstaclesSpawnerConfig : SpawnerConfigBase
{
    public float MoveSpeed { get; }
    public bool MoveRight { get; }
    public float BaseSpawnInterval { get; }
    public float SpawnIntervalJitter { get; }
    public int MinBatchCount { get; }
    public int MaxBatchCount { get; }
    public float BatchSpacing { get; }
    public float LifeTime { get; }
    public CellType RoadCellType { get; }

    public DynamicObstaclesSpawnerConfig(
        Vector3Int position,
        GameObject spawnerControllerPrefab,
        IReadOnlyList<GameObject> dynamicObstaclePrefabs,
        float moveSpeed,
        bool moveRight,
        float baseSpawnInterval,
        float spawnIntervalJitter,
        int minBatchCount,
        int maxBatchCount,
        float batchSpacing,
        float lifeTime,
        CellType roadCellType
    ) : base(position, spawnerControllerPrefab, dynamicObstaclePrefabs)
    {
        MoveSpeed = moveSpeed;
        MoveRight = moveRight;
        BaseSpawnInterval = baseSpawnInterval;
        SpawnIntervalJitter = spawnIntervalJitter;
        MinBatchCount = minBatchCount;
        MaxBatchCount = maxBatchCount;
        BatchSpacing = batchSpacing;
        LifeTime = lifeTime;
        RoadCellType = roadCellType;
    }
}
