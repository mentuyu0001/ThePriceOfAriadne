using UnityEngine;
using System;
using System.Collections.Generic;
using Parts.Types;

/// <summary>
/// パーツの形容詞と説明を保持するスクリプタブルオブジェクト
/// </summary>

[CreateAssetMenu(fileName = "PartsData", menuName = "ScriptableObject/Parts/PartsData")]

public class PartsData : ScriptableObject
{
    [Header("パーツの種類一覧")]
    [SerializeField] private List<PartsInfo> parts = new List<PartsInfo>();

    // アイテムIDに基づいてアイテム情報を取得するメソッド
    public PartsInfo GetPartsInfoByPartsChara(PartsChara chara)
    {
        return parts.Find(p => p.ownerType == chara);
    }
}

[Serializable]
public class PartsInfo
{
    [Header("所有者情報")]
    [Tooltip("このアイテムの元の所有者")]
    public PartsChara ownerType = PartsChara.Normal;     // アイテムの所有者

    [Header("パーツの形容詞")]
    public string adjective;

    [Header("パーツの説明")]
    [TextArea(3, 5)]
    public string descriptionArm;
    [TextArea(3, 5)]
    public string descriptionLeg;

    public PartsInfo(string adjective, string descriptionArm, string descriptionLeg,  PartsChara ownerType = PartsChara.Normal)
    {
        this.adjective = adjective;
        this.descriptionArm = descriptionArm;
        this.descriptionLeg = descriptionLeg;
        this.ownerType = ownerType;
    }
}