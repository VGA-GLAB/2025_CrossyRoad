using UnityEngine;

/// <summary>
/// �X�e�[�W�����̃e�X�g�p�h���C�o�B
/// �Œ胋�[���� StageData �𐶐�����B
/// PhaseBeta��ɃX�e�[�W���������ɍ����ւ��\��B
/// </summary>
public class StageGenerationTestDriver
{
    // ���������X�e�[�W�f�[�^��ێ��i�K�v�ɉ����ĊO������Q�Ɖ\�j
    public StageData data = null;

    // ScriptableObject���烍�[�h�������s���pConfig
    private BridgeSpawnerConfig bridgeSpawnerConfig;

    /// <summary>
    /// �e�X�g�p�̃X�e�[�W�f�[�^�𐶐����ĕԂ��B
    /// - ��20, ���s��100
    /// - XZ�������̊O���� Empty
    /// - Grass ���[���ɂ̓����_���� Tree ��z�u
    /// - Road ���[���͈ꗥ Road
    /// - River ���[���͈ꗥ River
    /// </summary>
    public void Initialize()
    {
        // Resources/SpawnerConfigs/BridgeSpawnerConfigSO_Default.asset �����[�h
        var configSO = Resources.Load<BridgeSpawnerConfigSO>("SpawnerConfigs/BridgeSpawnerConfigSO_Default");
        if (configSO != null)
        {
            bridgeSpawnerConfig = configSO.ToRuntimeConfig();
        }
        else
        {
            Debug.LogError("BridgeSpawnerConfigSO_Default �����[�h�ł��܂���ł����B");
        }
    }

    /// <summary>
    /// �e�X�g�p�̃X�e�[�W�f�[�^�𐶐����ĕԂ��B
    /// - ��20, ���s��100
    /// - XZ�������̊O���� Empty
    /// - Grass ���[���ɂ̓����_���� Tree ��z�u
    /// - Road ���[���͈ꗥ Road
    /// - River ���[���͈ꗥ River
    /// </summary>
    public StageData GenerateTestStage()
    {
        if (data == null)
        {
            data = new StageData();
        }
        data.width = 20;
        data.depth = 100;

        // ���[�����Ƃ̒n�`������
        for (int z = 0; z < data.depth; z++)
        {
            // Z�����̊O���� Empty
            if (z == 0 || z == data.depth - 1)
            {
                data.laneTypes[z] = CellType.Empty;
                continue;
            }

            // �T���v�����[��: 3���[�����Ƃ� Grass, Road, River ���J��Ԃ�
            if (z % 3 == 0)
                data.laneTypes[z] = CellType.Grass;
            else if (z % 3 == 1)
                data.laneTypes[z] = CellType.Road;
            else
            {
                data.laneTypes[z] = CellType.River;

                // ���샌�[���Ȃ� BridgeSpawnerConfig ��o�^��
                var pos = new Vector3Int(0, -1, z);      // Note: ���W�̓}�b�v���������Ŋm�肷��
                var spawner = new BridgeSpawnerConfig(
                    pos,
                    bridgeSpawnerConfig.SpawnerControllerPrefab,
                    bridgeSpawnerConfig.SpawnTargetPrefabs,
                    bridgeSpawnerConfig.SpawnInterval,
                    bridgeSpawnerConfig.BridgeInterval,
                    bridgeSpawnerConfig.BridgeCountPerLane
                );

                data.spawnerConfigs.Add(spawner);
                // ���o�^������
            }
        }

        // Grass ���[���Ƀ����_���� Tree ��z�u
        for (int z = 1; z < data.depth - 1; z++)
        {
            if (data.laneTypes[z] == CellType.Grass)
            {
                for (int x = 0; x < data.width; x++)
                {
                    // 20% �̊m���Ŗ؂�z�u
                    if (Random.value < 0.2f)
                    {
                        // Note: y���W��1�Œ�i�n�`�̏�ɒu���z��j
                        // �}�b�v���������⓮�I��Q���z�u�̍ۂɒ���������Y�����ǂ����邩�K�肷��K�v����
                        Vector3Int pos = new Vector3Int(x, 1, z);
                        data.staticObstacles[pos] = ObstacleType.Tree;
                    }
                }
            }
        }

        return data;
    }
}
