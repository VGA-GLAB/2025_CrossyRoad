using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BridgeSpawner : MonoBehaviour
{
    //�g�p���Ȃ�

    [Header("GridManager�̎Q��")]
    [SerializeField] private GridManager gridManager;
    
    [Header("����Prefab")]
    [SerializeField] private List<GameObject> bridges;
    [Header("���̐����Ԋu")]
    [SerializeField] private float spawnTime;
    private float spawnTimer;
    [Header("���[���ɐ������鋴�̊Ԋu")]
    [SerializeField] private float bridgeInterval;
    private float bridgeIntervalTimer;
    [Header("�P���[���ɐ������鋴�̐�")]
    [SerializeField] private int bridgeNum;
    private int currentBridgeCount;
    [Header("�����������̐e")]
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
            if(currentBridgeCount > 0) //�P���[���ɐ����\�ȋ��̐�
            {
                bridgeIntervalTimer += Time.fixedDeltaTime;
                if(bridgeIntervalTimer > bridgeInterval)
                {
                    //�X�e�[�W�����擾���āA�샌�[���̎擾
                    GetStageData();

                    //���𐶐�����
                    foreach (int river in riverCount)
                    {
                        //�����������̈ʒu��ݒ�
                        GameObject bridge = bridgePool.Get();
                        if (bridge != null)
                        {
                            //ToDo:������������łȂ��������狴�𗬂���悤�ɂ���

                            bridge.transform.position = new Vector3(0f, 1f, river);
                            bridge.SetActive(true);
                        }
                    }
                    currentBridgeCount--;
                    bridgeIntervalTimer = 0f;
                }
            }

            //�P���[���ɐ����\�ȋ��𐶐��I��
            if (currentBridgeCount == 0)
            {
                currentBridgeCount = bridgeNum;
                spawnTimer = 0f;
                bridgeIntervalTimer = 0f;
            }
        }
    }

    void GetStageData() //�X�e�[�W�f�[�^�擾
    {
        var data = stageGenerationTestDriver.data;
        riverCount.Clear();

        //�샌�[�����擾
        for (int i = 0; i < data.width; i++)
        {
            if (data.laneTypes[i] == CellType.River)        // ���O��Ԃ̏C��
            {
                riverCount.Add(i);
            }
        }
    }

    GameObject CreateBridge()
    {
        //���̎�ނ������_���Ɏ擾
        var random = Random.Range(0, bridges.Count);
        GameObject prefab = bridges[random];

        //���𐶐�����
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
