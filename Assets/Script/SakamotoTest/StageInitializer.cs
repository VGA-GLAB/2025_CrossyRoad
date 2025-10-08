using UnityEngine;

public class StageInitializer : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    private void Awake()
    {
        if (_gridManager == null)
        {
            _gridManager = FindAnyObjectByType<GridManager>();
        }
        // テスト用ステージ生成
        var generator = new StageGenerationTestDriver();
        StageData stageData = generator.GenerateTestStage();
        // ステージ構築（論理）
        _gridManager.BuildStage(stageData);
        // 初期描画
        _gridManager.UpdateRenderArea();
    }

    private void OnDestroy()
    {
        _gridManager?.ClearAll();
    }
}
