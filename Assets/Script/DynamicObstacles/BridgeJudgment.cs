using UnityEngine;

public class BridgeJudgment : MonoBehaviour
{
    //橋に乗ったら、プレイヤーを橋の子オブジェクトにしプレイヤーが動けるようにする
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(transform);
            Debug.Log("橋を乗る");
        }
    }

    //橋から降りたら、プレイヤーが橋の子オブジェクトから解除する
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
            Debug.Log("橋を降りる");
        }
    }
}
