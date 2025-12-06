using System;
using UnityEngine;

/// <summary>
/// 動的障害物（敵ロボットなど）にアタッチするコンポーネント。
/// スポナーから初期化パラメータを受け取り、その後は自動的に移動を開始する。
/// Transform.Translate によって移動を行う。
/// </summary>
public class DynamicObstacleMover : MonoBehaviour
{
    // アニメーター（必要に応じて使用）
    [SerializeField] private Animator animator;

    // スポナーから渡される初期化用の値
    private float moveSpeed;     // 移動速度
    private Vector3 moveDir;     // 移動方向（右 or 左）

    private bool initialized = false;

    private ObjectType objectType; // 自身のオブジェクトタイプを保持
    public ObjectType ObjectType => objectType;
    private bool isSound; // SE再生済みかどうか

    /// <summary>
    /// スポナーから呼ばれる初期化メソッド。
    /// </summary>
    public void Initialize(float speed, bool moveRight, ObjectType type)
    {
        moveSpeed = speed;
        moveDir = moveRight ? Vector3.right : Vector3.left;
        initialized = true;
        objectType = type;

        // 見た目の反転処理
        if (moveRight)
        {
            // 右向き → 回転なし
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // 左向き → 回転で反転
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Animator の初期化（必要なら使用）
        // (Animator で移動ループを再生するための準備。プログラム側で制御する想定)
        //animator = GetComponent<Animator>();
        //if (animator != null)
        //{
        //    // ToDo: 移動用アニメーションを設定
        //    // animator.SetBool("IsMoving", true);
        //}
    }

    void Update()
    {
        if (!initialized) return;

        // 毎フレーム移動速度に応じて移動
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        SoundType();
    }

    /// <summary>
    /// オブジェクトタイプに応じてSEを再生する
    /// </summary>
    private void SoundType()
    {
        if (isSound) //サウンド再生中
        {
            //範囲外になったら、SEを停止する
            if (SoundManager.instance.PlayerDistance(this.gameObject))
            {
                SoundManager.instance.ObstacleStopSE(this.gameObject);
                //CuePlay.instance.ObstacleStopSE(this.gameObject);
            }
            return;
        }

        switch (objectType)
        {
            case ObjectType.EnemyRobot:
                SoundManager.instance.ObstaclePlaySE("敵ロボットの移動", this.gameObject);
                //CuePlay.instance.ObstaclePlaySE("robott", this.gameObject);
                Debug.Log("ロボット再生");
                isSound = true;
                break;

            case ObjectType.Saw:
                SoundManager.instance.ObstaclePlaySE("ノコギリ移動", this.gameObject);
                //CuePlay.instance.ObstaclePlaySE("SE_Saw_Move", this.gameObject);
                isSound = true;
                break;
        }
    }
    
    /*　画面の比率を9:16　スマホみたくした結果、直ぐに画面外に出てSEがならないため使用を辞める
    private void OnDestroy()
    {
        SoundManager.instance.ObstacleStopSE(this.gameObject);
        //CuePlay.instance.ObstacleStopSE(this.gameObject);
    }
    */
}
