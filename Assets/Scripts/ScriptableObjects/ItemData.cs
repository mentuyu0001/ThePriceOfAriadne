using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// アイテムの情報を保持するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/Item/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("アイテム一覧")]
    [SerializeField] private List<Item> items = new List<Item>();

    // アイテムIDに基づいてアイテム情報を取得するメソッド
    public Item GetItemByID(int itemID)
    {
        return items.Find(item => item.id == itemID);
    }

    // アイテムIDに基づいてアイテム名を取得するメソッド
    public string GetItemNameByID(int itemID)
    {
        var item = GetItemByID(itemID);
        return item != null ? item.name : "Unknown Item";
    }

    // アイテムIDに基づいてアイテムテキストを取得するメソッド
    public string GetItemTextByID(int itemID)
    {
        var item = GetItemByID(itemID);
        return item != null ? item.text : "Unknown Item Text";
    }

    // アイテムIDに基づいて説明文を取得するメソッド
    public ItemDescriptions GetItemDescriptionsByID(int itemID)
    {
        var item = GetItemByID(itemID);
        return item?.descriptions;
    }

    // パーツタイプとアイテム所有者が一致するかチェック
    private bool IsOwnerMatch(string partsType, ItemOwnerType ownerType)
    {
        return partsType == ownerType.ToString() ||
               (partsType == "FireFighter" && ownerType == ItemOwnerType.Fire);
    }

    // パーツタイプに応じた口調の説明文を取得
    private string GetToneByPartsType(string partsType, ItemDescriptions descriptions)
    {
        return partsType switch
        {
            "Normal" => descriptions.playerTone ?? "",
            "Thief" => descriptions.theifTone ?? "",
            "Muscle" => descriptions.muscleTone ?? "",
            "FireFighter" => descriptions.fireTone ?? "",
            "Assassin" => descriptions.assassinTone ?? "",
            _ => descriptions.playerTone ?? "説明文がありません"
        };
    }
}

[Serializable]
public class Item
{
    [Header("基本情報")]
    public int id;                          // アイテムのID
    public string name;                     // アイテムの名前
    [TextArea(3, 5)]
    public string text;                     // アイテムの中身のテキスト
    
    [Header("所有者情報")]
    [Tooltip("このアイテムの元の所有者")]
    public ItemOwnerType ownerType = ItemOwnerType.Normal;     // アイテムの所有者
    
    [Header("説明文")]
    public ItemDescriptions descriptions;   // アイテムを説明するセリフ

    [Header("カテゴリ")]
    public ItemType itemType;              // アイテムの種類

    public Item(int id, string name, string text, ItemType itemType, ItemOwnerType ownerType = ItemOwnerType.Normal)
    {
        this.id = id;
        this.name = name;
        this.text = text;
        this.itemType = itemType;
        this.ownerType = ownerType;
        this.descriptions = new ItemDescriptions();
    }
}

[Serializable]
public enum ItemType
{
    ResearchReport,  // 研究報告書
    Diary           // 日記
}

[Serializable]
public enum ItemOwnerType
{
    Normal,         // プレイヤー
    Thief,          // 泥棒
    Muscle,         // マッチョ
    Fire,           // 消防士
    Assassin        // アサシン
}

[Serializable]
public class ItemDescriptions
{
    [Header("キャラクター別口調")]
    [Tooltip("プレイヤー口調での説明")]
    public string playerTone;

    [Tooltip("泥棒口調での説明")]
    public string theifTone;

    [Tooltip("マッチョ口調での説明")]
    public string muscleTone;

    [Tooltip("消防士口調での説明")]
    public string fireTone;

    [Tooltip("アサシン口調での説明")]
    public string assassinTone;

    [Header("パーツ占有率別")]
    [Tooltip("各パーツ占有率25%のときの説明")]
    public string allQuartersTone;

    [Tooltip("自身のパーツ占有率100%のときの説明")]
    public string ownFullTone;
}
