using UnityEngine;

/// <summary>
/// ���I��Q���i��F�ԂȂǁj�ɃA�^�b�`����R���|�[�l���g�B
/// �X�|�i�[���珉�����p�����[�^���󂯎��A�ȍ~�͎������Ĉ�����Ɉړ�����B
/// Transform.Translate �ň�����Ɉړ�����B
/// </summary>
public class DynamicObstacleMover : MonoBehaviour
{
    // �A�j���[�^�[�i�K�v�ɉ����Ďg�p�j
    [SerializeField] private Animator animator;

    // �X�|�i�[������󂯎�鏉���l�ݒ�
    private float moveSpeed;     // �ړ����x
    private Vector3 moveDir;     // �ړ������i�E or ���j

    private bool initialized = false;

    /// <summary>
    /// �X�|�i�[����Ă΂�鏉�������\�b�h�B
    /// </summary>
    public void Initialize(float speed, bool moveRight)
    {
        moveSpeed = speed;
        moveDir = moveRight ? Vector3.right : Vector3.left;
        initialized = true;

        // Animator ������Α��s�A�j�����Đ�
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // ToDo: �Đ�����A�j���[�V������ݒ�
            // animator.SetBool("IsMoving", true);
        }
    }

    void Update()
    {
        if (!initialized) return;

        // ���t���[����葬�x�ňړ�
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
    }
}
