using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動的障害物（例：車など）を一定間隔＋揺らぎでスポーンするクラス。
/// </summary>
public class DynamicObstaclesSpawner : MonoBehaviour
{

    [Header("スポーン対象の障害物のPrefabリスト")]
    [SerializeField] private List<GameObject> dynamicObstacles;                 // 複数の車種などを登録しておき、ランダムに選択してスポーンする

    [Header("スポーン対象の障害物の移動速度（レーン単位で固定）")]
    [SerializeField] private float moveSpeed                    = 10.0f;
    [SerializeField] public bool moveRight                      = true;         // trueなら右方向、falseなら左方向に進む

    [Header("スポーン間隔設定")]
    [SerializeField] public float baseSpawnInterval             = 3.0f;         // 基本の生成間隔
    [SerializeField] public float spawnIntervalJitter           = 0.5f;         // 間隔の揺らぎ幅（±）

    [Header("編隊設定")]
    [SerializeField] public int minBatchCount                   = 1;            // 一度に出す最小台数
    [SerializeField] public int maxBatchCount                   = 1;            // 一度に出す最大台数
    [SerializeField] public float batchSpacing                  = 1.5f;         // 同一バッチ内の車間距離

    [Header("Destroyするまでの時間")]
    [SerializeField] private float lifeTime                     = 12.0f;        // 生成後に自動破棄するまでの時間

    // 次回スポーンまでの残り時間
    private float spawnTimer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 最初のスポーンまでの時間を決定
        spawnTimer = GetNextSpawnInterval();
    }

    // Update is called once per frame
    void Update()
    {
        // 経過時間を減算
        spawnTimer -= Time.deltaTime;

        // タイマーが0以下になったらスポーン処理
        if (spawnTimer <= 0f)
        {
            SpawnBatch();
            // 次回スポーンまでの時間を再設定
            spawnTimer = GetNextSpawnInterval();
        }
    }

    /// <summary>
    /// 次回スポーンまでの時間を決定する。
    /// baseSpawnInterval ± spawnIntervalJitter の範囲でランダムに決定。
    /// </summary>
    private float GetNextSpawnInterval()
    {
        return Random.Range(baseSpawnInterval - spawnIntervalJitter,
                            baseSpawnInterval + spawnIntervalJitter);
    }

    /// <summary>
    /// 編隊（バッチ）単位で障害物を生成する。
    /// </summary>
    private void SpawnBatch()
    {
        // 今回の編隊の台数を決定
        int batchCount = Random.Range(minBatchCount, maxBatchCount + 1);

        for (int i = 0; i < batchCount; i++)
        {
            // プレハブが未登録の場合は何もしない
            if (dynamicObstacles == null || dynamicObstacles.Count == 0)
            {
                return;
            }

            // Prefabをランダムに選択
            GameObject prefab = dynamicObstacles[Random.Range(0, dynamicObstacles.Count)];

            // スポーン位置を決定
            Vector3 spawnPos = transform.position;
            if (moveRight)
            {
                spawnPos.x -= i * batchSpacing;     // 右向きなら左側に並べる
            }
            else
            {
                spawnPos.x += i * batchSpacing;     // 左向きなら右側に並べる
            }

            // インスタンス生成
            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);

            // 初期化パラメータを渡す
            DynamicObstacleMover mover = instance.GetComponent<DynamicObstacleMover>();
            if (mover != null)
            {
                mover.Initialize(moveSpeed, moveRight);
            }

            // 一定時間後に自動破棄
            Destroy(instance, lifeTime);
        }
    }

}
