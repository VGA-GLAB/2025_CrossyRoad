using UnityEngine;

public class BridgeSpawn : MonoBehaviour
{
    [Header("‹´")]
    [SerializeField] private GameObject bridgeObj;

    [Header("‹´‚Ì¶¬ŠÔŠu")]
    [SerializeField] private float spawnTime;
    private float spawnTimer;

    // GridManager ‚©‚çŒÄ‚Î‚ê‚é‰Šú‰»ƒƒ\ƒbƒh
    public void Initialize(BridgeSpawnerConfig config)
    {
        // Note: bridgeObj‚Ì•¡”w’è‚ğŒ³À‘•‚©‚ç‚Æ‚è‚±‚ñ‚¾‚ ‚Æ‚Í
        // bridges = config.SpawnTargetPrefabs;‚ÉC³—\’è
        if (config.SpawnTargetPrefabs != null && config.SpawnTargetPrefabs.Count > 0)
        {
            // æ“ª‚Ì—v‘f‚ğ bridgeObj ‚Éİ’è
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

    void BridgeGenerate() //‹´‚Ì¶¬
    {
        Instantiate(bridgeObj, transform.position, Quaternion.identity);    // Œ»İ‚ÌˆÊ’u‚É‹´‚ğ¶¬
        spawnTimer = 0f;
    }
}
