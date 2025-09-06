using UnityEngine;
using VContainer;
/// <summary>
/// アイテムの取得を管理するクラス
/// </summary>
public class ItemManager : MonoBehaviour
{
    // InventoryDataの参照
    [Inject] private InventoryData inventoryData; 

    // アイテムを取得するメソッド
    public void ObtainItem(int itemID)
    {
        // アイテムIDに応じてInventoryDataのboolをtrueに設定
        switch (itemID)
        {
            // 研究報告書
            case 1: // 少女の研究報告書
                InventoryData.Instance.SetPlayerReportObtained(true);
                Debug.Log("Collected item: 少女の研究報告書");
                break;
            case 2: // 泥棒の研究報告書
                InventoryData.Instance.SetTheifReportObtained(true);
                Debug.Log("Collected item: 泥棒の研究報告書");
                break;
            case 3: // マッチョの研究報告書
                InventoryData.Instance.SetMuscleReportObtained(true);
                Debug.Log("Collected item: マッチョの研究報告書");
                break;
            case 4: // 消防士の研究報告書
                InventoryData.Instance.SetFireReportObtained(true);
                Debug.Log("Collected item: 消防士の研究報告書");
                break;
            case 5: // アサシンの研究報告書
                InventoryData.Instance.SetAssassinReportObtained(true);
                Debug.Log("Collected item: アサシンの研究報告書");
                break;

            // 日記
            case 6: // 少女の日記
                InventoryData.Instance.SetPlayerDiaryObtained(true);
                Debug.Log("Collected item: 少女の日記");
                break;
            case 7: // 泥棒の日記
                InventoryData.Instance.SetTheifDiaryObtained(true);
                Debug.Log("Collected item: 泥棒の日記");
                break;
            case 8: // マッチョの日記
                InventoryData.Instance.SetMuscleDiaryObtained(true);
                Debug.Log("Collected item: マッチョの日記");
                break;
            case 9: // 消防士の日記
                InventoryData.Instance.SetFireDiaryObtained(true);
                Debug.Log("Collected item: 消防士の日記");
                break;
            case 10: // アサシンの日記
                InventoryData.Instance.SetAssassinDiaryObtained(true);
                Debug.Log("Collected item: アサシンの日記");
                break;

            default:
                Debug.LogWarning($"Unknown item ID: {itemID}");
                break;
        }
    }
}
