using UnityEngine;

public class MovingBridge : MonoBehaviour
{
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
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
//        rb.linearVelocity = new Vector3(moveSpeed, 0f, 0f);

        //���Ŏ��Ԃ��o������A��������
        destoryTimer += Time.fixedDeltaTime;
        if(destoryTimer >= destoryTime)
        {
            Destroy(this.gameObject);
        }
    }
}
