using UnityEngine;

public class BridgeJudgment : MonoBehaviour
{
    //���ɏ������A�v���C���[�����̎q�I�u�W�F�N�g�ɂ��v���C���[��������悤�ɂ���
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(transform);
            Debug.Log("�������");
        }
    }

    //������~�肽��A�v���C���[�����̎q�I�u�W�F�N�g�����������
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.SetParent(null);
            Debug.Log("�����~���");
        }
    }
}
