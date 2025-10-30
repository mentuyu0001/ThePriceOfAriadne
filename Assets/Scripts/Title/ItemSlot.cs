using UnityEngine;
using UnityEngine.UI;
using VContainer;
using System.Linq;
using TMPro;

public class ItemSlot : Button
{
    [SerializeField] int itemID;
    [SerializeField] ItemData itemData;
    [SerializeField] InventoryData inventoryData;
    private Image iconImage;
    [SerializeField] TextMeshProUGUI itemText; 

    public string ItemName => itemData.GetItemNameByID(itemID);
    public string ItemText => itemData.GetItemTextByID(itemID);
    public bool IsObtained => inventoryData.GetItemObtained(itemID);

    public void Initialize()
    {
        iconImage = GetComponentsInChildren<Image>(true)
            .FirstOrDefault(img => img.gameObject.name == "IconImage");
        itemText = GetComponentsInChildren<TextMeshProUGUI>(true)
            .FirstOrDefault(txt => txt.gameObject.name == "ItemText");

        if (iconImage == null)
        {
            Debug.LogError("IconImageが見つかりません。ItemSlotの子にIconImageという名前のImageを配置してください。");
        }
        if (itemText == null)
        {
            Debug.LogError("ItemTextが見つかりません。ItemSlotの子にItemTextという名前のTextMeshProUGUIを配置してください。");
        }
        UpdateIcon();
    }

    protected void Start()
    {
        iconImage = GetComponentsInChildren<Image>(true)
            .FirstOrDefault(img => img.gameObject.name == "IconImage");

        if (iconImage == null)
        {
            Debug.LogError("IconImageが見つかりません。ItemSlotの子にIconImageという名前のImageを配置してください。");
        }
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (IsObtained)
        {
            iconImage.sprite = itemData.GetItemIconByID(itemID);
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
    }

    public override void OnClick()
    {
        Debug.Log("OnClick ItemSlot: " + ItemName);
        ShowItemText();
    }

    protected override void TriggerSelectionEffects()
    {
        // 選択された際のエフェクト処理をここに追加
        Debug.Log($"ItemSlot for ItemID {itemID} selected.");
    }

    public void ShowItemText()
    {
        if (itemText != null && itemData != null)
        {
            itemText.text = ItemText;
            Debug.Log($"テキスト表示: {itemText.text}");
        }
        else
        {
            Debug.LogError("itemTextまたはitemDataが設定されていません。");
        }
    }
}
