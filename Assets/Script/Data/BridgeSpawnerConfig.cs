using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���X�|�i�[�̐ݒ�f�[�^�B
/// StageData �Ɋi�[����AGridManager �����߂��� BridgeSpawner �𐶐�����B
/// </summary>
public sealed class BridgeSpawnerConfig : SpawnerConfigBase
{
    /// <summary>
    /// ���̐����Ԋu�i�b�j�B
    /// spawnTime �ɑΉ��B
    /// </summary>
    public float SpawnInterval { get; }

    /// <summary>
    /// 1 ���[�����ŋ�����ׂ�Ԋu�i�b�j�B
    /// bridgeInterval �ɑΉ��B
    /// </summary>
    public float BridgeInterval { get; }

    /// <summary>
    /// 1 ���[���ɐ������鋴�̐��B
    /// bridgeNum �ɑΉ��B
    /// </summary>
    public int BridgeCountPerLane { get; }

    /// <summary>
    /// �R���X�g���N�^�B
    /// Config �͕s�σI�u�W�F�N�g�Ƃ��Ĉ������߁A���ׂĂ̒l���R���X�g���N�^�Őݒ肷��B
    /// </summary>
    public BridgeSpawnerConfig(
        Vector3Int position,
        GameObject spawnerControllerPrefab,
        IReadOnlyList<GameObject> spawnTargetPrefabs,
        float spawnInterval,
        float bridgeInterval,
        int bridgeCountPerLane
    ) : base(position, spawnerControllerPrefab, spawnTargetPrefabs)
    {
        SpawnInterval = spawnInterval;
        BridgeInterval = bridgeInterval;
        BridgeCountPerLane = bridgeCountPerLane;
    }
}
