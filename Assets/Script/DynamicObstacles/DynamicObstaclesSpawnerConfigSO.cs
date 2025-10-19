using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���I��Q���X�|�i�[�̐ݒ�f�[�^��ێ����� ScriptableObject�B
/// </summary>
[CreateAssetMenu(
    fileName = "DynamicObstaclesSpawnerConfigSO_Default",
    menuName = "Spawner/DynamicObstaclesSpawnerConfig",
    order = 0)]
public class DynamicObstaclesSpawnerConfigSO : ScriptableObject
{
    [Header("�X�|�[���Ώۂ̏�Q����Prefab���X�g")]
    public List<GameObject> dynamicObstacles;

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
}
