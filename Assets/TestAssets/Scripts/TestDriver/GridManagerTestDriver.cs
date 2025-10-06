using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GridManager �̒P�̃e�X�g�h���C�o�[�B
/// ��� GameObject �ɃA�^�b�`���ė��p����B
/// - Start() �ŃX�e�[�W�����{�����`��
/// - Update() �Ŗ��L�[�ɂ��v���C���[�ړ��{�`��X�V
/// - �e��L�[�ŗ�O�P�[�X�����������m�F
/// </summary>
public class GridManagerTestDriver : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;           // GridManager �R���|�[�l���g
    [SerializeField] private GameObject playerVisualPrefab;     // �v���C���[�Ɍ����Ă��I�u�W�F�N�g
    [SerializeField] private Camera followCamera;               // �v���C���[�Ǐ]�J����

    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 10f, 0f);
    [SerializeField] private Vector3 cameraEulerAngles = new Vector3(75f, 0f, 0f);

    private GameObject playerObj;

    private void Start()
    {
        var generator = new StageGenerationTestDriver();
        StageData stageData = generator.GenerateTestStage();

        // �X�e�[�W�����i�_���̂݁j
        gridManager.BuildStage(stageData);

        // �����`��
        gridManager.UpdateRenderArea();


        // �v���C���[�����I�u�W�F�N�g�𐶐�
        if (playerVisualPrefab != null)
        {
            playerObj = Instantiate(playerVisualPrefab);
            playerObj.name = "TestPlayer";

            // �v���C���[�̍�����␳�i���ʂ�Y=0�ɂȂ�悤�Ɂj
            var renderer = playerObj.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Vector3Int gridPos = new Vector3Int(1, 0, 1);
                Vector3 playerPos = gridManager.GridToWorld(gridPos);

                float halfHeight = playerObj.transform.localScale.y * 0.5f;
                playerPos.y = halfHeight;
                playerObj.transform.position = playerPos;
            }
        }
        else
        {
            playerObj = new GameObject("TestPlayer");
        }

        // GridManager�Ƀv���C���[�o�^
        gridManager.RegisterPlayer(playerObj);


        // �����Z���� (1,0,1) �ɐݒ�
        Vector3Int startCell = new Vector3Int(1, 0, 1);
        gridManager.UpdatePlayerCell(startCell);

        Debug.Log("=== GridManager TestDriver �N�� ===");
        Debug.Log("GridToWorld(1,0,1) = " + gridManager.GridToWorld(new Vector3Int(1, 0, 1)));
        Debug.Log("GetPlayerCell() = " + gridManager.GetPlayerCell());
    }

    private void Update()
    {
        // ���L�[�Ńv���C���[�ړ�
        Vector3Int move = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow)) move = Vector3Int.forward;
        if (Input.GetKeyDown(KeyCode.DownArrow)) move = Vector3Int.back;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) move = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) move = Vector3Int.right;

        if (move != Vector3Int.zero)
        {
            var newPos = gridManager.GetPlayerCell() + move;
            gridManager.UpdatePlayerCell(newPos);
            gridManager.UpdateRenderArea();

            Debug.Log($"�v���C���[�ړ�: {newPos}, CellType={gridManager.GetCellType(newPos)}");
        }

        // C�L�[: �}�b�v�f�[�^���
        if (Input.GetKeyDown(KeyCode.C))
        {
            gridManager.ClearAll();
            Debug.Log("�}�b�v�f�[�^���N���A���܂���");
        }

        // I�L�[: IsCellFree �m�F
        if (Input.GetKeyDown(KeyCode.I))
        {
            var pos = gridManager.GetPlayerCell();
            Debug.Log($"IsCellFree({pos}) = {gridManager.IsCellFree(pos)}");
        }

        // E�L�[: ��O�P�[�X�m�F
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("���݂��Ȃ��Z��: " + gridManager.GetCellType(new Vector3Int(999, 0, 999)));

            var testPos = new Vector3Int(0, 0, 0);
            gridManager.PlaceObstacleCell(testPos, ObstacleType.Tree);
            gridManager.PlaceObstacleCell(testPos, ObstacleType.Tree); // 2��ڂ͖��������͂�

            Debug.Log("Empty�Z���`��e�X�g: prefab=null �̂��ߐ�������Ȃ����Ƃ��m�F");
            gridManager.UpdateRenderArea();
        }
    }

    private void LateUpdate()
    {
        if (playerObj != null && gridManager != null)
        {
            // �v���C���[�Z���̃��[���h���W�ɃX�t�B�A��Ǐ]������
            Vector3 worldPos = gridManager.GridToWorld(gridManager.GetPlayerCell());
            worldPos.y = 0.5f * playerObj.transform.localScale.y;
            playerObj.transform.position = worldPos;
        }

        if (followCamera != null && gridManager != null)
        {
            // �v���C���[�̃O���b�h���W�����[���h���W�ɕϊ�
            Vector3 worldPos = gridManager.GridToWorld(gridManager.GetPlayerCell());

            // �J������z�u
            followCamera.transform.position = worldPos + cameraOffset;
            followCamera.transform.rotation = Quaternion.Euler(cameraEulerAngles);
        }
    }

    private void OnDestroy()
    {
        gridManager.ClearAll();
        Debug.Log("OnDestroy: �}�b�v�f�[�^��������܂���");
    }
}
