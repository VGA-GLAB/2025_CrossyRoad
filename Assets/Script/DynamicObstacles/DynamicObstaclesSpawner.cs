using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 動的障害物（例：車など）を一定間隔＋揺らぎでスポーンするクラス。
/// </summary>
public class DynamicObstaclesSpawner : MonoBehaviour
{
    // 基底Configから渡される生成対象Prefab群
    private IReadOnlyList<GameObject> spawnTargetPrefabs;

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
    
    [Header("オブジェクトタイプ")]
    [SerializeField] private ObjectType objectType;

    // 次回スポーン予定時刻（絶対時間）
    private float nextSpawnTime;

    /// <summary>
    /// 初期化メソッド
    /// ScriptableObject → RuntimeConfig で設定された値を反映する
    /// メンバー変数を初期化する
    /// </summary>
    // GridManager から呼ばれる
    public void Initialize(DynamicObstaclesSpawnerConfig config)
    {
        // ScriptableObject → RuntimeConfig で設定された値を反映
        spawnTargetPrefabs = config.SpawnTargetPrefabs;
        moveSpeed = config.MoveSpeed;
        moveRight = config.MoveRight;
        baseSpawnInterval = config.BaseSpawnInterval;
        spawnIntervalJitter = config.SpawnIntervalJitter;
        minBatchCount = config.MinBatchCount;
        maxBatchCount = config.MaxBatchCount;
        batchSpacing = config.BatchSpacing;
        lifeTime = config.LifeTime;
        
        objectType = config.ObjectType;

        // 初回スポーンタイマーをリセット
        SetNextSpawnTime();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 最初のスポーンまでの時間を決定
        SetNextSpawnTime();
    }

    // Update is called once per frame
    void Update()
    {
        //インゲーム中のみ動的障害物を生成する
        if (GameManager.instance != null && !GameManager.instance.IsInGamePlay) return;
        
        // 予定時刻(絶対時間)が過ぎたらスポーン
        if (Time.time >= nextSpawnTime)
        {
            SpawnBatch();
            SetNextSpawnTime();
        }
    }

    /// <summary>
    /// 次回のスポーン時刻を計算して設定する。
    /// - 基本間隔(baseSpawnInterval)にランダムな揺らぎ(spawnIntervalJitter)を加える
    /// - その値を BaseInterval の整数倍に丸めてリズムを揃える
    /// - 前回の nextSpawnTime から加算することで、必ず現在時刻より後に設定される
    ///   （これにより「同じフレームで連続スポーンする」問題を防ぐ）
    /// </summary>
    private void SetNextSpawnTime()
    {
        // ランダムに間隔を決定
        float raw = Random.Range(baseSpawnInterval - spawnIntervalJitter,
                             baseSpawnInterval + spawnIntervalJitter);

        // BaseInterval の整数倍に丸める
        float roundedInterval = Mathf.Max(
            SpawnerConstants.BaseInterval,
            Mathf.Round(raw / SpawnerConstants.BaseInterval) * SpawnerConstants.BaseInterval
        );

        // 前回の nextSpawnTime を基準に加算
        //  - もし nextSpawnTime が過去にある場合は「現在時刻 + interval」
        //  - まだ未来に残っている場合は「前回予定 + interval」
        //    こうすることで必ず「現在より後の時刻」に設定される
        if (nextSpawnTime < Time.time)
        {
            nextSpawnTime = Time.time + roundedInterval;
        }
        else
        {
            nextSpawnTime += roundedInterval;
        }
    }

    /// <summary>
    /// 編隊（バッチ）単位で障害物を生成する。
    /// </summary>
    private void SpawnBatch()
    {
        // 未設定時はリターン
        if (spawnTargetPrefabs == null || spawnTargetPrefabs.Count == 0)
            return;

        // 今回の編隊の台数を決定
        int batchCount = Random.Range(minBatchCount, maxBatchCount + 1);

        for (int i = 0; i < batchCount; i++)
        {
            // Prefabをランダムに選択
            GameObject prefab = spawnTargetPrefabs[Random.Range(0, spawnTargetPrefabs.Count)];

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

            // ToDo: 可能であれば位置ハブはプレハブのつくりで吸収し、
            // Y軸のオフセット補正処理はおこなわないようにしたい

            // Y 軸に 1.00f 持ち上げて生成
            spawnPos += new Vector3(0f, 1.00f, 0f);

            // インスタンス生成
            GameObject instance = Instantiate(prefab, spawnPos, Quaternion.identity);

            // 初期化パラメータを渡す
            DynamicObstacleMover mover = instance.GetComponent<DynamicObstacleMover>();
            if (mover != null)
            {
                mover.Initialize(moveSpeed, moveRight, objectType);
            }

            // 一定時間後に自動破棄
            Destroy(instance, lifeTime);
        }
    }

}
