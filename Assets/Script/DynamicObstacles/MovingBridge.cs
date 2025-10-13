using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("BridgeSpawner")]
    public BridgeSpawner bridgeSpawner;

    [Header("動く速さ")]
    [SerializeField] private float moveSpeed;

    [Header("消滅時間")]
    [SerializeField] private float destoryTime;
    private float destoryTimer;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveSpeed, 0f, 0f);

        //消滅時間が経ったら、橋を消す
        destoryTimer += Time.fixedDeltaTime;
        if(destoryTimer >= destoryTime)
        {
            Destroy(this.gameObject);
        }
    }

    //浮き沈みするようにする
    private void OnCollisionEnter(Collision collision)
    {
        //プレイヤーが橋に乗った時、プレイヤーを橋の子オブジェクトにする
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //プレイヤーを橋の子オブジェクトじゃなくする
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
