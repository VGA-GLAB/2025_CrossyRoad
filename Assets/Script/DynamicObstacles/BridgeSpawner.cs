using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BridgeSpawner : MonoBehaviour
{
    //使用しない

    [Header("GridManagerの参照")]
    [SerializeField] private GridManager gridManager;
    
    [Header("橋のPrefab")]
    [SerializeField] private List<GameObject> bridges;
    [Header("橋の生成間隔")]
    [SerializeField] private float spawnTime;
    private float spawnTimer;
    [Header("レーンに生成する橋の間隔")]
    [SerializeField] private float bridgeInterval;
    private float bridgeIntervalTimer;
    [Header("１レーンに生成する橋の数")]
    [SerializeField] private int bridgeNum;
    private int currentBridgeCount;
    [Header("生成した橋の親")]
    [SerializeField] private Transform spawnParent;

    private StageGenerationTestDriver stageGenerationTestDriver;

    private ObjectPool<GameObject> bridgePool;
    List<int> riverCount = new List<int>();


    void Awake()
    {
        bridgePool = new ObjectPool<GameObject>(
            CreateBridge, GetBridge, ReleaseBridge, DestroyBridge);
    }

    void Start()
    {
        stageGenerationTestDriver = new StageGenerationTestDriver();
        stageGenerationTestDriver.GenerateTestStage();
        currentBridgeCount = bridgeNum;
    }

    void FixedUpdate()
    { 
        if (!GameManager.instance.isInGamePlay) return;

        spawnTimer += Time.fixedDeltaTime;
        if(spawnTimer > spawnTime)
        {
            if(currentBridgeCount > 0) //１レーンに生成可能な橋の数
            {
                bridgeIntervalTimer += Time.fixedDeltaTime;
                if(bridgeIntervalTimer > bridgeInterval)
                {
                    //ステージ情報を取得して、川レーンの取得
                    GetStageData();

                    //橋を生成する
                    foreach (int river in riverCount)
                    {
                        //生成した橋の位置を設定
                        GameObject bridge = bridgePool.Get();
                        if (bridge != null)
                        {
                            //ToDo:一方方向だけでなく両方から橋を流せるようにする

                            bridge.transform.position = new Vector3(0f, 1f, river);
                            bridge.SetActive(true);
                        }
                    }
                    currentBridgeCount--;
                    bridgeIntervalTimer = 0f;
                }
            }

            //１レーンに生成可能な橋を生成終了
            if (currentBridgeCount == 0)
            {
                currentBridgeCount = bridgeNum;
                spawnTimer = 0f;
                bridgeIntervalTimer = 0f;
            }
        }
    }

    void GetStageData() //ステージデータ取得
    {
        var data = stageGenerationTestDriver.data;
        riverCount.Clear();

        //川レーンを取得
        for (int i = 0; i < data.width; i++)
        {
            if (data.laneTypes[i] == CellType.River)        // 名前空間の修正
            {
                riverCount.Add(i);
            }
        }
    }

    GameObject CreateBridge()
    {
        //橋の種類をランダムに取得
        var random = Random.Range(0, bridges.Count);
        GameObject prefab = bridges[random];

        //橋を生成する
        GameObject bridge = Instantiate(prefab);
        bridge.SetActive(false);
        bridge.transform.SetParent(spawnParent);
        bridge.GetComponent<MovingBridge>().bridgeSpawner = this;
        return bridge;
    }

    void GetBridge(GameObject obj)
    {
        if (obj == null) return;

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
