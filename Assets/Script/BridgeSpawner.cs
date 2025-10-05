using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BridgeSpawner : MonoBehaviour
{
    [Header("橋のPrefab")]
    [SerializeField] List<GameObject> bridges;

    [Header("橋の生成間隔")]
    [SerializeField] float spawnTime;
    private float spawnTimer;
    [Header("生成する個数")]
    [SerializeField] int spawnCount;
    [Header("生成場所")]
    [SerializeField] Transform spawnParent;

    private ObjectPool<GameObject> bridgePool;


    void Awake()
    {
        bridgePool = new ObjectPool<GameObject>(
            CreateBridge, GetBridge, ReleaseBridge, DestroyBridge, false, spawnCount, spawnCount);
    }

    void FixedUpdate()
    {
        spawnTimer += Time.fixedDeltaTime;
        if(spawnTimer > spawnTime)
        {
            bridgePool.Get();
            spawnTimer = 0;
        }
    }

    GameObject CreateBridge()
    {
        //橋の種類をランダムに取得
        var random = Random.Range(0, bridges.Count);
        GameObject prefab = bridges[random];

        //生成
        GameObject bridge = Instantiate(prefab);
        bridge.SetActive(false);
        bridge.transform.SetParent(spawnParent);
        bridge.GetComponent<MovingBridge>().bridgeSpawner = this;
        return bridge;
    }

    void GetBridge(GameObject obj)
    {
        //初期位置に配置
        obj.transform.position = new Vector3(0f, 0f, 0f);

        obj.SetActive(true);
    }

    void ReleaseBridge(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void Release(GameObject obj)
    {
        bridgePool.Release(obj);
    }

    void DestroyBridge(GameObject obj)
    {
        Destroy(obj.gameObject);
    }
}
