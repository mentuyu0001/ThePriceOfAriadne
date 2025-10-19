using UnityEngine;
using UnityEngine.InputSystem; // 新しいInput Systemを使う場合

public class PauseManager : MonoBehaviour
{
    // Inspectorからポーズ画面のUIパネルをアタッチする
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private PlayerInput playerInput;

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
                Resume();
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
    }

    // ゲームをポーズするメソッド
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // 時間の流れを止める
        playerInput.SwitchCurrentActionMap("UI");
        isPaused = true;
    }
}