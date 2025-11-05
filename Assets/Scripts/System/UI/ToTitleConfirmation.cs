using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VContainer;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System;

public class ToTitleConfirmation : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private FadeController fadeController;

    // 親オブジェクトの取得
    [SerializeField] private GameObject confirmationDialogPanel;

    // Yesボタンの取得
    [SerializeField] private GameObject noButton;

    // ダイアログを開く直前に選択されていたボタンを記憶しておく変数
    private GameObject lastSelectedButton;

    private float fadeAnimation = 3.0f;

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
    public async void OnYesButtonClicked()
    {
        EventSystem.current.SetSelectedGameObject(null);
        
        Debug.Log("アイテムリセットを実行します。");
        SoundManager.Instance.PlaySE(14); // 14はUI決定音のインデックス

        fadeController.FadeOut(fadeAnimation).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(fadeAnimation), ignoreTimeScale: true);

        gameSceneManager.LoadTitle();
    }

    // 「いいえ」ボタンが押された時の処理
    public void OnNoButtonClicked()
    {
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
