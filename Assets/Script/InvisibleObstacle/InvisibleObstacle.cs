using UnityEngine;

/// <summary>
/// 川の両端などに配置される不可視障害物。
/// プレイヤーが触れると死亡させる。
/// </summary>
public class InvisibleObstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに触れた場合のみ処理
        if (other.CompareTag("Player"))
        {
            // PlayerMove コンポーネントを取得
            var playerMove = other.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                // PlayerMove 側の Kill() を呼び出して死亡処理を委譲
                playerMove.Kill();
            }
        }
    }
}
