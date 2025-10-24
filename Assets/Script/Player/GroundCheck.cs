using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Action OnPlayerRiverDie;
    [SerializeField] private float rayLength = 0.3f; 
    [SerializeField] private LayerMask riverLayer;   // 川のLayer
    [SerializeField] private PlayerMove _playerMove;
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.blue);
        // 川の上をチェック
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit riverHit, rayLength, riverLayer))
        {
            Debug.Log("川に落ちた！");
            OnPlayerRiverDie?.Invoke();
        }
    }
}
