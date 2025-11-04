using UnityEngine;

/// <summary>
/// GridManager + StageGenerator の統合テストドライバ。
/// 空の GameObject にアタッチして利用する。
/// - Start() で StageGenerator を初期化し、最初のチャンクを生成
/// - Update() で矢印キーによるプレイヤー移動＋UpdateStageFlow 呼び出し
/// - 各種キーで例外ケースや解放処理を確認
/// </summary>
public class GridManagerAutoGenTestDriver : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;       // GridManager コンポーネント
    [SerializeField] private StageGenerator stageGenerator; // StageGenerator コンポーネント
    [SerializeField] private GameObject playerVisualPrefab; // プレイヤーに見立てたオブジェクト
    [SerializeField] private Camera followCamera;           // プレイヤー追従カメラ

    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 10f, -5f);
    [SerializeField] private Vector3 cameraEulerAngles = new Vector3(75f, 0f, 0f);

    private GameObject playerObj;

    private void Start()
    {
        // プレイヤー可視化オブジェクトを生成
        if (playerVisualPrefab != null)
        {
            playerObj = Instantiate(playerVisualPrefab);
            playerObj.name = "TestPlayer";

            Vector3Int startCell = new Vector3Int(1, 0, 1);
            Vector3 playerPos = gridManager.GridToWorld(startCell);
            playerPos.y = playerObj.transform.localScale.y * 0.5f;
            playerObj.transform.position = playerPos;
        }
        else
        {
            playerObj = new GameObject("TestPlayer");
        }

        // GridManagerにプレイヤー登録
        gridManager.RegisterPlayer(playerObj);

        // 初期セルを設定
        gridManager.UpdatePlayerCell(new Vector3Int(1, 0, 1));
        gridManager.UpdateStageFlow();

        Debug.Log("=== GridManagerAutoGenTestDriver 起動 ===");
    }

    private void Update()
    {
        // 矢印キーでプレイヤー移動
        Vector3Int move = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.UpArrow)) move = Vector3Int.forward;
        if (Input.GetKeyDown(KeyCode.DownArrow)) move = Vector3Int.back;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) move = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) move = Vector3Int.right;

        if (move != Vector3Int.zero)
        {
            var newPos = gridManager.GetPlayerCell() + move;
            gridManager.UpdatePlayerCell(newPos);

            // 自動生成フローを呼び出し
            gridManager.UpdateStageFlow();

            Debug.Log($"プレイヤー移動: {newPos}, CellType={gridManager.GetCellType(newPos)}");
        }

        // Cキー: マップデータ解放
        if (Input.GetKeyDown(KeyCode.C))
        {
            gridManager.ClearAll();
            Debug.Log("マップデータをクリアしました");
        }

        // Iキー: IsCellFree 確認
        if (Input.GetKeyDown(KeyCode.I))
        {
            var pos = gridManager.GetPlayerCell();
            Debug.Log($"IsCellFree({pos}) = {gridManager.IsCellFree(pos)}");
        }

        // Eキー: 例外ケース確認
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("存在しないセル: " + gridManager.GetCellType(new Vector3Int(999, 0, 999)));
        }
    }

    private void LateUpdate()
    {
        if (playerObj != null && gridManager != null)
        {
            // プレイヤーセルのワールド座標に追従
            Vector3 worldPos = gridManager.GridToWorld(gridManager.GetPlayerCell());
            worldPos.y = 0.5f * playerObj.transform.localScale.y;
            playerObj.transform.position = worldPos;
        }

        if (followCamera != null && gridManager != null)
        {
            Vector3 worldPos = gridManager.GridToWorld(gridManager.GetPlayerCell());
            followCamera.transform.position = worldPos + cameraOffset;
            followCamera.transform.rotation = Quaternion.Euler(cameraEulerAngles);
        }
    }

    private void OnDestroy()
    {
        gridManager.ClearAll();
        Debug.Log("OnDestroy: マップデータを解放しました");
    }
}
