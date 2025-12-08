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

    // マップ左右端の座標
    private float mapLeftBoundaryX;
    private float mapRightBoundaryX;


    /// <summary>
    /// スポナーから呼ばれる初期化メソッド。
    /// </summary>
    public void Initialize(bool moveRight, float mapLeftBoundaryX, float mapRightBoundaryX)
    {
        // 進行方向の左右を保持
        moveDir = moveRight ? Vector3.right : Vector3.left;

        // マップ左右端の座標を保持
        this.mapLeftBoundaryX = mapLeftBoundaryX;
        this.mapRightBoundaryX = mapRightBoundaryX;
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

        // マップ範囲外に移動したら、橋を消す
        if (transform.position.x < mapLeftBoundaryX || transform.position.x > mapRightBoundaryX)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // 子にプレイヤーがいる場合は親子関係を解除
        var player = GetComponentInChildren<PlayerMove>();
        if (player != null)
        {
            player.transform.SetParent(null);
        }
    }
}
