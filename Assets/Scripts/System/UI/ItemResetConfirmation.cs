using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VContainer;
using System.Runtime.Serialization;

public class ItemResetConfirmation : MonoBehaviour
{
    /// <summary>
    /// アイテムリセットの確認画面用のスクリプト
    /// </summary>

    // 親オブジェクトの取得
    [SerializeField] private GameObject confirmationDialogPanel;

    // Yesボタンの取得
    [SerializeField] private GameObject noButton;

    // InventoryDataの参照
    private InventoryData inventoryData;
    // StageInventoryDataの参照
    private StageInventoryData stageInventoryData;
    // ItemSlotの配列
    [SerializeField] private ItemSlot[] itemSlots;
    [SerializeField] private StageItemSlot[] stageItemSlots;

    // ダイアログを開く直前に選択されていたボタンを記憶しておく変数
    private GameObject lastSelectedButton;

    // ゲームデータマネージャーの取得
    private GameDataManager gameDataManager;

    void Start()
    {
        gameDataManager = GameObject.Find("GameDataManager").GetComponent<GameDataManager>();
        inventoryData = GameObject.Find("InventoryData").GetComponent<InventoryData>();
        stageInventoryData = GameObject.Find("StageInventoryData").GetComponent<StageInventoryData>();

        // 最初は非表示にしておく
        confirmationDialogPanel.SetActive(false);
    }
    
    // 「セーブしますか？」のダイアログを表示するメソッド
    // 各セーブスロットのボタンからこのメソッドを呼び出す
    public void ShowConfirmationDialog()
    {
        UnityEngine.Debug.Log("ShowConfirmationDialog called");
        // 現在選択されているUI要素を記憶する
        lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        confirmationDialogPanel.SetActive(true);
        // フォーカスを強制的に「いいえ」ボタンに移す
        EventSystem.current.SetSelectedGameObject(noButton);
    }

    // 「はい」ボタンが押された時の処理
    public void OnYesButtonClicked()
    {
        Debug.Log("アイテムリセットを実行します。");
        // ここで実際のアイテムリセット処理を呼び出す
        inventoryData.ResetAllItems();
        stageInventoryData.ResetAllItems();
        gameDataManager.SaveItemData();
        gameDataManager.LoadItemData();

        // 全てのItemSlotのアイコンを更新
        foreach (var slot in itemSlots)
        {
            if (slot != null)
            {
                slot.UpdateIcon();
                UnityEngine.Debug.Log($"ItemSlot {slot.name} のアイコン{slot.iconImage.enabled}");
            }
        }
        foreach (var slot in stageItemSlots)
        {
            if (slot != null)
            {
                slot.UpdateIcon();
                UnityEngine.Debug.Log($"ItemSlot {slot.name} のアイコン{slot.iconImage.enabled}");
            }
        }

        CloseDialog();
    }

    // 「いいえ」ボタンが押された時の処理
    public void OnNoButtonClicked()
    {
        Debug.Log("アイテムリセットをキャンセルしました。");
        CloseDialog();
    }

    // ダイアログを閉じる処理を共通化
    private void CloseDialog()
    {
        SoundManager.Instance.PlaySE(14); // 14はUI決定音のインデックス
        confirmationDialogPanel.SetActive(false);
        //  記憶しておいたボタンにフォーカスを戻す
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }
}
