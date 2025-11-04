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
    public byte difficultyAppear;
    public byte difficultyCost;
    public List<BridgeSpawnerConfig> bridgeConfigs;

    public BridgeSpawnerGroupEntry(byte difficultyAppear, byte difficultyCost, List<BridgeSpawnerConfig> bridgeConfigs)
    {
        this.difficultyAppear = difficultyAppear;
        this.difficultyCost = difficultyCost;
        this.bridgeConfigs = bridgeConfigs;
    }
}
