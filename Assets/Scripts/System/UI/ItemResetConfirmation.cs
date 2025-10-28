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
    [SerializeField] private InventoryData inventoryData;
    // StageInventoryDataの参照
    [SerializeField] private StageInventoryData stageInventoryData;

    // ダイアログを開く直前に選択されていたボタンを記憶しておく変数
    private GameObject lastSelectedButton;

    void Start()
    {
        // 最初は非表示にしておく
        confirmationDialogPanel.SetActive(false);
    }
    
    // 「セーブしますか？」のダイアログを表示するメソッド
    // 各セーブスロットのボタンからこのメソッドを呼び出す
    public void ShowConfirmationDialog()
    {
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
        confirmationDialogPanel.SetActive(false);

        //  記憶しておいたボタンにフォーカスを戻す
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }
}
