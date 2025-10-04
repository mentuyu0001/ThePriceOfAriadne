using UnityEngine;
using VContainer;
public class ItemSlot : Button
{
    [SerializeField] int itemID; // アイテムのID
    [Inject] ItemData itemData; // アイテムデータの参照
    [Inject] InventoryData inventoryData; // インベントリデータの参照

    // アイテムの名前を取得するプロパティ
    public string ItemName => itemData.GetItemNameByID(itemID);
    // アイテムの説明文を取得するプロパティ
    public string ItemText => itemData.GetItemTextByID(itemID);

    // インベントリデータから取得状況を確認するプロパティ
    public bool IsObtained => inventoryData.GetItemObtained(itemID);

    // クリックされたらアイテムの詳細を表示する
    public override void OnClick()
    {
        if (IsObtained)
        {
            // ここでアイテムの詳細を表示する処理を実装
            Debug.Log($"Item: {ItemName}\nText: {ItemText}");
        }
        else
        {
            Debug.Log("Item not obtained yet.");
        }
    }

}
