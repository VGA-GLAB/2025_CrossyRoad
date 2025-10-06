using UnityEngine;

/// <summary>
/// ステージ生成のテスト用ドライバ。
/// 固定ルールで StageData を生成する。
/// PhaseBeta後にステージ自動生成に差し替え予定。
/// </summary>
public class StageGenerationTestDriver
{
    /// <summary>
    /// テスト用のステージデータを生成して返す。
    /// - 幅20, 奥行き100
    /// - XZ両方向の外周は Empty
    /// - Grass レーンにはランダムで Tree を配置
    /// - Road レーンは一律 Road
    /// - River レーンは一律 River
    /// </summary>
    public StageData GenerateTestStage()
    {
        StageData data = new StageData();
        data.width = 20;
        data.depth = 100;

        // レーンごとの地形を決定
        for (int z = 0; z < data.depth; z++)
        {
            // Z方向の外周は Empty
            if (z == 0 || z == data.depth - 1)
            {
                data.laneTypes[z] = GridManager.CellType.Empty;
                continue;
            }

            // サンプルルール: 3レーンごとに Grass, Road, River を繰り返す
            if (z % 3 == 0)
                data.laneTypes[z] = GridManager.CellType.Grass;
            else if (z % 3 == 1)
                data.laneTypes[z] = GridManager.CellType.Road;
            else
                data.laneTypes[z] = GridManager.CellType.River;
        }

        // Grass レーンにランダムで Tree を配置
        for (int z = 1; z < data.depth - 1; z++)
        {
            if (data.laneTypes[z] == GridManager.CellType.Grass)
            {
                for (int x = 0; x < data.width; x++)
                {
                    // 20% の確率で木を配置
                    if (Random.value < 0.2f)
                    {
                        // Note: y座標は1固定（地形の上に置く想定）
                        // マップ自動生成や動的障害物配置の際に調整したりY軸をどうするか規定する必要あり
                        Vector3Int pos = new Vector3Int(x, 1, z);
                        data.staticObstacles[pos] = ObstacleType.Tree;
                    }
                }
            }
        }

        return data;
    }
}
