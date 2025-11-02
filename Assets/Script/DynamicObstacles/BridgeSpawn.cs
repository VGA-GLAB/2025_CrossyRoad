using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [Header("橋")]
    [SerializeField] private GameObject bridgeObj;

    [Header("橋の生成間隔")]
    [SerializeField] private float spawnIntervalSeconds;

    // 次回スポーン予定時刻（絶対時間）
    private float nextSpawnTime;

    /// <summary>
    /// 初期化メソッド
    /// ・BridgeSpawnerConfigの反映
    /// ・初回スポーンをスケジューリング
    /// </summary>
    public void Initialize(BridgeSpawnerConfig config)
    {

        // BridgeSpawnerConfig 値の反映
        spawnIntervalSeconds = config.SpawnInterval;

        // Note: bridgeObjの複数指定を元実装からとりこんだあとは
        // bridges = config.SpawnTargetPrefabs;に修正予定
        if (config.SpawnTargetPrefabs != null && config.SpawnTargetPrefabs.Count > 0)
        {
            // 先頭の要素を bridgeObj に設定
            bridgeObj = config.SpawnTargetPrefabs[0];
        }

        // 初回スポーンをスケジューリング
        SetNextSpawnTime();
    }

    void Update()
    {
        //インゲーム中のみ橋を生成する
        if (GameManager.instance != null &&!GameManager.instance.IsInGamePlay) return;

        // 予定時刻(絶対時間)が過ぎたらスポーン
        if (Time.time >= nextSpawnTime)
        {
            BridgeGenerate();
        }
    }

    /// <summary>
    /// 外部設定値 spawnIntervalSeconds（秒）を基準に、
    /// BaseInterval の整数倍に丸めた上でグローバル時間にスナップし、
    /// 次回スポーン予定時刻（絶対時間）を nextSpawnTime に設定する。
    /// </summary>
    private void SetNextSpawnTime()
    {
        // BaseInterval の整数倍に丸める
        float roundedInterval = Mathf.Max(
            SpawnerConstants.BaseInterval,
            Mathf.Round(spawnIntervalSeconds / SpawnerConstants.BaseInterval) * SpawnerConstants.BaseInterval
        );

        // グローバル時間にスナップ
        float now = Time.time;
        nextSpawnTime = Mathf.Ceil(now / roundedInterval) * roundedInterval;
    }

    /// <summary>
    /// 橋を生成する処理。
    /// ・bridgeObj プレハブをインスタンス化する。
    /// ・生成後は次回スポーン予定時刻を再計算してスケジューリングする。
    /// </summary>
    void BridgeGenerate() //橋の生成
    {
        // ToDo: 可能であれば位置ハブはプレハブのつくりで吸収し、
        // Y軸のオフセット補正処理はおこなわないようにしたい

        // Y 軸に 0.45f 持ち上げて生成
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.451f, 0f);

        Instantiate(bridgeObj, spawnPos, Quaternion.identity);    // 現在の位置に橋を生成

        // 次回のスポーン予定時刻を設定
        SetNextSpawnTime();
    }
}
