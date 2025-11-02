using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BridgeSpawnerGroupConfigSO を展開した実行時用のエントリ。
/// - グループの難易度
/// - グループに含まれる BridgeSpawnerConfig のリスト
/// を保持する。
/// </summary>
public class BridgeSpawnerGroupEntry
{
    public byte difficulty;
    public List<BridgeSpawnerConfig> bridgeConfigs;

    public BridgeSpawnerGroupEntry(byte difficulty, List<BridgeSpawnerConfig> bridgeConfigs)
    {
        this.difficulty = difficulty;
        this.bridgeConfigs = bridgeConfigs;
    }
}
