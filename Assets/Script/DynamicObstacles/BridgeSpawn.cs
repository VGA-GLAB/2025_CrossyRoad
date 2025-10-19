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
        //�C���Q�[�����̂݋��𐶐�����
        //if (!GameManager.instance.IsInGamePlay) return;

        spawnTimer += Time.fixedDeltaTime;
        if(spawnTimer >= spawnTime)
        {
            BridgeGenerate();
        }
    }

    void BridgeGenerate() //���̐���
    {
        // Y ���� 0.45f �����グ�Đ���
        Vector3 spawnPos = transform.position + new Vector3(0f, 0.451f, 0f);

        Instantiate(bridgeObj, spawnPos, Quaternion.identity);    // ���݂̈ʒu�ɋ��𐶐�
        spawnTimer = 0f;
    }
}
