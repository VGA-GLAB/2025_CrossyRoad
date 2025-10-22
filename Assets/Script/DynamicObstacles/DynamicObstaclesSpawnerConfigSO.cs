using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動的障害物スポナーの設定データを保持する ScriptableObject。
/// </summary>
[CreateAssetMenu(
    fileName = "DynamicObstaclesSpawnerConfigSO_Default",
    menuName = "Spawner/DynamicObstaclesSpawnerConfig",
    order = 0)]
public class DynamicObstaclesSpawnerConfigSO : ScriptableObject
{
    [Header("スポーン対象の障害物のPrefabリスト")]
    public List<GameObject> dynamicObstacles;

    [Header("スポーン対象の障害物の移動速度（レーン単位で固定）")]
    public float moveSpeed = 10.0f;
    public bool moveRight = true;

    [Header("スポーン間隔設定")]
    public float baseSpawnInterval = 3.0f;
    public float spawnIntervalJitter = 0.5f;

    [Header("編隊設定")]
    public int minBatchCount = 1;
    public int maxBatchCount = 1;
    public float batchSpacing = 1.5f;

    [Header("Destroyするまでの時間")]
    public float lifeTime = 12.0f;
}
