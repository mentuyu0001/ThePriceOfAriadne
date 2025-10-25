using UnityEngine;
using System;
using System.Collections.Generic;
using Parts.Types;

/// <summary>
/// オブジェクトインタラクション時のテキスト情報を保持するスクリプタブルオブジェクト
/// </summary>
[CreateAssetMenu(fileName = "ObjectTextData", menuName = "ScriptableObject/Object/ObjectTextData")]
public class ObjectTextData : ScriptableObject
{
    [Header("オブジェクトテキスト一覧")]
    [SerializeField] private List<ObjectText> objectTexts = new List<ObjectText>();

    // オブジェクトIDに基づいてテキスト情報を取得するメソッド
    public ObjectText GetObjectTextByID(int objectID)
    {
        return objectTexts.Find(obj => obj.id == objectID);
    }

    // PartsChara型で統一
    public string GetTextByIDAndCharacter(int objectID, PartsChara chara)
    {
        var objectText = GetObjectTextByID(objectID);
        if (objectText == null) return "Unknown Object";

        return chara switch
        {
            PartsChara.Normal => objectText.characterDialogues.playerTone,
            PartsChara.Thief => objectText.characterDialogues.thiefTone,
            PartsChara.Muscle => objectText.characterDialogues.soldierTone,
            PartsChara.Fire => objectText.characterDialogues.firefighterTone,
            PartsChara.Assassin => objectText.characterDialogues.assassinTone,
            _ => "Unknown Character"
        };
    }
    // キメラ口調のテキストを取得するメソッド
    public string GetChimeraToneByID(int objectID)
    {
        var objectText = GetObjectTextByID(objectID);
        if (objectText == null) return "Unknown Object";
        return objectText.characterDialogues.allQuartersTone;
    }   

    // すべてのオブジェクトテキストを取得するメソッド
    public List<ObjectText> GetAllObjectTexts()
    {
        return new List<ObjectText>(objectTexts);
    }

    // オブジェクトテキストを追加するメソッド（エディタ用）
    public void AddObjectText(ObjectText objectText)
    {
        if (objectTexts.Find(x => x.id == objectText.id) == null)
        {
            objectTexts.Add(objectText);
        }
        else
        {
            Debug.LogWarning($"ID {objectText.id} のオブジェクトテキストは既に存在します。");
        }
    }
}

[Serializable]
public class ObjectText
{
    [Header("基本情報")]
    public int id;
    public string objectName;
    
    [Header("オブジェクトタイプ")]
    public ObjectType objectType;

    [Header("キャラクター別セリフ")]
    public CharacterDialogues characterDialogues;

    public ObjectText(int id, string objectName, ObjectType objectType)
    {
        this.id = id;
        this.objectName = objectName;
        this.objectType = objectType;
        this.characterDialogues = new CharacterDialogues();
    }
}

[Serializable]
public enum ObjectType
{
    WaterTank,
    BurningFire,
    EmberFire,
    HeabyObject,
    Door,
    Other
}

[Serializable]
public class CharacterDialogues
{
    [Header("プレイヤー口調")]
    [TextArea(2, 4)]
    [Tooltip("50%～75%の時のセリフ")]
    public string playerTone;

    [Header("泥棒口調")]
    [TextArea(2, 4)]
    [Tooltip("50%～75%の時のセリフ")]
    public string thiefTone;
    
    [Header("軍人口調")]
    [TextArea(2, 4)]
    [Tooltip("50%～75%の時のセリフ")]
    public string soldierTone;
    
    [Header("消防士口調")]
    [TextArea(2, 4)]
    [Tooltip("50%～75%の時のセリフ")]
    public string firefighterTone;

    [Header("アサシン口調")]
    [TextArea(2, 4)]
    [Tooltip("50%～75%の時のセリフ")]
    public string assassinTone;
    [Header("キメラ口調")]
    [TextArea(2, 4)]
    [Tooltip("All25%の時のセリフ")]
    public string allQuartersTone;
}
