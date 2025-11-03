using System.Collections.Generic;

/// <summary>
/// DynamicObstaclesSpawnerGroupConfigSO を展開した実行時用のエントリ。
/// - グループの難易度
/// - グループに含まれる DynamicObstaclesSpawnerConfig のリスト
/// を保持する。
/// </summary>
public class DynamicObstaclesGroupEntry
{
    public byte difficultyAppear;
    public byte difficultyCost;
    public List<DynamicObstaclesSpawnerConfig> obstacleConfigs;

    public DynamicObstaclesGroupEntry(byte difficultyAppear, byte difficultyCost, List<DynamicObstaclesSpawnerConfig> obstacleConfigs)
    {
        this.difficultyAppear = difficultyAppear;
        this.difficultyCost = difficultyCost;
        this.obstacleConfigs = obstacleConfigs;
    }
}
