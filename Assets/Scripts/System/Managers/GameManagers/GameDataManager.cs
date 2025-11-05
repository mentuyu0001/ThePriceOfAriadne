using UnityEngine;
using System.IO; // ファイルの読み書きに必要
using VContainer;
using Parts.Types;
using UnityEngine.SceneManagement;

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
    private PlayerParts playerParts; // プレイヤーのパーツ情報を保持するシングルトン
    [Inject] private InventoryData inventoryData; // インベントリのデータを保持
    [Inject] private StageNumber stageNumber; // 現在のステージを管理するシングルトン
    [Inject] private GameSceneManager sceneManager; // GameSceneManagerのインスタンス
    [Inject] private PlayerCustomizer playerCustomizer; // PlayerCustomizerのインスタンス
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
        playerParts = GameObject.Find ("PlayerParts").GetComponent<PlayerParts>();
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
    public void SaveGame(int slot)
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
        saveData.stageNumber = stageNumber.GetCurrentStage();

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
        SetCurrentSlot(slot);
        File.WriteAllText(saveFilePath, json);

        Debug.Log("セーブしました: " + saveFilePath);
    }

    // ゲームをロードするメソッド
    public void LoadGame(int slot)
    {
        if (!isInitialized)
        {
            Debug.LogError("GameDataManager: まだ初期化されていません");
            return;
        }

        // playerCustomizerのnullチェックを追加
        if (playerCustomizer == null)
        {
            Debug.LogError("GameDataManager: PlayerCustomizerが注入されていません");
            return;
        }

        SetCurrentSlot(slot);

        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                saveData = JsonUtility.FromJson<SaveData>(json);
                Debug.Log("セーブデータをロードしました");

                // パーツ変更処理
                try
                {
                    playerCustomizer.LoadPlayerParts(PartsSlot.LeftArm, saveData.LeftArm);
                    playerCustomizer.LoadPlayerParts(PartsSlot.RightArm, saveData.RightArm);
                    playerCustomizer.LoadPlayerParts(PartsSlot.LeftLeg, saveData.LeftLeg);
                    playerCustomizer.LoadPlayerParts(PartsSlot.RightLeg, saveData.RightLeg);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"パーツロード中にエラーが発生: {e.Message}\n{e.StackTrace}");
                    return;
                }

                // ステージへ移動
                sceneManager.LoadStage(saveData.stageNumber);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ロード処理中にエラーが発生: {e.Message}\n{e.StackTrace}");
                return;
            }
        }
        else
        {
            Debug.LogWarning($"セーブファイルが見つかりません: {saveFilePath}");
            saveData = new SaveData();
        }
    }
}