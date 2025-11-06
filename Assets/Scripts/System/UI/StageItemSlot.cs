using UnityEngine;
using UnityEngine.UI;
using VContainer;
using System.Linq;
using TMPro;
using Parts.Types;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

public class StageItemSlot : Button
{
    [SerializeField] int itemID;
    [SerializeField] ItemData itemData;
    [SerializeField] StageInventoryData stageInventoryData;
    [SerializeField] SelectFirstButton uiController;
    [SerializeField] TextMeshProUGUI itemText;
    [SerializeField] TextMeshProUGUI itemText2;
    private PlayerPartsRatio partsRatio;
    [SerializeField] GameObject rightButton;
    [SerializeField] GameObject buttons;
    [SerializeField] SelectedButtonManager selectedButtonManager;

    [SerializeField] public Image iconImage;

    [SerializeField] private GameObject playerImg;
    [SerializeField] private GameObject theifImg;
    [SerializeField] private GameObject fireImg;
    [SerializeField] private GameObject musuleImg;
    [SerializeField] private GameObject assasinImg;
    

    // ダイアログを開く直前に選択されていたボタンを記憶しておく変数
    private GameObject lastSelectedButton;

    private List<string> pageTexts = new List<string>();
    private int currentPage = 0;

    public string ItemName => itemData.GetItemNameByID(itemID);
    public string ItemText => itemData.GetItemTextByID(itemID);
    public bool IsObtained => stageInventoryData.GetItemObtained(itemID);
    private bool isInitialized = false;

    public void Initialize()
    {
        if (partsRatio == null)
        {
            //Debug.LogError("PlayerPartsRatio: PlayerPartsが注入されていません");
            return;
        }
        isInitialized = true;

        iconImage = GetComponentsInChildren<Image>(true)
            .FirstOrDefault(img => img.gameObject.name == "IconImage");
        itemText = GetComponentsInChildren<TextMeshProUGUI>(true)
            .FirstOrDefault(txt => txt.gameObject.name == "ItemText");
        itemText2 = GetComponentsInChildren<TextMeshProUGUI>(true)
            .FirstOrDefault(txt => txt.gameObject.name == "ItemText2");

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
    public void Start()
    {
        partsRatio = GameObject.Find("PlayerPartsRatio").GetComponent<PlayerPartsRatio>();
    }
    protected void Awake()
    {
        ImageFalse();

        stageInventoryData = GameObject.Find("StageInventoryData").GetComponent<StageInventoryData>();
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
        if (IsObtained == false)
        {
            Debug.Log("未取得のアイテムです。");
            return;
        }

        ImageFalse();
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
        // nullチェックとデバッグログを追加
        if (itemText == null) Debug.LogError($"{gameObject.name}: itemTextがnullです");
        if (itemText2 == null) Debug.LogError($"{gameObject.name}: itemText2がnullです");
        if (partsRatio == null) Debug.LogError($"{gameObject.name}: partsRatioがnullです");

        itemText.enabled = true;
        itemText2.enabled = true;

        if (!IsObtained)
        {
            itemText.text = "???";
            itemText2.text = "";
            return;
        }

        // パーツ比率の取得
        var ratios = partsRatio.GetAllRatios();
        var orderedRatios = ratios.OrderByDescending(x => x.Value).ToList();
        UnityEngine.Debug.Log("Parts Ratios: " + string.Join(", ", orderedRatios.Select(r => $"{r.Key}: {r.Value}%")));

        // アイテムの所有者タイプを取得
        var itemOwner = itemData.GetItemByID(itemID).ownerType;
        var dominantParts = partsRatio.GetDominantParts();

        // パーツが100%で、アイテムの所有者と一致する場合
        if (partsRatio.HasFullRatioParts())
        {
            UnityEngine.Debug.Log("100%の口調表示");
            var fullParts = partsRatio.GetFullRatioParts();
            if (itemOwner.ToString() == fullParts.ToString() ||
                (itemOwner == ItemOwnerType.Fire && fullParts == PartsChara.Fire))
            {
                itemText.text = itemData.GetOwnFullTone(itemID);
                itemText2.text = "";
                return;
            }
        }

        // 50-50の場合
        if (orderedRatios.Count >= 2 &&
            Mathf.Approximately(orderedRatios[0].Value, 50f) &&
            Mathf.Approximately(orderedRatios[1].Value, 50f))
        {
            UnityEngine.Debug.Log("50-50の口調表示");
            itemText.text = itemData.GetToneTextByPartsChara(itemID, orderedRatios[0].Key);
            itemText2.text = itemData.GetToneTextByPartsChara(itemID, orderedRatios[1].Key);
            return;
        }

        // それ以外の場合は最も比率の高いパーツの口調を表示
        UnityEngine.Debug.Log("通常の口調表示");
        itemText.text = itemData.GetToneTextByPartsChara(itemID, dominantParts);
        itemText2.text = "";
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
            if (currentPage == 0)
            {
                switch (itemID)
                {
                    case 1:
                        playerImg.SetActive(true);
                        break;
                    case 2:
                        theifImg.SetActive(true);
                        break;
                    case 3:
                        musuleImg.SetActive(true);
                        break;
                    case 4:
                        fireImg.SetActive(true);
                        break;
                    case 5:
                        assasinImg.SetActive(true);
                        break;
                    default:
                        break;
                }   
            }
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
            ImageFalse();
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
            ImageFalse();
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
        ImageFalse();
        pageTexts = new List<string>();
        Debug.Log("ReturnSelectedButton lastSelectedButton: " + lastSelectedButton);
        selectedButtonManager.ReturnSelectedButton();
        ButtonsSetFalse();
    }

    protected void OnEnable()
    {
        UpdateIcon();
    }

    private void ImageFalse()
    {
        playerImg.SetActive(false);
        theifImg.SetActive(false);
        fireImg.SetActive(false);
        musuleImg.SetActive(false);
        assasinImg.SetActive(false);
    }
}
