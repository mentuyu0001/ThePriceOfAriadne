using Parts.Types;
using UnityEngine;
using VContainer; 

/// <summary>
/// プレイヤーが装備しているパーツとマップに落ちているパーツを交換するスクリプト
/// </summary>

public class PartsManager : MonoBehaviour
{
    // PlayerCustomizerの参照
    [Inject] private PlayerCustomizer playerCustomizer;

    // プレイヤーが装備しているパーツをマップに落ちているパーツと交換するメソッド
    public void ExchangeParts(MapParts mapParts)
    {
        Debug.Log("交換前のマップパーツの種類: " + mapParts.SlotType + ", " + mapParts.CharaType);

        // マップに落ちているパーツと交換し，装備していたパーツを取得
        PartsChara befChara = playerCustomizer.ChangePlayerParts(mapParts.SlotType, mapParts.CharaType);

        // マップに落ちているパーツの種類を，装備していたパーツの種類に変更
        mapParts.CharaType = befChara;

        Debug.Log("交換後のマップパーツの種類: " + mapParts.SlotType + ", " + mapParts.CharaType);
    }
}
