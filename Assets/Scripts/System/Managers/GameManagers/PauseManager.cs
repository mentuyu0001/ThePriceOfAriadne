using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemを使う場合

public class PauseManager : MonoBehaviour
{
    // Inspectorからポーズ画面のUIパネルをアタッチする
    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private PlayerInput playerInput;

    // uiボタンのリセット用スクリプトのアタッチ
    [SerializeField] private SelectFirstButton pauseMenuUIScript;

    // 設定でのプレイヤーの見た目を変更するスクリプトの参照
    [SerializeField] private PlayerVisualCustomizer playerVisualCustomizer;
    
    // テキストパネルの参照
    [SerializeField] private GameObject textPanel;

    // 音を追加する
    [SerializeField] private SoundManager soundManager;

    // 現在ポーズ中かどうかを保持するフラグ
    private bool isPaused = false;

    private void Start()
    {
        // 最初はポーズ画面を非表示にしておく
        pauseMenuUI.SetActive(false);
    }

    // Input Systemから呼び出されるメソッド
    public void OnPause(InputAction.CallbackContext context)
    {
        // ボタンが押された瞬間だけ実行する
        if (context.performed)
        {
            // isPausedの状態を反転させる
            if (isPaused)
            {
                if (pauseMenuUI.activeSelf) 
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    // ゲームを再開するメソッド
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // 時間の流れを元に戻す
        playerInput.SwitchCurrentActionMap("Player");
        isPaused = false;
        pauseMenuUIScript.ResetLastSelected();
    }

    // ゲームをポーズするメソッド
    private void Pause()
    {
        soundManager.PlaySE(14);
        pauseMenuUI.SetActive(true);
        playerVisualCustomizer.visuallizePlayerParts(); // プレイヤーの見た目を反映
        Time.timeScale = 0f; // 時間の流れを止める
        playerInput.SwitchCurrentActionMap("UI");
        isPaused = true;
        if (textPanel != null)
        {
            textPanel.SetActive(false);
        }
    }
}