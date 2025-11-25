using UnityEngine;

/// <summary>
/// 動的障害物（例：車など）にアタッチするコンポーネント。
/// スポナーから初期化パラメータを受け取り、以降は自立して一方向に移動する。
/// Transform.Translate で一方向に移動する。
/// </summary>
public class DynamicObstacleMover : MonoBehaviour
{
    // アニメーター（必要に応じて使用）
    [SerializeField] private Animator animator;

    // スポナー側から受け取る初期値設定
    private float moveSpeed;     // 移動速度
    private Vector3 moveDir;     // 移動方向（右 or 左）

    private bool initialized = false;

    /// <summary>
    /// スポナーから呼ばれる初期化メソッド。
    /// </summary>
    public void Initialize(float speed, bool moveRight)
    {
        moveSpeed = speed;
        moveDir = moveRight ? Vector3.right : Vector3.left;
        initialized = true;

        // モデル反転処理
        Vector3 localScale = transform.localScale;
        if (moveRight)
        {
            // 右向き → 正常スケール
            localScale.x = Mathf.Abs(localScale.x);
        }
        else
        {
            // 左向き → X軸反転
            localScale.x = -Mathf.Abs(localScale.x);
        }

        transform.localScale = localScale;


        // Animator があれば走行アニメを再生
        // (Animator で自動でループ再生されるため現状、プログラム制御不要)
        //animator = GetComponent<Animator>();
        //if (animator != null)
        //{
        //    // ToDo: 再生するアニメーションを設定
        //    // animator.SetBool("IsMoving", true);
        //}
    }

    void Update()
    {
        if (!initialized) return;

        // 毎フレーム一定速度で移動
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }
}
