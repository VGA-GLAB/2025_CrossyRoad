using System.Collections.Generic;
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
    // �C���X�y�N�^�[�ݒ荀��
    //==================================================
    // 1�Z���̃��[���h�T�C�Y�B
    [SerializeField] private float cellSize = 1.0f;

    // ���i���E�����j�̕`��͈�
    [SerializeField] private int renderWidth = 10;
    // ���s���i�O������j�̕`��͈�
    [SerializeField] private int renderDepthForward = 20;
    [SerializeField] private int renderDepthBackward = 5;
 
    // �����̊e��Prefab
    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject riverPrefab;
    [SerializeField] private GameObject treePrefab;

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

    /// <summary>
    /// ������A�����f�[�^�ƕ`��I�u�W�F�N�g�����ׂĉ������B
    /// </summary>
    private void OnDestroy()
    {
        ClearAll();
    }

    //==================================================
    // ���䃁�\�b�h
    //==================================================
    /// <summary>
    /// �����f�[�^�ƕ`��I�u�W�F�N�g�����ׂĉ������B
    /// �X�e�[�W�I������V�[���j�����ɌĂԁB
    /// </summary>
    public void ClearAll()
    {
        // �_���f�[�^�̃N���A
        terrainCells.Clear();
        staticObstacleCells.Clear();

        // �n�`�v���n�u�̔j��
        foreach (var obj in terrainPrefabs.Values)
        {
            if (obj != null) Destroy(obj);
        }
        terrainPrefabs.Clear();

        // ��Q���v���n�u�̔j��
        foreach (var obj in staticObstaclePrefabs.Values)
        {
            if (obj != null) Destroy(obj);
        }
        staticObstaclePrefabs.Clear();

        playerCell = Vector3Int.zero;
        player = null;
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
        // ���[���h���W���Z���T�C�Y�Ɋ�Â��ăO���b�h���W�ɕϊ����ԋp
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        int z = Mathf.FloorToInt(worldPos.z / cellSize);

        return new Vector3Int(x, y, z);
    }

    /// <summary>
    /// �O���b�h���W(Vector3Int)�����[���h���W(Vector3)�ɕϊ�����B
    /// �Z���̒��S���W��Ԃ��B
    /// </summary>
    public Vector3 GridToWorld(Vector3Int gridPos)
    {
        // �O���b�h���W�����[���h���W�ɕϊ����ԋp
        float x = gridPos.x * cellSize;
        float y = gridPos.y * cellSize;
        float z = gridPos.z * cellSize;

        return new Vector3(x, y, z);
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
        // �n�`�����݂��Ȃ��Z���͕s��
        if (!terrainCells.ContainsKey(gridPos)) return false; 

        // ��Q�����C���[���m�F���ĕԂ�
        // Note: �ÓI��Q���̔z�u�́AY���ɂ��ăL���[�u�̉��ʂ��[���ƂȂ�悤�␳���Ă�����
        // �����ł�Y=1�Ō�������B���܂��Y��Ȏ����ł͂Ȃ��̂�Y���̈����𐮗����������ŗv���P�B
        var obstaclePos = new Vector3Int(gridPos.x, 1, gridPos.z);
        return !staticObstacleCells.ContainsKey(obstaclePos);

    }

    /// <summary>
    /// �w��Z�����u���܂��Ă���v�Ɠo�^����B
    /// �ÓI��Q���z�u���ɌĂ΂��B
    /// </summary>
    public void OccupyCell(Vector3Int gridPos, ObstacleType type)
    {
        staticObstacleCells[gridPos] = type;
    }

    /// <summary>
    /// �w��Z�����������B
    /// �ÓI��Q���폜���ɌĂ΂��B
    /// </summary>
    public void ReleaseCell(Vector3Int gridPos)
    {
        staticObstacleCells.Remove(gridPos);
    }

    //==================================================
    // 3. �v���C���[�ʒu�n
    //==================================================

    /// <summary>
    /// �v���C���[��o�^����i���������ɌĂԁj�B
    /// </summary>
    public void RegisterPlayer(GameObject player)
    {
        this.player = player;
    }

    /// <summary>
    /// �v���C���[�̌��݃Z�����X�V����B
    /// �v���C���[�ړ����ƂɌĂ΂��B
    /// </summary>
    public void UpdatePlayerCell(Vector3Int gridPos)
    {
        playerCell = gridPos;
    }

    /// <summary>
    /// �v���C���[�̌��݃Z����Ԃ��B
    /// �J�����Ǐ]��X�N���[�������A�X�R�A�v�Z�ɗ��p�B
    /// </summary>
    public Vector3Int GetPlayerCell()
    {
        return playerCell;
    }

    /// <summary>
    /// �Z���̍ŏI�I�ȏ�Ԃ�Ԃ��B
    /// - ��Q��������� Occupied
    /// - �Ȃ���Βn�`���C���[�̎��
    /// - ���o�^�Ȃ� Empty
    /// </summary>
    public CellType GetCellType(Vector3Int gridPos)
    {
        if (staticObstacleCells.ContainsKey(gridPos))
            return CellType.Occupied;

        if (terrainCells.TryGetValue(gridPos, out var type))
            return type;

        return CellType.Empty;
    }

    /// <summary>
    /// �Z���̒n�`���C���[�݂̂�Ԃ��B
    /// ��Q���͖������ď����ɒn�`�^�C�v��Ԃ��B
    /// </summary>
    public CellType GetTerrainCellType(Vector3Int gridPos)
    {
        if (terrainCells.TryGetValue(gridPos, out var type))
            return type;

        return CellType.Empty;
    }

    //==================================================
    // 4. �z�u�E�����n
    //==================================================
    /// <summary>
    /// �w��Z���ɒn�`Prefab��z�u����B
    /// ���łɕ`��ς݂Ȃ牽�����Ȃ��B
    /// </summary>
    private void CreateTerrainPrefab(Vector3Int gridPos, CellType type)
    {
        if (terrainPrefabs.ContainsKey(gridPos))
            return;

        GameObject prefab = null;
        switch (type)
        {
            case CellType.Grass: prefab = grassPrefab; break;
            case CellType.Road: prefab = roadPrefab; break;
            case CellType.River: prefab = riverPrefab; break;
            case CellType.Empty: prefab = null; break;
        }

        if (prefab != null)
        {
            Vector3 worldPos = GridToWorld(gridPos);
            worldPos.y = -0.5f * cellSize;      // �L���[�u�̏�ʂ��[�����W�ƂȂ�悤Y���W�𒲐�
            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);
            terrainPrefabs[gridPos] = obj;
        }
    }

    /// <summary>
    /// �w��Z���̒n�`Prefab���폜����B
    /// </summary>
    private void DestroyTerrainPrefab(Vector3Int gridPos)
    {
        if (terrainPrefabs.TryGetValue(gridPos, out var obj))
        {
            Destroy(obj);
            terrainPrefabs.Remove(gridPos);
        }
    }

    /// <summary>
    /// �w��Z���ɏ�Q��Prefab��z�u����B
    /// ���łɏ�Q��������ꍇ�͉������Ȃ��B
    /// </summary>
    private void CreateObstaclePrefab(Vector3Int gridPos, ObstacleType type)
    {
        if (staticObstaclePrefabs.ContainsKey(gridPos))
        {
            return; // ���łɐ����ς݂Ȃ牽�����Ȃ�
        }

        GameObject prefab = null;
        switch (type)
        {
            case ObstacleType.Tree: prefab = treePrefab; break;
        }

        if (prefab != null)
        {
            Vector3 worldPos = GridToWorld(gridPos);
            worldPos.y = 0.5f * cellSize;      // �L���[�u�̉��ʂ��[�����W�ƂȂ�悤Y���W�𒲐�

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, this.transform);
            staticObstaclePrefabs[gridPos] = obj;
        }
    }

    /// <summary>
    /// �w��Z�����̃I�u�W�F�N�g���폜����B
    /// </summary>
    private void DestroyObstaclePrefab(Vector3Int gridPos)
    {
        if (staticObstaclePrefabs.TryGetValue(gridPos, out var obj))
        {
            Destroy(obj);
            staticObstaclePrefabs.Remove(gridPos);
        }
    }

    /// <summary>
    /// �w��Z���ɏ�Q��Prefab��z�u���A��L�����X�V����B
    /// ���łɏ�Q��������ꍇ�͉������Ȃ��B
    /// ���p�ҁF�ÓI��Q���S���A�X�e�[�W�����S��
    /// </summary>
    public void PlaceObstacleCell(Vector3Int gridPos, ObstacleType type)
    {
        // �Z�����ɃI�u�W�F�N�g�𐶐����A��L����o�^����
        OccupyCell(gridPos, type);
        CreateObstaclePrefab(gridPos, type);
    }

    /// <summary>
    /// �w��Z�����̃I�u�W�F�N�g���폜���A��L�����������B
    /// �n�`���C���[�͕ύX���Ȃ��B
    /// </summary>
    public void ClearObstacleCell(Vector3Int gridPos)
    {
        // �Z�����̃I�u�W�F�N�g���폜���A��L�����������
        ReleaseCell(gridPos);
        DestroyObstaclePrefab(gridPos);
    }

    /// <summary>
    /// StageData ���󂯎��A�}�b�v�S�̂̃��C���[���\�z����B
    /// </summary>
    public void BuildStage(StageData data)
    {
        terrainCells.Clear();
        staticObstacleCells.Clear();

        // �n�`���C���[�𔽉f
        foreach (var lane in data.laneTypes)
        {
            int z = lane.Key;
            CellType type = lane.Value;

            for (int x = 0; x < data.width; x++)
            {
                Vector3Int gridPos = new Vector3Int(x, 0, z);
                terrainCells[gridPos] = type;
            }
        }

        // �ÓI��Q�����C���[�𔽉f
        foreach (var obstacle in data.staticObstacles)
        {
            Vector3Int gridPos = obstacle.Key;
            ObstacleType type = obstacle.Value;
            staticObstacleCells[gridPos] = type;
        }
    }

    /// <summary>
    /// �v���C���[�ʒu����ɕ`��͈͂��X�V����
    /// </summary>
    public void UpdateRenderArea()
    {
        var center = playerCell;
        var newVisible = new HashSet<Vector3Int>();

        // ������: -renderWidth �` +renderWidth
        // ���s������: -renderDepthBackward �` +renderDepthForward
        for (int dx = -renderWidth; dx <= renderWidth; dx++)
        {
            for (int dz = -renderDepthBackward; dz <= renderDepthForward; dz++)
            {
                var pos = new Vector3Int(center.x + dx, 0, center.z + dz);
                newVisible.Add(pos);

                // �n�`�����݂���Ε`��
                if (terrainCells.TryGetValue(pos, out var terrainType))
                    CreateTerrainPrefab(pos, terrainType);

                // ��Q�������݂���Ε`��
                // Note: �ÓI��Q���̔z�u�́AY���ɂ��ăL���[�u�̉��ʂ��[���ƂȂ�悤�␳���Ă�����
                // �����ł�Y=1�Ō�������B���܂��Y��Ȏ����ł͂Ȃ��̂�Y���̈����𐮗����������ŗv���P�B
                pos.y = 1;
                if (staticObstacleCells.TryGetValue(pos, out var obstacleType))
                {
                    CreateObstaclePrefab(pos, obstacleType);
                    newVisible.Add(pos);
                }
            }
        }

        // �͈͊O�ɂȂ����Z�����폜
        foreach (var kv in new List<Vector3Int>(terrainPrefabs.Keys))
        {
            if (!newVisible.Contains(kv))
                DestroyTerrainPrefab(kv);
        }
        foreach (var kv in new List<Vector3Int>(staticObstaclePrefabs.Keys))
        {
            // Note: �ÓI��Q���̔z�u�́AY���ɂ��ăL���[�u�̉��ʂ��[���ƂȂ�悤�␳���Ă�����
            // �����ł�Y=1�Ō�������B�v���P�B
            var comparePos = new Vector3Int(kv.x, 1, kv.z);
            
            if (!newVisible.Contains(comparePos))
                DestroyObstaclePrefab(kv);
        }
    }

    //==================================================
    // �������
    //==================================================
    // �v���C���[�̌��݃Z���ʒu
    private Vector3Int playerCell = Vector3Int.zero;

    // �v���C���[�̎Q��
    private GameObject player;

    //-------------------------------------------------- 
    // ����񃌃C���[
    //-------------------------------------------------- 

    //
    // �}�b�v�S��
    //

    // �n�`���C���[�i�Z���P�ʁj
    private Dictionary<Vector3Int, CellType> terrainCells = new Dictionary<Vector3Int, CellType>();
    
    // �ÓI��Q�����C���[�i�Z���P�ʁj
    private Dictionary<Vector3Int, ObstacleType> staticObstacleCells = new Dictionary<Vector3Int, ObstacleType>();

    //
    // �`��̈�
    //

    // �n�`���C���[�i�v���n�u�j
    private Dictionary<Vector3Int, GameObject> terrainPrefabs = new Dictionary<Vector3Int, GameObject>();
    
    // �ÓI��Q�����C���[�i�v���n�u�j
    private Dictionary<Vector3Int, GameObject> staticObstaclePrefabs = new Dictionary<Vector3Int, GameObject>();
}

