using UnityEngine;
using UnityEngine.UI;
using VContainer;
using System.Linq;
using TMPro;
using Parts.Types;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class ItemSlot : Button
{
    [SerializeField] int itemID;
    [SerializeField] ItemData itemData;
    [SerializeField] InventoryData inventoryData;
    [SerializeField] SelectFirstButton uiController; 
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] PlayerPartsRatio partsRatio;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject buttons;
    [SerializeField] SelectedButtonManager selectedButtonManager;

    [SerializeField] public Image iconImage;

    // ダイアログを開く直前に選択されていたボタンを記憶しておく変数
    private GameObject lastSelectedButton;

    private List<string> pageTexts = new List<string>();
    private int currentPage = 0;

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

    public void UpdateIcon()
    {
        if (iconImage == null)
        {
            Debug.LogError($"{gameObject.name}: IconImageがnullです");
            return;
        }

        // 即座に表示/非表示を切り替え
        iconImage.enabled = IsObtained;
        iconImage.gameObject.SetActive(IsObtained);  // SetActiveを追加
        
        // キャンバスを強制的に更新
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(iconImage.rectTransform);
        
        Debug.Log($"{gameObject.name}のアイコン表示を{(IsObtained ? "表示" : "非表示")}に設定しました " +
                  $"(enabled: {iconImage.enabled}, active: {iconImage.gameObject.activeSelf})");
    }

    public override void OnClick()
    {
        base.OnClick(); // 決定音を鳴らす
        if(IsObtained == false)
        {
            Debug.Log("未取得のアイテムです。");
            return;
        }
        // アイテム説明用UIのボタンを表示
        ButtonsSetTrue();
        // 現在選択されているUI要素を記憶する
        selectedButtonManager.SetLastSelectedButton(EventSystem.current.currentSelectedGameObject);
        Debug.Log("ReturnSelectedButton lastSelectedButton: " + lastSelectedButton);
        // フォーカスを強制的に「次へ」ボタンに移す
        EventSystem.current.SetSelectedGameObject(rightButton);
        Debug.Log("OnClick ItemSlot: " + ItemName);
        ShowItemText();
    }

    protected override void TriggerSelectionEffects()
    {
        // 選択された際のエフェクト処理をここに追加
        ShowFlervorText();
        Debug.Log($"ItemSlot for ItemID {itemID} selected.");
    }

    public void ShowItemText()
    {
        if (itemText != null && itemData != null)
        {
            itemText.text = ItemText;
            ShowPagedText(ItemText);
            Debug.Log($"テキスト表示: {itemText.text}");
        }
        else
        {
            Debug.LogError("itemTextまたはitemDataが設定されていません。");
        }
    }

    public void ShowFlervorText()
    {
        itemText.enabled = true;
        itemText.text = itemData.GetAllQuartersTone(itemID);
    }

    public void ShowPagedText(string rawText)
    {
        // @で分割してページリストを作成
        pageTexts = rawText.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries)
                           .Select(s => s.Trim())
                           .ToList();
        currentPage = 0;
        ShowCurrentPage();
    }

    public void ShowCurrentPage()
    {
        if (itemText != null && pageTexts.Count > 0)
        {
            itemText.text = pageTexts[currentPage];
            Debug.Log($"ページ {currentPage + 1}/{pageTexts.Count}: {itemText.text}");
        }
    }

    public void NextPage()
    {
        if (pageTexts.Count <= 1)
        {
            Debug.Log("１ページしかないため進めません");
            // ページが1枚しかない場合は何もしない
            return;
        }
        if (currentPage < pageTexts.Count - 1)
        {
            currentPage++;
            ShowCurrentPage();
        }
    }

    public void PrevPage()
    {
        if (pageTexts.Count <= 1)
        {
            Debug.Log("１ページしかないため戻れません");
            // ページが1枚しかない場合は何もしない
            return;
        }
        if (currentPage > 0)
        {
            currentPage--;
            ShowCurrentPage();
        }
    }

    public void ButtonsSetTrue()
    {
        buttons.SetActive(true);
    }

    public void ButtonsSetFalse()
    {
        buttons.SetActive(false);
    }

    public void ReturnSelectedButton()
    {
        pageTexts = new List<string>();
        Debug.Log("ReturnSelectedButton lastSelectedButton: " + lastSelectedButton);
        selectedButtonManager.ReturnSelectedButton();
        ButtonsSetFalse();
    }
}
