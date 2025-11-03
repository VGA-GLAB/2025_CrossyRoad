using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 橋スポナーの設定データ。
/// StageData に格納され、GridManager が解釈して BridgeSpawner を生成する。
/// </summary>
public sealed class BridgeSpawnerConfig : SpawnerConfigBase
{
    /// <summary>
    /// 橋の生成間隔（秒）。
    /// spawnTime に対応。
    /// </summary>
    public float SpawnInterval { get; }

    /// <summary>
    /// 1 レーン内で橋を並べる間隔（秒）。
    /// bridgeInterval に対応。
    /// </summary>
    public float BridgeInterval { get; }

    /// <summary>
    /// 1 レーンに生成する橋の数。
    /// bridgeNum に対応。
    /// </summary>
    public int BridgeCountPerLane { get; }

    public bool MoveRight { get; }

    /// <summary>
    /// コンストラクタ。
    /// Config は不変オブジェクトとして扱うため、すべての値をコンストラクタで設定する。
    /// </summary>
    public BridgeSpawnerConfig(
        Vector3Int position,
        GameObject spawnerControllerPrefab,
        IReadOnlyList<GameObject> spawnTargetPrefabs,
        float spawnInterval,
        float bridgeInterval,
        int bridgeCountPerLane,
        bool moveRight
    ) : base(position, spawnerControllerPrefab, spawnTargetPrefabs)
    {
        SpawnInterval = spawnInterval;
        BridgeInterval = bridgeInterval;
        BridgeCountPerLane = bridgeCountPerLane;
        MoveRight = moveRight;
    }
}
