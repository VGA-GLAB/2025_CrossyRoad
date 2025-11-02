using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動的障害物スポナーの設定データを保持する ScriptableObject。
/// </summary>
[CreateAssetMenu(
    fileName = "DynamicObstaclesSpawnerConfigSO_Default",
    menuName = "Stage/DynamicObstaclesSpawnerConfig",
    order = 0)]
public class DynamicObstaclesSpawnerConfigSO : ScriptableObject
{
    [Header("スポナー本体のPrefab")]
    public GameObject spawnerControllerPrefab;

    [Header("スポーン対象の障害物のPrefabリスト")]
    public List<GameObject> dynamicObstaclePrefabs;

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

    /// <summary>
    /// ScriptableObject に保存された値をもとに、
    /// 実行時専用の <see cref="DynamicObstaclesSpawnerConfig"/> を生成する。
    /// 
    /// - ステージ自動生成コードはこのメソッドを通じて Config を取得する。
    /// - Config は読み取り専用の不変オブジェクトとして扱われるため、
    ///   実行中に値が書き換わることを防げる。
    /// - また、この変換処理は「DynamicObstaclesSpawnerConfig と SO の項目が一致しているか」
    ///   をコンパイル時にチェックする役割も兼ねている。
    ///   （項目が増減した場合、ここでコンパイルエラーとなり気付ける）
    /// </summary>
    public DynamicObstaclesSpawnerConfig ToRuntimeConfig()
    {
        return new DynamicObstaclesSpawnerConfig(
            Vector3Int.zero,            // Positionは後でStageGeneration側で設定
            spawnerControllerPrefab,
            dynamicObstaclePrefabs,
            moveSpeed,
            moveRight,
            baseSpawnInterval,
            spawnIntervalJitter,
            minBatchCount,
            maxBatchCount,
            batchSpacing,
            lifeTime
        );
    }
}
