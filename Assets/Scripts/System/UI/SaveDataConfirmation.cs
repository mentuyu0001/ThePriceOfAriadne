using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SaveDataConfirmation : MonoBehaviour
{
    /// <summary>
    /// セーブデータのセーブとロードの確認画面用のスクリプト
    /// </summary>

    // 親オブジェクトの取得
    [SerializeField] private GameObject confirmationDialogPanel;

    // Yesボタンの取得
    [SerializeField] private GameObject noButton;

    // ダイアログを開く直前に選択されていたボタンを記憶しておく変数
    private GameObject lastSelectedButton;

    // セーブするスロット
    private int saveCurrentSlot;

    void Start()
    {
        // 最初は非表示にしておく
        confirmationDialogPanel.SetActive(false);
    }
    
    // 「セーブしますか？」のダイアログを表示するメソッド
    // 各セーブスロットのボタンからこのメソッドを呼び出す
    public void ShowConfirmationDialog(int slotId)
    {
        // 現在選択されているUI要素を記憶する
        lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        // slotIDを記憶する
        saveCurrentSlot = slotId;
        confirmationDialogPanel.SetActive(true);
        // フォーカスを強制的に「いいえ」ボタンに移す
        EventSystem.current.SetSelectedGameObject(noButton);
    }

    // 「はい」ボタンが押された時の処理
    public void OnYesButtonClickedSave()
    {
        Debug.Log("セーブを実行します。");
        // ここで実際のセーブ処理を呼び出す
        // saveManager.SaveGame(currentSlot);

        CloseDialog();
    }

    public void OnYesButtonClickedLoad()
    {
        Debug.Log("ロードを実行します。");
        // ここで実際のロード処理を呼び出す
        // saveManager.SaveGame(currentSlot);

        CloseDialog(); // 実際は閉じずにゲームを起動する
    }

    // 「いいえ」ボタンが押された時の処理
    public void OnNoButtonClicked()
    {
        Debug.Log("セーブをキャンセルしました。");
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
