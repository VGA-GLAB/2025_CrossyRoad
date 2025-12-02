using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GridManager の単体テストドライバー。
/// 空の GameObject にアタッチして利用する。
/// - Start() でステージ生成＋初期描画
/// - Update() で矢印キーによるプレイヤー移動＋描画更新
/// - 各種キーで例外ケースや解放処理を確認
/// </summary>
public class GridManagerTestDriver : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;           // GridManager コンポーネント
    [SerializeField] private GameObject playerVisualPrefab;     // プレイヤーに見立てたオブジェクト
    [SerializeField] private Camera followCamera;               // プレイヤー追従カメラ

    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 10f, 0f);
    [SerializeField] private Vector3 cameraEulerAngles = new Vector3(75f, 0f, 0f);

    private GameObject playerObj;

    private void Start()
    {
        var generator = new StageGenerationTestDriver();
        generator.Initialize();
        StageData stageData = generator.GenerateTestStage();

        // ステージ生成（論理のみ）
        gridManager.BuildStage(stageData);

        // 初期描画
        gridManager.UpdateRenderArea();


        // プレイヤー可視化オブジェクトを生成
        if (playerVisualPrefab != null)
        {
            playerObj = Instantiate(playerVisualPrefab);
            playerObj.name = "TestPlayer";

            // プレイヤーの高さを補正（下面がY=0になるように）
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

        // GridManagerにプレイヤー登録
        gridManager.RegisterPlayer(playerObj);


        // 初期セルを (1,0,1) に設定
        Vector3Int startCell = new Vector3Int(1, 0, 1);
        gridManager.UpdatePlayerCell(startCell);

        Debug.Log("=== GridManager TestDriver 起動 ===");
        Debug.Log("GridToWorld(1,0,1) = " + gridManager.GridToWorld(new Vector3Int(1, 0, 1)));
        Debug.Log("GetPlayerCell() = " + gridManager.GetPlayerCell());
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
            gridManager.UpdateRenderArea();

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

            var testPos = new Vector3Int(0, 0, 0);
            gridManager.PlaceObstacleCell(testPos, ObstacleType.Tank);
            gridManager.PlaceObstacleCell(testPos, ObstacleType.Tank); // 2回目は無視されるはず

            Debug.Log("Emptyセル描画テスト: prefab=null のため生成されないことを確認");
            gridManager.UpdateRenderArea();
        }
    }

    private void LateUpdate()
    {
        if (playerObj != null && gridManager != null)
        {
            // プレイヤーセルのワールド座標にスフィアを追従させる
            Vector3 worldPos = gridManager.GridToWorld(gridManager.GetPlayerCell());
            worldPos.y = 0.5f * playerObj.transform.localScale.y;
            playerObj.transform.position = worldPos;
        }

        if (followCamera != null && gridManager != null)
        {
            // プレイヤーのグリッド座標をワールド座標に変換
            Vector3 worldPos = gridManager.GridToWorld(gridManager.GetPlayerCell());

            // カメラを配置
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
