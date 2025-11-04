using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("動く速さ")]
    [SerializeField] private float moveSpeed;

    [Header("消滅時間")]
    [SerializeField] private float destoryTime;
    private float destoryTimer;

    // 進行方向のベクトル
    private Vector3 moveDir;

    private Rigidbody rb;

    /// <summary>
    /// スポナーから呼ばれる初期化メソッド。
    /// </summary>
    public void Initialize(bool moveRight)
    {
        moveDir = moveRight ? Vector3.right : Vector3.left;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {
        transform.position += moveDir  * moveSpeed * Time.deltaTime;
//        rb.linearVelocity = new Vector3(moveSpeed, 0f, 0f);

        //消滅時間が経ったら、橋を消す
        destoryTimer += Time.fixedDeltaTime;
        if(destoryTimer >= destoryTime)
        {
            Destroy(this.gameObject);
        }
    }
}
