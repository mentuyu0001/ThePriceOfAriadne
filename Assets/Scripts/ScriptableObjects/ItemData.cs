using UnityEngine;
using System;
using System.Collections.Generic;

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

    // すべてのアイテムを取得するメソッド
    public List<Item> GetAllItems()
    {
        return new List<Item>(items);
    }

    // アイテムを追加するメソッド（エディタ用）
    public void AddItem(Item item)
    {
        if (items.Find(x => x.id == item.id) == null)
        {
            items.Add(item);
        }
        else
        {
            Debug.LogWarning($"ID {item.id} のアイテムは既に存在します。");
        }
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
    
    [Header("説明文")]
    public ItemDescriptions descriptions;   // アイテムを説明するセリフ

    [Header("カテゴリ")]
    public ItemType itemType;              // アイテムの種類

    public Item(int id, string name, string text, ItemType itemType)
    {
        this.id = id;
        this.name = name;
        this.text = text;
        this.itemType = itemType;
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

    [Tooltip("自身のパーツ占有率75%のときの説明")]
    public string ownThreeQuartersTone;

    [Tooltip("自身のパーツ占有率100%のときの説明")]
    public string ownFullTone;
}
