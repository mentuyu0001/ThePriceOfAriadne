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
        if (stageInventoryData.PlayerReportObtained) inventoryData.PlayerReportObtained = stageInventoryData.PlayerReportObtained;
        if (stageInventoryData.TheifReportObtained) inventoryData.TheifReportObtained = stageInventoryData.TheifReportObtained;
        if (stageInventoryData.MuscleReportObtained) inventoryData.MuscleReportObtained = stageInventoryData.MuscleReportObtained;
        if (stageInventoryData.FireReportObtained) inventoryData.FireReportObtained = stageInventoryData.FireReportObtained;
        if (stageInventoryData.AssassinReportObtained) inventoryData.AssassinReportObtained = stageInventoryData.AssassinReportObtained;
        if (stageInventoryData.PlayerItemObtained) inventoryData.PlayerItemObtained = stageInventoryData.PlayerItemObtained;
        if (stageInventoryData.TheifItemObtained) inventoryData.TheifItemObtained = stageInventoryData.TheifItemObtained;
        if (stageInventoryData.MuscleItemObtained) inventoryData.MuscleItemObtained = stageInventoryData.MuscleItemObtained;
        if (stageInventoryData.FireItemObtained) inventoryData.FireItemObtained = stageInventoryData.FireItemObtained;
        if (stageInventoryData.AssassinItemObtained) inventoryData.AssassinItemObtained = stageInventoryData.AssassinItemObtained;
    }

    // Inventoryの取得状況をStageにコピーする
    public void SyncInventoryToStage()
    {
        stageInventoryData.PlayerReportObtained = inventoryData.PlayerReportObtained;
        stageInventoryData.TheifReportObtained = inventoryData.TheifReportObtained;
        stageInventoryData.MuscleReportObtained = inventoryData.MuscleReportObtained;
        stageInventoryData.FireReportObtained = inventoryData.FireReportObtained;
        stageInventoryData.AssassinReportObtained = inventoryData.AssassinReportObtained;
        stageInventoryData.PlayerItemObtained = inventoryData.PlayerItemObtained;
        stageInventoryData.TheifItemObtained = inventoryData.TheifItemObtained;
        stageInventoryData.MuscleItemObtained = inventoryData.MuscleItemObtained;
        stageInventoryData.FireItemObtained = inventoryData.FireItemObtained;
        stageInventoryData.AssassinItemObtained = inventoryData.AssassinItemObtained;
    }
}
