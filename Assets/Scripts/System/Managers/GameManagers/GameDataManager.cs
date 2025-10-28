using UnityEngine;
using System.IO; // ファイルの読み書きに必要
using VContainer;
using Parts.Types;

// セーブするデータのクラス
// [System.Serializable] をつけないとJsonUtilityで変換できない
[System.Serializable]
public class SaveData
{
    /// パーツの種類を保存する変数
    public PartsChara LeftArm;
    public PartsChara RightArm;
    public PartsChara LeftLeg;
    public PartsChara RightLeg;

    // アイテムの取得状況を保存する変数

    /*
    public bool playerReportObtained;
    public bool theifReportObtained;
    public bool muscleReportObtained;
    public bool fireReportObtained;
    public bool assassinReportObtained;
    public bool playerDiaryObtained;
    public bool theifDiaryObtained;
    public bool muscleDiaryObtained;
    public bool fireDiaryObtained;
    public bool assassinDiaryObtained;
    */

    // ゲームの進行状況を保存する変数
    public int stageNumber;
}

public class GameDataManager : MonoBehaviour
{
    [Inject] private PlayerParts playerParts; // プレイヤーのパーツ情報を保持するシングルトン
    [Inject] private InventoryData inventoryData; // インベントリのデータを保持
    public SaveData saveData; // セーブデータを保持するインスタンス
    private string saveFilePath; // セーブファイルのパス

    private bool isInitialized = false;
    private int currentSlot = 1; // デフォルトは1番スロット

    void Awake()
    {
        // セーブファイルのパスを決定する（OSごとに適切な場所を指定してくれる）
        saveFilePath = Application.persistentDataPath + "/save.json";
    }

    void Start()
    {
        Debug.Log("playerParts: " + playerParts);
        Debug.Log("inventoryData: " + inventoryData);
        // 依存関係の注入確認
        if (playerParts == null || inventoryData == null)
        {
            Debug.LogError("GameDataManager: 必要な依存関係が注入されていません");
            return;
        }
        
        isInitialized = true;
    }

    // スロット番号をセットするメソッド
    public void SetCurrentSlot(int saveSlotNumber)
    {
        currentSlot = saveSlotNumber;
        saveFilePath = Application.persistentDataPath + $"/save_{currentSlot}.json";
    }

    // ゲームをセーブするメソッド
    public void SaveGame()
    {
        if (!isInitialized)
        {
            Debug.LogError("GameDataManager: まだ初期化されていません");
            return;
        }

        // saveDataを初期化
        if (saveData == null)
        {
            saveData = new SaveData();
        }

        // 今のゲームの状態をsaveDataインスタンスに設定する
        saveData.LeftArm = playerParts.LeftArm;
        saveData.RightArm = playerParts.RightArm;
        saveData.LeftLeg = playerParts.LeftLeg;
        saveData.RightLeg = playerParts.RightLeg;
        saveData.stageNumber = 1; //仮に1を設定

        /*
        saveData.playerReportObtained = inventoryData.PlayerReportObtained;
        saveData.theifReportObtained = inventoryData.TheifReportObtained;
        saveData.muscleReportObtained = inventoryData.MuscleReportObtained;
        saveData.fireReportObtained = inventoryData.FireReportObtained;
        saveData.assassinReportObtained = inventoryData.AssassinReportObtained;
        saveData.playerDiaryObtained = inventoryData.PlayerDiaryObtained;
        saveData.theifDiaryObtained = inventoryData.TheifDiaryObtained;
        saveData.muscleDiaryObtained = inventoryData.MuscleDiaryObtained;
        saveData.fireDiaryObtained = inventoryData.FireDiaryObtained;
        saveData.assassinDiaryObtained = inventoryData.AssassinDiaryObtained;
        */
        // ... 他のデータも同様に設定   

        // SaveDataをJSON形式の文字列に変換
        string json = JsonUtility.ToJson(saveData);

        // JSON文字列をファイルに書き込む
        File.WriteAllText(saveFilePath, json);

        Debug.Log("セーブしました: " + saveFilePath);
    }

    // ゲームをロードするメソッド
    public void LoadGame()
    {
        if (!isInitialized)
        {
            Debug.LogError("GameDataManager: まだ初期化されていません");
            return;
        }

        // セーブファイルが存在するかチェック
        if (File.Exists(saveFilePath))
        {
            // ファイルからJSON文字列を読み込む
            string json = File.ReadAllText(saveFilePath);

            // JSON文字列をSaveDataクラスのインスタンスに変換
            saveData = JsonUtility.FromJson<SaveData>(json);

            Debug.Log("ロードしました");

            // --- ロードしたデータをゲームに反映させる処理 ---
            playerParts.LeftArm = saveData.LeftArm;
            playerParts.RightArm = saveData.RightArm;
            playerParts.LeftLeg = saveData.LeftLeg;
            playerParts.RightLeg = saveData.RightLeg;
            // = saveData.stageNumber;// ゲームの進行状況も反映

            Debug.Log("Loaded Parts - LeftArm: " + playerParts.LeftArm + ", RightArm: " + playerParts.RightArm +
                      ", LeftLeg: " + playerParts.LeftLeg + ", RightLeg: " + playerParts.RightLeg);

            /*
            inventoryData.PlayerReportObtained = saveData.playerReportObtained;
            inventoryData.TheifReportObtained = saveData.theifReportObtained;
            inventoryData.MuscleReportObtained = saveData.muscleReportObtained;
            inventoryData.FireReportObtained = saveData.fireReportObtained;
            inventoryData.AssassinReportObtained = saveData.assassinReportObtained;
            inventoryData.PlayerDiaryObtained = saveData.playerDiaryObtained;
            inventoryData.TheifDiaryObtained = saveData.theifDiaryObtained;
            inventoryData.MuscleDiaryObtained = saveData.muscleDiaryObtained;
            inventoryData.FireDiaryObtained = saveData.fireDiaryObtained;
            inventoryData.AssassinDiaryObtained = saveData.assassinDiaryObtained;
            */
        }
        else
        {
            Debug.LogWarning("セーブファイルが見つかりません。");
            // セーブファイルがない場合、新しいデータで初期化する
            saveData = new SaveData();
        }
    }
}