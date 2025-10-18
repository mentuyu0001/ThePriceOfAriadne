using UnityEngine;
using System;
using System.Collections.Generic;

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

    // オブジェクトIDとキャラクタータイプに基づいてテキストを取得するメソッド
    public string GetTextByIDAndCharacter(int objectID, Parts.Types.PartsChara chara)
    {
        var objectText = GetObjectTextByID(objectID);
        if (objectText == null) return "Unknown Object";

        return chara switch
        {
            Parts.Types.PartsChara.Normal => objectText.characterDialogues.playerTone,
            Parts.Types.PartsChara.Theif => objectText.characterDialogues.thiefTone,
            Parts.Types.PartsChara.Muscle => objectText.characterDialogues.soldierTone,
            Parts.Types.PartsChara.Fire => objectText.characterDialogues.firefighterTone,
            Parts.Types.PartsChara.Assassin => objectText.characterDialogues.assassinTone,
            _ => "Unknown Character"
        };
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
}
