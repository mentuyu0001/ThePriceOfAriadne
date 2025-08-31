using UnityEngine;
using System;

/// <summary>
/// アイテムの情報を保持するスクリプタブルオブジェクト
/// </summary>

[CreateAssetMenu(fileName = "ItemData", menuName = "ScriptableObject/Item/ItemData")]
public class ItemData : ScriptableObject
{
    // 少女の研究報告書
    [Header("少女の研究報告書")]
    [SerializeField] public int playerReportID; // アイテムのID
    [SerializeField] public string playerReportName; // アイテムの名前
    [SerializeField] public ItemDescriptions playerReportDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）

    [SerializeField] public string playerReportText; // アイテムの中身のテキスト  

    // 泥棒の研究報告書
    [Header("泥棒の研究報告書")]
    [SerializeField] public int theifReportID; // アイテムのID
    [SerializeField] public string theifReportName; // アイテムの名前
    [SerializeField] public ItemDescriptions theifReportDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string theifReportText; // アイテムの中身のテキスト

    // マッチョの研究報告書
    [Header("マッチョの研究報告書")]
    [SerializeField] public int muscleReportID; // アイテムのID
    [SerializeField] public string muscleReportName; // アイテムの名前              
    [SerializeField] public ItemDescriptions muscleReportDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string muscleReportText; // アイテムの中身のテキスト

    // 消防士の研究報告書
    [Header("消防士の研究報告書")]
    [SerializeField] public int fireReportID; // アイテムのID
    [SerializeField] public string fireReportName; // アイテムの名前
    [SerializeField] public ItemDescriptions fireReportDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string fireReportText; // アイテムの中身のテキスト

    // アサシンの研究報告書
    [Header("アサシンの研究報告書")]
    [SerializeField] public int assassinReportID; // アイテムのID
    [SerializeField] public string assassinReportName; // アイテムの名前
    [SerializeField] public ItemDescriptions assassinReportDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string assassinReportText; // アイテムの中身のテキスト

    // 少女の日記
    [Header("少女の日記")]
    [SerializeField] public int playerDiaryID; // アイテムのID
    [SerializeField] public string playerDiaryName; // アイテムの名前
    [SerializeField] public ItemDescriptions playerDiaryDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string playerDiaryText; // アイテムの中身のテキスト     
    
    // 泥棒の日記
    [Header("泥棒の日記")]
    [SerializeField] public int theifDiaryID; // アイテムのID
    [SerializeField] public string theifDiaryName; // アイテムの名前
    [SerializeField] public ItemDescriptions theifDiaryDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string theifDiaryText; // アイテムの中身のテキスト
    
    // マッチョの日記
    [Header("マッチョの日記")]
    [SerializeField] public int muscleDiaryID; // アイテムのID
    [SerializeField] public string muscleDiaryName; // アイテムの名前
    [SerializeField] public ItemDescriptions muscleDiaryDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string muscleDiaryText; // アイテムの中身のテキスト
    
    // 消防士の日記
    [Header("消防士の日記")]
    [SerializeField] public int fireDiaryID; // アイテムのID
    [SerializeField] public string fireDiaryName; // アイテムの名前
    [SerializeField] public ItemDescriptions fireDiaryDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string fireDiaryText; // アイテムの中身のテキスト
    
    // アサシンの日記
    [Header("アサシンの日記")]
    [SerializeField] public int assassinDiaryID; // アイテムのID
    [SerializeField] public string assassinDiaryName; // アイテムの名前
    [SerializeField] public ItemDescriptions assassinDiaryDescriptions; // アイテムを説明するセリフ（各キャラクターの口調パターンを用意）
    [SerializeField] public string assassinDiaryText; // アイテムの中身のテキスト

    // --- プロパティ ---   
    public int PlayerReportID => playerReportID;
    public string PlayerReportName => playerReportName;
    public ItemDescriptions PlayerReportDescriptions => playerReportDescriptions;
    public string PlayerReportText => playerReportText;
    public int TheifReportID => theifReportID;
    public string TheifReportName => theifReportName;
    public ItemDescriptions TheifReportDescriptions => theifReportDescriptions;
    public string TheifReportText => theifReportText;
    public int MuscleReportID => muscleReportID;
    public string MuscleReportName => muscleReportName;
    public ItemDescriptions MuscleReportDescriptions => muscleReportDescriptions;
    public string MuscleReportText => muscleReportText;
    public int FireReportID => fireReportID;
    public string FireReportName => fireReportName;
    public ItemDescriptions FireReportDescriptions => fireReportDescriptions;
    public string FireReportText => fireReportText;
    public int AssassinReportID => assassinReportID;
    public string AssassinReportName => assassinReportName;
    public ItemDescriptions AssassinReportDescriptions => assassinReportDescriptions;
    public string AssassinReportText => assassinReportText;       
    public int PlayerDiaryID => playerDiaryID;
    public string PlayerDiaryName => playerDiaryName;
    public ItemDescriptions PlayerDiaryDescriptions => playerDiaryDescriptions;
    public string PlayerDiaryText => playerDiaryText;
    
    public int TheifDiaryID => theifDiaryID;
    public string TheifDiaryName => theifDiaryName;
    public ItemDescriptions TheifDiaryDescriptions => theifDiaryDescriptions;
    public string TheifDiaryText => theifDiaryText;
    
    public int MuscleDiaryID => muscleDiaryID;
    public string MuscleDiaryName => muscleDiaryName;
    public ItemDescriptions MuscleDiaryDescriptions => muscleDiaryDescriptions;
    public string MuscleDiaryText => muscleDiaryText;
    
    public int FireDiaryID => fireDiaryID;
    public string FireDiaryName => fireDiaryName;
    public ItemDescriptions FireDiaryDescriptions => fireDiaryDescriptions;
    public string FireDiaryText => fireDiaryText;
    
    public int AssassinDiaryID => assassinDiaryID;
    public string AssassinDiaryName => assassinDiaryName;
    public ItemDescriptions AssassinDiaryDescriptions => assassinDiaryDescriptions;
    public string AssassinDiaryText => assassinDiaryText;
}

[Serializable]
public class ItemDescriptions
{
    [Tooltip("プレイヤー視点での説明")]
    public string playerTone; 
    
    [Tooltip("泥棒視点での説明")]
    public string theifTone;
    
    [Tooltip("マッチョ視点での説明")]
    public string muscleTone;
    
    [Tooltip("消防士視点での説明")]
    public string fireTone;
    
    [Tooltip("アサシン視点での説明")]
    public string assassinTone;
}
