using UnityEngine;
using Parts.Types;

/// <summary>
/// マップ上のパーツの種類を保持するスクリプト
/// </summary>

public class MapParts : MonoBehaviour
{
    // どの部位か
    [SerializeField] private PartsSlot slotType;
    // どのキャラのパーツなのか
    [SerializeField] private PartsChara charaType;

    // どの部位かを取得するプロパティ(read-only)
    public PartsSlot SlotType
    {
        get { return slotType; }
    }

    // どのキャラのパーツかを取得するプロパティ
    public PartsChara CharaType
    {
        get { return charaType; }
        set { charaType = value; }
    }
}
