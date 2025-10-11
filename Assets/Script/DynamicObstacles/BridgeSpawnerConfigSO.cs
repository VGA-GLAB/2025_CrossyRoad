using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���X�|�i�[�̐ݒ�� ScriptableObject �Ƃ��ĕێ�����A�Z�b�g�B
/// 
/// - Unity �G�f�B�^��ŃC���X�y�N�^����ҏW�\�ȁu�ݒ�t�@�C���v�Ƃ��ė��p����B
/// - �v���n�u�A�Z�b�g�i�X�|�i�[�{�̂⋴�̃v���n�u�Q�j��R�t���A
///   �����Ԋu�⋴�̐��Ȃǂ̃p�����[�^���p�^�[�������ĕۑ��ł���B
/// - �X�e�[�W�����������ɂ́A���� ScriptableObject �����[�h���A
///   ���s����p�� <see cref="BridgeSpawnerConfig"/> �ɕϊ����ė��p����B
/// 
/// �܂�A�G�f�B�^�ō쐬�E�ۑ�����u�ҏW�p�f�[�^�v�ƁA
/// ���s���ɎQ�Ƃ���u�ǂݎ���p�f�[�^�v�𕪗����邽�߂̎d�g�݁B
/// </summary>
[CreateAssetMenu(fileName = "BridgeSpawnerConfigSO", menuName = "Stage/BridgeSpawnerConfig")]
public class BridgeSpawnerConfigSO : ScriptableObject
{
    [Header("�X�|�i�[��z�u����O���b�h��̍��W")]
    public Vector3Int position;

    [Header("�X�|�i�[�{�̂̃v���n�u (BridgeSpawner �Ȃ�)")]
    public GameObject spawnerControllerPrefab;

    [Header("���̃v���n�u�Q�i������`��̃o���G�[�V������o�^�j")]
    public List<GameObject> bridgePrefabs;

    [Header("���̐����Ԋu�i�b�j")]
    public float spawnInterval;

    [Header("1���[�����ŋ�����ׂ�Ԋu�i�b�j")]
    public float bridgeInterval;

    [Header("1���[���ɐ������鋴�̐�")]
    public int bridgeCountPerLane;

    /// <summary>
    /// ScriptableObject �ɕۑ����ꂽ�l�����ƂɁA
    /// ���s����p�� <see cref="BridgeSpawnerConfig"/> �𐶐�����B
    /// 
    /// - �X�e�[�W���������R�[�h�͂��̃��\�b�h��ʂ��� Config ���擾����B
    /// - Config �͓ǂݎ���p�̕s�σI�u�W�F�N�g�Ƃ��Ĉ����邽�߁A
    ///   ���s���ɒl����������邱�Ƃ�h����B
    /// - �܂��A���̕ϊ������́uBridgeSpawnerConfig �� SO �̍��ڂ���v���Ă��邩�v
    ///   ���R���p�C�����Ƀ`�F�b�N������������˂Ă���B
    ///   �i���ڂ����������ꍇ�A�����ŃR���p�C���G���[�ƂȂ�C�t����j
    /// </summary>
    public BridgeSpawnerConfig ToRuntimeConfig()
    {
        return new BridgeSpawnerConfig(
            position,
            spawnerControllerPrefab,
            bridgePrefabs,
            spawnInterval,
            bridgeInterval,
            bridgeCountPerLane
        );
    }
}
