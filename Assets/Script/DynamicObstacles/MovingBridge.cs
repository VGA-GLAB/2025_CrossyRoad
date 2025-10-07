using UnityEngine;

public class MovingBridge : MonoBehaviour
{
    [Header("BridgeSpawner")]
    public BridgeSpawner bridgeSpawner;

    [Header("“®‚­‘¬‚³")]
    [SerializeField] float moveSpeed;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(0f, 0f, moveSpeed);
    }

    void OnBecameInvisible()
    {
        bridgeSpawner.Release(this.gameObject);
    }
}
