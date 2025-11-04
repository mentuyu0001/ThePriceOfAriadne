using UnityEngine;

/// <summary>
/// インベントリのデータを保持するシングルトン
/// </summary>
public class InventoryData : MonoBehaviour
{
    public static InventoryData Instance { get; private set; }

    // 研究報告書の取得状態
    private bool playerReportObtained;
    private bool theifReportObtained;
    private bool muscleReportObtained;
    private bool fireReportObtained;
    private bool assassinReportObtained;

    // アイテムの取得状態
    private bool playerItemObtained = true;
    private bool theifItemObtained;
    private bool muscleItemObtained;
    private bool fireItemObtained;
    private bool assassinItemObtained;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 研究報告書の取得状態プロパティ
    public bool PlayerReportObtained { get => playerReportObtained; set => playerReportObtained = value; }
    public bool TheifReportObtained { get => theifReportObtained; set => theifReportObtained = value; }
    public bool MuscleReportObtained { get => muscleReportObtained; set => muscleReportObtained = value; }
    public bool FireReportObtained { get => fireReportObtained; set => fireReportObtained = value; }
    public bool AssassinReportObtained { get => assassinReportObtained; set => assassinReportObtained = value; }

    // アイテムの取得状態プロパティ
    public bool PlayerItemObtained { get => playerItemObtained; set => playerItemObtained = value; }
    public bool TheifItemObtained { get => theifItemObtained; set => theifItemObtained = value; }
    public bool MuscleItemObtained { get => muscleItemObtained; set => muscleItemObtained = value; }
    public bool FireItemObtained { get => fireItemObtained; set => fireItemObtained = value; }
    public bool AssassinItemObtained { get => assassinItemObtained; set => assassinItemObtained = value; }

    // アイテム取得メソッド    
    public void SetPlayerReportObtained(bool obtained) => PlayerReportObtained = obtained;
    public void SetTheifReportObtained(bool obtained) => TheifReportObtained = obtained;
    public void SetMuscleReportObtained(bool obtained) => MuscleReportObtained = obtained;
    public void SetFireReportObtained(bool obtained) => FireReportObtained = obtained;
    public void SetAssassinReportObtained(bool obtained) => AssassinReportObtained = obtained;
     public void SetPlayerItemObtained(bool obtained) => PlayerItemObtained = obtained;
    public void SetTheifItemObtained(bool obtained) => TheifItemObtained = obtained;
    public void SetMuscleItemObtained(bool obtained) => MuscleItemObtained = obtained;
    public void SetFireItemObtained(bool obtained) => FireItemObtained = obtained;
    public void SetAssassinItemObtained(bool obtained) => AssassinItemObtained = obtained;

    // IDベースでアイテム取得状態を設定するメソッド
    public void SetItemObtained(int itemID, bool obtained)
    {
        switch (itemID)
        {
            case 1: SetPlayerReportObtained(obtained); break;
            case 2: SetTheifReportObtained(obtained); break;
            case 3: SetMuscleReportObtained(obtained); break;
            case 4: SetFireReportObtained(obtained); break;
            case 5: SetAssassinReportObtained(obtained); break;
            case 6: SetPlayerItemObtained(obtained); break;
            case 7: SetTheifItemObtained(obtained); break;
            case 8: SetMuscleItemObtained(obtained); break;
            case 9: SetFireItemObtained(obtained); break;
            case 10: SetAssassinItemObtained(obtained); break;
            default: Debug.LogWarning($"対応していないアイテムID: {itemID}"); break;
        }
    }

    // IDベースでアイテム取得状態を取得するメソッド
    public bool GetItemObtained(int itemID)
    {
        switch (itemID)
        {
            case 1: return PlayerReportObtained;
            case 2: return TheifReportObtained;
            case 3: return MuscleReportObtained;
            case 4: return FireReportObtained;
            case 5: return AssassinReportObtained;
            case 6: return PlayerItemObtained;
            case 7: return TheifItemObtained;
            case 8: return MuscleItemObtained;
            case 9: return FireItemObtained;
            case 10: return AssassinItemObtained;
            default:
                Debug.LogWarning($"対応していないアイテムID: {itemID}");
                return false;
        }
    }

    // すべてのアイテムをリセットするメソッド
    public void ResetAllItems()
    {
        playerReportObtained = false;
        theifReportObtained = false;
        muscleReportObtained = false;
        fireReportObtained = false;
        assassinReportObtained = false;
        playerItemObtained = false;
        theifItemObtained = false;
        muscleItemObtained = false;
        fireItemObtained = false;
        assassinItemObtained = false;
    }

    /*
    // 取得済みアイテム数を取得するメソッド 
    public int GetObtainedReportsCount()
    {
        int count = 0;
        if (playerReportObtained) count++;
        if (theifReportObtained) count++;
        if (muscleReportObtained) count++;
        if (fireReportObtained) count++;
        if (assassinReportObtained) count++;
        return count;
    }

    public int GetObtainedDiariesCount()
    {
        int count = 0;
        if (playerDiaryObtained) count++;
        if (theifDiaryObtained) count++;
        if (muscleDiaryObtained) count++;
        if (fireDiaryObtained) count++;
        if (assassinDiaryObtained) count++;
        return count;
    }

    public int GetTotalObtainedItemsCount()
    {
        return GetObtainedReportsCount() + GetObtainedDiariesCount();
    }
    */
}
