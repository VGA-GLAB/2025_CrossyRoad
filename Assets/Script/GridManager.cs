using UnityEngine;

/// <summary>
/// �O���b�h�S�̂��Ǘ�����N���X�B
/// - ���W�ϊ��i���[���h���W�ƃO���b�h���W�̑��ݕϊ��j
/// - ��L�Ǘ��i�ÓI��Q���̔z�u��ԁj
/// - �v���C���[�ʒu�̓o�^�E�Q��
/// - �����i�Z����ʂ̖₢���킹�j
/// - ��Q���̔z�u�E�폜
/// ���ꌳ�I�Ɉ����B
/// </summary>
public class GridManager : MonoBehaviour
{
    //==================================================
    // �񋓌^��`
    //==================================================

    /// <summary>
    /// �Z���̎�ނ�\���񋓌^�B
    /// �����I�Ɏ�ނ𑝂₷�ꍇ�͂����ɒǉ�����B
    /// </summary>
    public enum CellType
    {
        Grass,      // �����F�ʏ�̈ړ��\�}�X
        Road,       // ���H�F�Ԃ��o�����郌�[��
        River,      // ��F�ۑ��ɏ���Ă��Ȃ���Ύ��S
        Occupied,   // �ÓI��Q���Ŗ��܂��Ă���
        Empty       // �����Ȃ�
    }

    //==================================================
    // Unity�W���C�x���g
    //==================================================

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �����������������ɋL�q�\��
    }

    // Update is called once per frame
    void Update()
    {
        // ���t���[���̍X�V�����������ɋL�q�\��
    }

    //==================================================
    // 1. ���W�ϊ��n
    //==================================================

    /// <summary>
    /// ���[���h���W���O���b�h���W(Vector3Int)�ɕϊ�����B
    /// ���p�ҁF�v���C���[�S���i�ړ��ʒu�v�Z�j�A�J�����S���i�Ǐ]�ʒu�v�Z�j�A���I��Q���S���i�ړ������j
    /// </summary>
    public Vector3Int WorldToGrid(Vector3 worldPos)
    {
        // TODO: ���[���h���W���Z���T�C�Y�Ɋ�Â��ăO���b�h���W�ɕϊ����鏈��������
        return Vector3Int.zero;
    }

    /// <summary>
    /// �O���b�h���W(Vector3Int)�����[���h���W(Vector3)�ɕϊ�����B
    /// �Z���̒��S���W��Ԃ��B
    /// </summary>
    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        // TODO: �O���b�h���W�����[���h���W�ɕϊ����鏈��������
        return Vector3.zero;
    }

    //==================================================
    // 2. ��L�Ǘ��n�i�ÓI��Q���p�j
    //==================================================

    /// <summary>
    /// �w��Z�����󂢂Ă��邩�ǂ�����Ԃ��B
    /// �ÓI��Q���̓��̓L�����Z������Ɏg�p�B
    /// </summary>
    public bool IsCellFree(Vector3Int gridPos)
    {
        // TODO: ��L��Ԃ��m�F���ĕԂ�
        return true;
    }

    /// <summary>
    /// �w��Z�����u���܂��Ă���v�Ɠo�^����B
    /// �ÓI��Q���z�u���ɌĂ΂��B
    /// </summary>
    public void OccupyCell(Vector3Int gridPos, GameObject obj)
    {
        // TODO: ��L����o�^���鏈��������
    }

    /// <summary>
    /// �w��Z�����������B
    /// �ÓI��Q���폜���ɌĂ΂��B
    /// </summary>
    public void ReleaseCell(Vector3Int gridPos)
    {
        // TODO: ��L����������鏈��������
    }

    //==================================================
    // 3. �v���C���[�ʒu�n
    //==================================================

    /// <summary>
    /// �v���C���[��o�^����i���������ɌĂԁj�B
    /// </summary>
    public void RegisterPlayer(GameObject player)
    {
        // TODO: �v���C���[�Q�Ƃ�ێ����鏈��������
    }

    /// <summary>
    /// �v���C���[�̌��݃Z�����X�V����B
    /// �v���C���[�ړ����ƂɌĂ΂��B
    /// </summary>
    public void UpdatePlayerCell(Vector3Int gridPos)
    {
        // TODO: �v���C���[�̌��݃Z�����X�V���鏈��������
    }

    /// <summary>
    /// �v���C���[�̌��݃Z����Ԃ��B
    /// �J�����Ǐ]��X�N���[�������A�X�R�A�v�Z�ɗ��p�B
    /// </summary>
    public Vector3Int GetPlayerCell()
    {
        // TODO: �v���C���[�̌��݃Z����Ԃ�����������
        return Vector3Int.zero;
    }

    /// <summary>
    /// �w��Z���̎�ނ�Ԃ��B
    /// Grass / Road / River / Occupied / Empty �ȂǁB
    /// </summary>
    public CellType GetCellType(Vector3Int gridPos)
    {
        // TODO: �Z����ʂ𔻒肵�ĕԂ�����������
        return CellType.Empty;
    }

    //==================================================
    // 4. �z�u�E�����n
    //==================================================

    /// <summary>
    /// �w��Z���ɏ�Q��Prefab��z�u���A��L�����X�V����B
    /// ���p�ҁF�ÓI��Q���S���A�X�e�[�W�����S��
    /// </summary>
    public void PlaceObstacle(Vector3Int gridPos, ObstacleType type)
    {
        // TODO: Prefab�𐶐����A��L�����X�V���鏈��������
    }

    /// <summary>
    /// �w��Z�����̃I�u�W�F�N�g���폜���A��L�����������B
    /// </summary>
    public void ClearCell(Vector3Int gridPos)
    {
        // TODO: �Z�����̃I�u�W�F�N�g���폜���A��L����������鏈��������
    }
}

/// <summary>
/// ��Q���̎�ނ�\���񋓌^�i��j�B
/// ���ۂ̃Q�[���d�l�ɉ����Ċg������B
/// </summary>
public enum ObstacleType
{
    Tree,
}
