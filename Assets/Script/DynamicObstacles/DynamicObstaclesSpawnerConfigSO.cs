using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���I��Q���X�|�i�[�̐ݒ�f�[�^��ێ����� ScriptableObject�B
/// </summary>
[CreateAssetMenu(
    fileName = "DynamicObstaclesSpawnerConfigSO_Default",
    menuName = "Stage/DynamicObstaclesSpawnerConfig",
    order = 0)]
public class DynamicObstaclesSpawnerConfigSO : ScriptableObject
{
    [Header("�X�|�i�[�{�̂�Prefab")]
    public GameObject spawnerControllerPrefab;

    [Header("�X�|�[���Ώۂ̏�Q����Prefab���X�g")]
    public List<GameObject> dynamicObstaclePrefabs;

    [Header("�X�|�[���Ώۂ̏�Q���̈ړ����x�i���[���P�ʂŌŒ�j")]
    public float moveSpeed = 10.0f;
    public bool moveRight = true;

    [Header("�X�|�[���Ԋu�ݒ�")]
    public float baseSpawnInterval = 3.0f;
    public float spawnIntervalJitter = 0.5f;

    [Header("�ґ��ݒ�")]
    public int minBatchCount = 1;
    public int maxBatchCount = 1;
    public float batchSpacing = 1.5f;

    [Header("Destroy����܂ł̎���")]
    public float lifeTime = 12.0f;

    [Header("���̃X�|�i�[���Ή����铹�H�^�C�v")]
    public CellType roadCellType = CellType.RoadRobot;

    [Header("オブジェクトのタイプ")] 
    public ObjectType objectType;

    /// <summary>
    /// ScriptableObject �ɕۑ����ꂽ�l�����ƂɁA
    /// ���s����p�� <see cref="DynamicObstaclesSpawnerConfig"/> �𐶐�����B
    /// 
    /// - �X�e�[�W���������R�[�h�͂��̃��\�b�h��ʂ��� Config ���擾����B
    /// - Config �͓ǂݎ���p�̕s�σI�u�W�F�N�g�Ƃ��Ĉ����邽�߁A
    ///   ���s���ɒl����������邱�Ƃ�h����B
    /// - �܂��A���̕ϊ������́uDynamicObstaclesSpawnerConfig �� SO �̍��ڂ���v���Ă��邩�v
    ///   ���R���p�C�����Ƀ`�F�b�N������������˂Ă���B
    ///   �i���ڂ����������ꍇ�A�����ŃR���p�C���G���[�ƂȂ�C�t����j
    /// </summary>
    public DynamicObstaclesSpawnerConfig ToRuntimeConfig()
    {
        return new DynamicObstaclesSpawnerConfig(
            Vector3Int.zero,            // Position�͌��StageGeneration���Őݒ�
            spawnerControllerPrefab,
            dynamicObstaclePrefabs,
            moveSpeed,
            moveRight,
            baseSpawnInterval,
            spawnIntervalJitter,
            minBatchCount,
            maxBatchCount,
            batchSpacing,
            lifeTime,
            roadCellType,
            objectType
        );
    }
}
