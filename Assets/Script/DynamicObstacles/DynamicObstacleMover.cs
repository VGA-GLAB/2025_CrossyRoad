using System;
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
    
    private ObjectType objectType; //自身のオブジェクトタイプを保持
    private bool isSound; //SE再生済みか

    /// <summary>
    /// �X�|�i�[����Ă΂�鏉�������\�b�h�B
    /// </summary>
    public void Initialize(float speed, bool moveRight, ObjectType type)
    {
        moveSpeed = speed;
        moveDir = moveRight ? Vector3.right : Vector3.left;
        initialized = true;
        objectType = type;

        // ���f�����]����
        Vector3 localScale = transform.localScale;
        if (moveRight)
        {
            // �E���� �� ����X�P�[��
            localScale.x = Mathf.Abs(localScale.x);
        }
        else
        {
            // ������ �� X�����]
            localScale.x = -Mathf.Abs(localScale.x);
        }

        transform.localScale = localScale;


        // Animator ������Α��s�A�j�����Đ�
        // (Animator �Ŏ����Ń��[�v�Đ�����邽�ߌ���A�v���O��������s�v)
        //animator = GetComponent<Animator>();
        //if (animator != null)
        //{
        //    // ToDo: �Đ�����A�j���[�V������ݒ�
        //    // animator.SetBool("IsMoving", true);
        //}
    }

    void Update()
    {
        if (!initialized) return;

        // ���t���[����葬�x�ňړ�
        transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
        SoundType();
    }

    /// <summary>
    /// オブジェクトタイプに応じてSEを再生する
    /// </summary>
    private void SoundType()
    {
        if (isSound)
        {
            if (SoundManager.instance.PlayerDistance(this.gameObject))
            {
                SoundManager.instance.ObstacleStopSE(this.gameObject);
            }
            return;
        }
        
        switch (objectType)
        {
            case ObjectType.EnemyRobot:
                SoundManager.instance.ObstaclePlaySE("敵ロボットの移動", this.gameObject);
                isSound = true;
                break;
            
            case ObjectType.Saw:
                SoundManager.instance.ObstaclePlaySE("ノコギリ移動", this.gameObject);
                isSound = true;
                break;
        }
    }

    private void OnDestroy()
    {
        SoundManager.instance.ObstacleStopSE(this.gameObject);
    }
}
