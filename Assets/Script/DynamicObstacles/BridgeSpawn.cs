using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [Header("��")]
    [SerializeField] private GameObject bridgeObj;

    [Header("���̐����Ԋu")]
    [SerializeField] private float spawnTime;
    private float spawnTimer;

    // GridManager ����Ă΂�鏉�������\�b�h
    public void Initialize(BridgeSpawnerConfig config)
    {
        // Note: bridgeObj�̕����w�������������Ƃ肱�񂾂��Ƃ�
        // bridges = config.SpawnTargetPrefabs;�ɏC���\��
        if (config.SpawnTargetPrefabs != null && config.SpawnTargetPrefabs.Count > 0)
        {
            // �擪�̗v�f�� bridgeObj �ɐݒ�
            bridgeObj = config.SpawnTargetPrefabs[0];
        }
        this.spawnTime = config.SpawnInterval;
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isInGamePlay) return;

        spawnTimer += Time.fixedDeltaTime;
        if(spawnTimer >= spawnTime)
        {
            BridgeGenerate();
        }
    }

    void BridgeGenerate() //���̐���
    {
        Instantiate(bridgeObj, transform.position, Quaternion.identity);    // ���݂̈ʒu�ɋ��𐶐�
        spawnTimer = 0f;
    }
}
