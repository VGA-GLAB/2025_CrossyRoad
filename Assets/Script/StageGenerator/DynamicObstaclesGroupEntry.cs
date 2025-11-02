using System.Collections.Generic;

/// <summary>
/// DynamicObstaclesSpawnerGroupConfigSO を展開した実行時用のエントリ。
/// - グループの難易度
/// - グループに含まれる DynamicObstaclesSpawnerConfig のリスト
/// を保持する。
/// </summary>
public class DynamicObstaclesGroupEntry
{
    public byte difficulty;
    public List<DynamicObstaclesSpawnerConfig> obstacleConfigs;

    public DynamicObstaclesGroupEntry(byte difficulty, List<DynamicObstaclesSpawnerConfig> obstacleConfigs)
    {
        this.difficulty = difficulty;
        this.obstacleConfigs = obstacleConfigs;
    }
}
