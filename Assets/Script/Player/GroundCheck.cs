using System;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Action OnPlayerRiverDie;
    [SerializeField] private float rayLength = 0.3f; 
    [SerializeField] private LayerMask riverLayer;   // ���Layer
    [SerializeField] private PlayerMove _playerMove;
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.blue);
        // ��̏���`�F�b�N
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit riverHit, rayLength, riverLayer))
        {
            Debug.Log("��ɗ������I");
            OnPlayerRiverDie?.Invoke();
        }
    }
}
