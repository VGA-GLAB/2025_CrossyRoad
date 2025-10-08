using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BridgeSpawner : MonoBehaviour
{
    [Header("����Prefab")]
    [SerializeField] List<GameObject> bridges;

    [Header("���̐����Ԋu")]
    [SerializeField] float spawnTime;
    private float spawnTimer;
    [Header("�����������̐e")]
    [SerializeField] Transform spawnParent;

    private ObjectPool<GameObject> bridgePool;

    //ToDo:GridManager�������擾���āA�����ɋ��𐶐��ł���悤�ɂ���
    //ToDo:GameState��InGameState�̎��̂݁A��������悤�ɂ���


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
        //���̎�ނ������_���Ɏ擾
        var random = Random.Range(0, bridges.Count);
        GameObject prefab = bridges[random];

        //����
        GameObject bridge = Instantiate(prefab);
        bridge.SetActive(false);
        bridge.transform.SetParent(spawnParent);
        bridge.GetComponent<MovingBridge>().bridgeSpawner = this;
        return bridge;
    }

    void GetBridge(GameObject obj)
    {
        //�����ʒu�ɔz�u�@��
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
