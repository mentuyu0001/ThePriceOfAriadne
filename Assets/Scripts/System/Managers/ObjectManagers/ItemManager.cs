using UnityEngine;
using VContainer;

/// <summary>
/// アイテムの取得を管理するクラス
/// </summary>
public class ItemManager : MonoBehaviour
{
    // InventoryDataの参照
    [Inject] private InventoryData inventoryData;
    // StageInventoryDataの参照
    [Inject] private StageInventoryData stageInventoryData;
    // ItemDataの参照を追加
    [Inject] private ItemData itemData;

    // アイテムを取得するメソッド
    public void ObtainItem(int itemID)
    {
        // ItemDataからアイテム情報を取得
        var item = itemData.GetItemByID(itemID);
        if (item == null)
        {
            Debug.LogWarning($"アイテムID {itemID} は ItemData に存在しません。");
            return;
        }

        // アイテムIDに応じてInventoryDataのboolをtrueに設定
        switch (itemID)
        {
            // 研究報告書
            case 1: // 少女の研究報告書
                StageInventoryData.Instance.SetPlayerReportObtained(true);
                break;
            case 2: // 泥棒の研究報告書
                StageInventoryData.Instance.SetTheifReportObtained(true);
                break;
            case 3: // マッチョの研究報告書
                StageInventoryData.Instance.SetMuscleReportObtained(true);
                break;
            case 4: // 消防士の研究報告書
                StageInventoryData.Instance.SetFireReportObtained(true);
                break;
            case 5: // アサシンの研究報告書
                StageInventoryData.Instance.SetAssassinReportObtained(true);
                break;

            // 日記
            case 6: // 少女の日記
                StageInventoryData.Instance.SetPlayerItemObtained(true);
                break;
            case 7: // 泥棒の日記
                StageInventoryData.Instance.SetTheifItemObtained(true);
                break;
            case 8: // マッチョの日記
                StageInventoryData.Instance.SetMuscleItemObtained(true);
                break;
            case 9: // 消防士の日記
                StageInventoryData.Instance.SetFireItemObtained(true);
                break;
            case 10: // アサシンの日記
                StageInventoryData.Instance.SetAssassinItemObtained(true);
                break;

            default:
                Debug.LogWarning($"対応していないアイテムID: {itemID}");
                return;
        }

        // ItemDataから取得した正確なアイテム名でログ出力
        Debug.Log($"アイテムを取得しました: {item.name} (ID: {itemID})");
    }

    // アイテムが取得済みかどうかを確認するメソッド
    public bool IsItemObtained(int itemID)
    {
        switch (itemID)
        {
            case 1: return StageInventoryData.Instance.PlayerReportObtained;
            case 2: return StageInventoryData.Instance.TheifReportObtained;
            case 3: return StageInventoryData.Instance.MuscleReportObtained;
            case 4: return StageInventoryData.Instance.FireReportObtained;
            case 5: return StageInventoryData.Instance.AssassinReportObtained;
            case 6: return StageInventoryData.Instance.PlayerItemObtained;
            case 7: return StageInventoryData.Instance.TheifItemObtained;
            case 8: return StageInventoryData.Instance.MuscleItemObtained;
            case 9: return StageInventoryData.Instance.FireItemObtained;
            case 10: return StageInventoryData.Instance.AssassinItemObtained;
            default:
                Debug.LogWarning($"対応していないアイテムID: {itemID}");
                return false;
        }
    }
    // StageInventoryDataの取得状況をInventoryDataに反映する
    public void SyncStageToInventory()
    {
        inventoryData.PlayerReportObtained = stageInventoryData.PlayerReportObtained;
        inventoryData.TheifReportObtained = stageInventoryData.TheifReportObtained;
        inventoryData.MuscleReportObtained = stageInventoryData.MuscleReportObtained;
        inventoryData.FireReportObtained = stageInventoryData.FireReportObtained;
        inventoryData.AssassinReportObtained = stageInventoryData.AssassinReportObtained;
        inventoryData.PlayerItemObtained = stageInventoryData.PlayerItemObtained;
        inventoryData.TheifItemObtained = stageInventoryData.TheifItemObtained;
        inventoryData.MuscleItemObtained = stageInventoryData.MuscleItemObtained;
        inventoryData.FireItemObtained = stageInventoryData.FireItemObtained;
        inventoryData.AssassinItemObtained = stageInventoryData.AssassinItemObtained;
    }
}
