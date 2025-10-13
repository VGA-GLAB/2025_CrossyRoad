using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �X�|�i�[�ݒ�̊��N���X�B
/// StageData �Ɋi�[����AGridManager �����߂��� Spawner �𐶐�����B
/// �f�[�^�݂̂�ێ����A���W�b�N�͎����Ȃ��B
/// </summary>
public abstract class SpawnerConfigBase
{
    /// <summary>
    /// �X�|�i�[��z�u����O���b�h��̍��W�B
    /// </summary>
    public Vector3Int Position { get; }

    /// <summary>
    /// �X�|�i�[�{�̂̃v���n�u�B
    /// MonoBehaviour (��: BridgeSpawner, CarSpawner) ���A�^�b�`������I�u�W�F�N�g��z��B
    /// GridManager �� Instantiate ���ăV�[���ɔz�u����B
    /// </summary>
    public GameObject SpawnerControllerPrefab { get; }

    /// <summary>
    /// �X�|�i�[����������Ώۂ̃v���n�u�B
    /// ��: ���A�ԁA�G�ȂǁB�K���������o���O��Ŋ��ɐ錾�B
    /// </summary>
    public IReadOnlyList<GameObject> SpawnTargetPrefabs { get; }

    /// <summary>
    /// �R���X�g���N�^�B
    /// Config �͍쐬��ɒl��ύX���Ȃ����߁A�������ɂ��ׂĂ̒l��ݒ肷��B
    /// </summary>
    protected SpawnerConfigBase(
        Vector3Int position,
        GameObject spawnerControllerPrefab,
        IReadOnlyList<GameObject> spawnTargetPrefabs)
    {
        Position = position;
        SpawnerControllerPrefab = spawnerControllerPrefab;
        SpawnTargetPrefabs = spawnTargetPrefabs;
    }
}
