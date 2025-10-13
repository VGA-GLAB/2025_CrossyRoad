using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("BridgeSpawner")]
    public BridgeSpawner bridgeSpawner;

    [Header("��������")]
    [SerializeField] private float moveSpeed;

    [Header("���Ŏ���")]
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

        //���Ŏ��Ԃ��o������A��������
        destoryTimer += Time.fixedDeltaTime;
        if(destoryTimer >= destoryTime)
        {
            Destroy(this.gameObject);
        }
    }

    //�������݂���悤�ɂ���
    private void OnCollisionEnter(Collision collision)
    {
        //�v���C���[�����ɏ�������A�v���C���[�����̎q�I�u�W�F�N�g�ɂ���
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //�v���C���[�����̎q�I�u�W�F�N�g����Ȃ�����
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
