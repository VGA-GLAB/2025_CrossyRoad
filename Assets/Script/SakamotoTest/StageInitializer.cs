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
        // �e�X�g�p�X�e�[�W����
        var generator = new StageGenerationTestDriver();
        StageData stageData = generator.GenerateTestStage();
        // �X�e�[�W�\�z�i�_���j
        _gridManager.BuildStage(stageData);
        // �����`��
        _gridManager.UpdateRenderArea();
    }

    private void OnDestroy()
    {
        _gridManager?.ClearAll();
    }
}
