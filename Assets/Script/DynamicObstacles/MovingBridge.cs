using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("ìÆÇ≠ë¨Ç≥")]
    [SerializeField] private float moveSpeed;

    [Header("è¡ñ≈éûä‘")]
    [SerializeField] private float destoryTime;
    private float destoryTimer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
//        rb.linearVelocity = new Vector3(moveSpeed, 0f, 0f);

        //è¡ñ≈éûä‘Ç™åoÇ¡ÇΩÇÁÅAã¥Çè¡Ç∑
        destoryTimer += Time.fixedDeltaTime;
        if(destoryTimer >= destoryTime)
        {
            Destroy(this.gameObject);
        }
    }
}
