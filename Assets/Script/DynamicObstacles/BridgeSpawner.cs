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
    [Header("生成した橋の親")]
    [SerializeField] Transform spawnParent;

    private ObjectPool<GameObject> bridgePool;

    //ToDo:GridManagerから川を取得して、そこに橋を生成できるようにする
    //ToDo:GameStateがInGameStateの時のみ、生成するようにする


    void Awake()
    {
        bridgePool = new ObjectPool<GameObject>(
            CreateBridge, GetBridge, ReleaseBridge, DestroyBridge);
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isInGamePlay) return;

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
        //初期位置に配置　仮
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
