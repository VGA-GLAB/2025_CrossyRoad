using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 橋スポナーの設定を ScriptableObject として保持するアセット。
/// 
/// - Unity エディタ上でインスペクタから編集可能な「設定ファイル」として利用する。
/// - プレハブアセット（スポナー本体や橋のプレハブ群）を紐付け、
///   生成間隔や橋の数などのパラメータをパターン化して保存できる。
/// - ステージ自動生成時には、この ScriptableObject をロードし、
///   実行時専用の <see cref="BridgeSpawnerConfig"/> に変換して利用する。
/// 
/// つまり、エディタで作成・保存する「編集用データ」と、
/// 実行時に参照する「読み取り専用データ」を分離するための仕組み。
/// </summary>
[CreateAssetMenu(fileName = "BridgeSpawnerConfigSO", menuName = "Stage/BridgeSpawnerConfig")]
public class BridgeSpawnerConfigSO : ScriptableObject
{
    [Header("スポナーを配置するグリッド上の座標")]
    public Vector3Int position;

    [Header("スポナー本体のプレハブ (BridgeSpawner など)")]
    public GameObject spawnerControllerPrefab;

    [Header("橋のプレハブ群（長さや形状のバリエーションを登録）")]
    public List<GameObject> bridgePrefabs;

    [Header("橋の生成間隔（秒）")]
    public float spawnInterval;

    [Header("1レーン内で橋を並べる間隔（秒）")]
    public float bridgeInterval;

    [Header("1レーンに生成する橋の数")]
    public int bridgeCountPerLane;

    [Header("進行方向")]
    public bool moveRight;

    /// <summary>
    /// ScriptableObject に保存された値をもとに、
    /// 実行時専用の <see cref="BridgeSpawnerConfig"/> を生成する。
    /// 
    /// - ステージ自動生成コードはこのメソッドを通じて Config を取得する。
    /// - Config は読み取り専用の不変オブジェクトとして扱われるため、
    ///   実行中に値が書き換わることを防げる。
    /// - また、この変換処理は「BridgeSpawnerConfig と SO の項目が一致しているか」
    ///   をコンパイル時にチェックする役割も兼ねている。
    ///   （項目が増減した場合、ここでコンパイルエラーとなり気付ける）
    /// </summary>
    public BridgeSpawnerConfig ToRuntimeConfig()
    {
        return new BridgeSpawnerConfig(
            position,
            spawnerControllerPrefab,
            bridgePrefabs,
            spawnInterval,
            bridgeInterval,
            bridgeCountPerLane,
            moveRight
        );
    }
}
