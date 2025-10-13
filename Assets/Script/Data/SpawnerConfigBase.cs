using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スポナー設定の基底クラス。
/// StageData に格納され、GridManager が解釈して Spawner を生成する。
/// データのみを保持し、ロジックは持たない。
/// </summary>
public abstract class SpawnerConfigBase
{
    /// <summary>
    /// スポナーを配置するグリッド上の座標。
    /// </summary>
    public Vector3Int Position { get; }

    /// <summary>
    /// スポナー本体のプレハブ。
    /// MonoBehaviour (例: BridgeSpawner, CarSpawner) をアタッチした空オブジェクトを想定。
    /// GridManager が Instantiate してシーンに配置する。
    /// </summary>
    public GameObject SpawnerControllerPrefab { get; }

    /// <summary>
    /// スポナーが生成する対象のプレハブ。
    /// 例: 橋、車、敵など。必ず何かを出す前提で基底に宣言。
    /// </summary>
    public IReadOnlyList<GameObject> SpawnTargetPrefabs { get; }

    /// <summary>
    /// コンストラクタ。
    /// Config は作成後に値を変更しないため、生成時にすべての値を設定する。
    /// </summary>
    protected SpawnerConfigBase(
        Vector3Int position,
        GameObject spawnerControllerPrefab,
        IReadOnlyList<GameObject> spawnTargetPrefabs)
    {
        Position = position;
        SpawnerControllerPrefab = spawnerControllerPrefab;
        SpawnTargetPrefabs = spawnTargetPrefabs;
    }
}
