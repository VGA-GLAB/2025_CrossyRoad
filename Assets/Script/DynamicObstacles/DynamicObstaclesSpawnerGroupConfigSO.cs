using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 複数の動的障害物スポナー設定をまとめたグループを ScriptableObject として保持するアセット。
/// 
/// - Unity エディタ上でインスペクタから編集可能な「設定ファイル」として利用する。
/// - 1連・2連・3連など、複数の <see cref="DynamicObstaclesSpawnerConfigSO"/> を束ねて
///   ひとつの「動的障害物パターン」として保存できる。
/// - グループ単位で「難易度」を付与できるため、
///   ステージ進行に応じて難易度に合った動的障害物パターンを選択することが可能。
/// - ステージ自動生成時には、この ScriptableObject をロードし、
///   内包する各 <see cref="DynamicObstaclesSpawnerConfigSO"/> を実行時専用の
///   <see cref="DynamicObstaclesSpawnerConfig"/> に変換して利用する。
/// 
/// つまり、エディタで作成・保存する「動的障害物パターングループの編集用データ」と、
/// 実行時に参照する「読み取り専用データ」を分離するための仕組み。
/// </summary>
[CreateAssetMenu(menuName = "Stage/DynamicObstaclesSpawnerGroupConfig")]
public class DynamicObstaclesSpawnerGroupConfigSO : ScriptableObject
{
    [Tooltip("このグループに含まれる障害物の設定（1連〜複数連）")]
    public List<DynamicObstaclesSpawnerConfigSO> dynamicObstaclesConfigs;

    [Tooltip("このグループ全体の難易度レベル")]
    public byte difficultyAppear;       // このグループが登場する進行度(0～255の範囲)
    public byte difficultyCost;         // このギミックのコスト(0～255の範囲)
}
