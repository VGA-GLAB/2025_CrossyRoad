using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [Header("ã¥")]
    [SerializeField] private GameObject bridgeObj;

    [Header("ã¥ÇÃê∂ê¨ä‘äu")]
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

    void BridgeGenerate() //ã¥ÇÃê∂ê¨
    {
        Instantiate(bridgeObj);
        spawnTimer = 0f;
    }
}
