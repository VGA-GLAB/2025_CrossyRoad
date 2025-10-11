using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�e�[�W�S�̂̃f�[�^�\���B
/// - �}�b�v�T�C�Y�i���E���s���j
/// - �n�`���C���[�i���[���P�ʁj
/// - �ÓI��Q�����C���[�i�Z���P�ʁj
/// </summary>
public class StageData
{
    public int width;   // �}�b�v�̉����ix�����j
    public int depth;   // �}�b�v�̉��s���iz�����j

    /// <summary>
    /// �n�`���C���[
    /// key: z���W�i���[���ԍ��j
    /// value: CellType�iGrass, Road, River, Empty�Ȃǁj
    /// </summary>
    public Dictionary<int, CellType> laneTypes
        = new Dictionary<int, CellType>();

    /// <summary>
    /// �ÓI��Q�����C���[
    /// key: �O���b�h���W
    /// value: ��Q���̎��
    /// </summary>
    public Dictionary<Vector3Int, ObstacleType> staticObstacles
        = new Dictionary<Vector3Int, ObstacleType>();

    /// <summary>
    /// �X�|�i�[�ݒ胊�X�g�B
    /// - �e�X�|�i�[�̈ʒu�␶���Ώۂ�ێ�����B
    /// - GridManager �����߂��ăV�[���ɃX�|�i�[��z�u����B
    /// - BridgeSpawnerConfig �ȂǁASpawnerConfigBase ���p�������^���i�[�B
    /// </summary>
    public List<SpawnerConfigBase> spawnerConfigs
        = new List<SpawnerConfigBase>();
}
