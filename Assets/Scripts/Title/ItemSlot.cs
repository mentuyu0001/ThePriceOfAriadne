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

    private Image iconImage;

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
        // パーツ占有率を再計算
        partsRatio.CalculatePartsRatio();

        // パーツ占有率を取得
        var allRatios = partsRatio.GetAllRatios();

        if (allRatios.Count == 0)
        {
            Debug.LogWarning("パーツ占有率が取得できませんでした");
            return;
        }

        // 最大の占有率を取得
        float maxRatio = allRatios.Values.Max();

        // 最大占有率のパーツを全て取得（同率の場合は複数）
        var dominantParts = allRatios.Where(x => x.Value == maxRatio)
                                     .Select(x => (PartsChara)x.Key)
                                     .ToList();

        // テキストリストを生成
        var textList = new List<string>();
        foreach (var parts in dominantParts)
        {
            // ItemDataから口調テキストを取得
            string text = itemData.GetToneTextByPartsChara(itemID, parts);
            if (!string.IsNullOrEmpty(text))
            {
                textList.Add(text); // ←ここをAddに修正
            }
        }

        // 全て25%なら特別なテキスト
        if (partsRatio.IsAllQuarters())
        {
            string allQuartersText = itemData.GetAllQuartersTone(itemID);
            itemText.text = allQuartersText;
            Debug.Log("全て25%のため特別テキストを表示: " + itemText.text);
            return;
        }

        // 100%のパーツがあれば特別なテキスト
        if (maxRatio >= 100f && dominantParts.Count == 1)
        {
            string fullToneText = itemData.GetOwnFullTone(itemID);
            itemText.text = fullToneText;
            Debug.Log("100%のため特別テキストを表示: " + itemText.text);
            return;
        }

        // 通常は最大占有率キャラの口調テキストを連結して表示
        if (textList.Count > 0)
        {
            itemText.text = string.Join("\n", textList);
            Debug.Log("最大占有率キャラの口調テキスト: " + itemText.text);
        }
        else
        {
            itemText.text = "セリフがありません";
            Debug.LogWarning("表示するセリフがありません");
        }
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
        //  記憶しておいたボタンにフォーカスを戻す
        selectedButtonManager.ReturnSelectedButton();
        ButtonsSetFalse();
    }
}
