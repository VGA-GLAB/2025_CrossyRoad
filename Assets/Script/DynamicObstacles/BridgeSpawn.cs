using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [Header("��")]
    [SerializeField] private GameObject bridgeObj;

    [Header("���̐����Ԋu")]
    [SerializeField] private float spawnTime;
    private float spawnTimer;

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
        Instantiate(bridgeObj);
        spawnTimer = 0f;
    }
}
