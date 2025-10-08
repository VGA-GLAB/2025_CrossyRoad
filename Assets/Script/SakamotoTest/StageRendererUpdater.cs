using UnityEngine;

/// <summary>
/// �v���C���[�̈ʒu�ω��ɉ����ăX�e�[�W�̕`��͈͂��X�V����N���X�B
/// 
/// GridManager ���ێ�����u���݂̃v���C���[�Z���v���Ď����A
/// �ʒu���ω������ꍇ�̂� UpdateRenderArea() ���Ăяo���B
/// 
/// ���̃X�N���v�g����� GameObject �ɃA�^�b�`���Ă������ƂŁA
/// PlayerMove �̈ړ������Ɏ���������`��X�V���������ł���B
/// </summary>
public class StageRendererUpdater : MonoBehaviour
{
    private GridManager _gridManager;
    private PlayerMove _player;
    // �O�t���[���ł̃v���C���[�̃Z�����W
    private Vector3Int _lastPlayerCell;

    private void Start()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
        _player = FindAnyObjectByType<PlayerMove>();
        if (_gridManager == null || _player == null)
        {
            Debug.LogError("[StageRendererUpdater] GridManager �܂��� PlayerMove ��������܂���B");
            enabled = false;
            return;
        }
        // �����Z���ʒu���L�^���Ă���
        _lastPlayerCell = _gridManager.GetPlayerCell();
    }

    private void Update()
    {
        // ���S�m�F�F�Q�Ƃ��r���Ŕj������Ă��Ȃ����`�F�b�N
        if (_player == null || _gridManager == null) return;
        // ���݂̃v���C���[�̃Z���ʒu���擾
        Vector3Int currentCell = _gridManager.GetPlayerCell();
        // �v���C���[���ʂ̃Z���Ɉړ����Ă����ꍇ�̂ݕ`��X�V���s��
        if (currentCell != _lastPlayerCell)
        {
            // �V�����`��͈͂��X�V
            _gridManager.UpdateRenderArea();
            // �O��ʒu���X�V
            _lastPlayerCell = currentCell;
        }
    }
}
