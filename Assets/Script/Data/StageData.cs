using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ全体のデータ構造。
/// - マップサイズ（幅・奥行き）
/// - 地形レイヤー（レーン単位）
/// - 静的障害物レイヤー（セル単位）
/// </summary>
public class StageData
{
    public int width;   // マップの横幅（x方向）
    public int depth;   // マップの奥行き（z方向）

    /// <summary>
    /// 地形レイヤー
    /// key: z座標（レーン番号）
    /// value: CellType（Grass, Road, River, Emptyなど）
    /// </summary>
    public Dictionary<int, CellType> laneTypes
        = new Dictionary<int, CellType>();

    /// <summary>
    /// 静的障害物レイヤー
    /// key: グリッド座標
    /// value: 障害物の種類
    /// </summary>
    public Dictionary<Vector3Int, ObstacleType> staticObstacles
        = new Dictionary<Vector3Int, ObstacleType>();

    /// <summary>
    /// スポナー設定リスト。
    /// - 各スポナーの位置や生成対象を保持する。
    /// - GridManager が解釈してシーンにスポナーを配置する。
    /// - BridgeSpawnerConfig など、SpawnerConfigBase を継承した型を格納。
    /// </summary>
    public List<SpawnerConfigBase> spawnerConfigs
        = new List<SpawnerConfigBase>();
}
