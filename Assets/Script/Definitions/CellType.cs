/// <summary>
/// セルの種類を表す列挙型。
/// グリッド上の地形や状態を示すために使用する。
/// ゲーム仕様に応じて拡張する。
/// </summary>
public enum CellType
{
    Grass,      // 草原：通常の移動可能マス
    RoadGear,   // 道路：車が出現するレーン
    RoadRobot,  // 道路：敵ロボットが出現するレーン
    River,      // 川：丸太に乗っていなければ死亡
    Occupied,   // 静的障害物で埋まっている
    Empty       // 何もない
}
