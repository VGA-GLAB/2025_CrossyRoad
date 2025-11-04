using UnityEngine;

/// <summary>
/// スポナー全体で共通利用する定数をまとめたクラス。
/// </summary>
public static class SpawnerConstants
{
    /// <summary>
    /// スポナーの基準となる最小単位の生成間隔（秒）
    /// 各スポナーはこの値の整数倍を使用することで生成タイミングを同期させる。
    /// 設定値は「1チャンク内のすべてのスポナーが一巡して生成を終える時間」を
    /// 基準に十分に小さい値を選ぶことを推奨。
    /// </summary>
    public const float BaseInterval = 0.05f;
}
