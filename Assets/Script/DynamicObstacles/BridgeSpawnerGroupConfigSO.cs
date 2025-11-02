using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 複数の橋スポナー設定をまとめたグループを ScriptableObject として保持するアセット。
/// 
/// - Unity エディタ上でインスペクタから編集可能な「設定ファイル」として利用する。
/// - 1連橋・2連橋・3連橋など、複数の <see cref="BridgeSpawnerConfigSO"/> を束ねて
///   ひとつの「橋パターン」として保存できる。
/// - グループ単位で「難易度」を付与できるため、
///   ステージ進行に応じて難易度に合った橋パターンを選択することが可能。
/// - ステージ自動生成時には、この ScriptableObject をロードし、
///   内包する各 <see cref="BridgeSpawnerConfigSO"/> を実行時専用の
///   <see cref="BridgeSpawnerConfig"/> に変換して利用する。
/// 
/// つまり、エディタで作成・保存する「橋パターングループの編集用データ」と、
/// 実行時に参照する「読み取り専用データ」を分離するための仕組み。
/// </summary>
[CreateAssetMenu(menuName = "Stage/BridgeSpawnerGroupConfig")]
public class BridgeSpawnerGroupConfigSO : ScriptableObject
{
    [Tooltip("このグループに含まれる橋の設定（1連〜複数連）")]
    public List<BridgeSpawnerConfigSO> bridgeConfigs;

    [Tooltip("このグループ全体の難易度レベル")]
    public byte difficulty;         // 難易度は0～255の範囲
}
