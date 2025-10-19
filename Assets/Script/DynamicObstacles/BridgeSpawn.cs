using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [Header("橋")]
    [SerializeField] private GameObject bridgeObj;

    [Header("橋の生成間隔")]
    [SerializeField] private float spawnTime;
    private float spawnTimer;

    // GridManager から呼ばれる初期化メソッド
    public void Initialize(BridgeSpawnerConfig config)
    {
        // Note: bridgeObjの複数指定を元実装からとりこんだあとは
        // bridges = config.SpawnTargetPrefabs;に修正予定
        if (config.SpawnTargetPrefabs != null && config.SpawnTargetPrefabs.Count > 0)
        {
            // 先頭の要素を bridgeObj に設定
            bridgeObj = config.SpawnTargetPrefabs[0];
        }
        this.spawnTime = config.SpawnInterval;
    }

    void FixedUpdate()
    {
        //インゲーム中のみ橋を生成する
        //if (!GameManager.instance.IsInGamePlay) return;

        spawnTimer += Time.fixedDeltaTime;
        if(spawnTimer >= spawnTime)
        {
            BridgeGenerate();
        }
    }

    void BridgeGenerate() //橋の生成
    {
        // Y 軸に 0.45f 持ち上げて生成
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.451f, 0f);

        Instantiate(bridgeObj, spawnPos, Quaternion.identity);    // 現在の位置に橋を生成
        spawnTimer = 0f;
    }
}
