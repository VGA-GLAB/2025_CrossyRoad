using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("BridgeSpawner")]
    public BridgeSpawner bridgeSpawner;

    [Header("ìÆÇ≠ë¨Ç≥")]
    [SerializeField] float moveSpeed;

    [Header("è¡ñ≈éûä‘")]
    [SerializeField] float destoryTime;
    private float destoryTimer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveSpeed, 0f, 0f);

        destoryTimer += Time.fixedDeltaTime;
        if(destoryTimer >= destoryTime)
        {
            Destroy(this.gameObject);
        }
    }

    void OnBecameInvisible()
    {
        //bridgeSpawner.Release(this.gameObject);
    }


    //ïÇÇ´íæÇ›Ç∑ÇÈÇÊÇ§Ç…Ç∑ÇÈ
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

        }
    }
}
