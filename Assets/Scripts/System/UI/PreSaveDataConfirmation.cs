using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PreSaveDataConfirmation : MonoBehaviour
{
    [SerializeField] private GameObject saveSlotPanel; // セーブスロットのUI
    [SerializeField] private GameObject confirmationPanel; // 「セーブしますか？」パネル
    [SerializeField] private GameObject firstSelectedButton; // セーブスロットの最初に選択するボタン
    [SerializeField] private GameObject returnButton;

    // Yesボタンが押されたとき
    public void OnYesButtonClicked()
    {
        if (saveSlotPanel != null)
            saveSlotPanel.SetActive(true); // セーブスロット表示
            returnButton.SetActive(true);

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false); // 確認パネル非表示

        EventSystem.current.SetSelectedGameObject(firstSelectedButton); // フォーカスをセーブスロットの最初のボタンに移す
    }

    // Noボタンが押されたとき
    public void OnNoButtonClicked()
    {
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false); // 確認パネル非表示

        // 次のステージへ遷移 
    }
}
