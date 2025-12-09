using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 新しいInput Systemを使う場合

public class PauseManager : MonoBehaviour
{
    private bool isLoading = false;

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

    [SerializeField] private Controller controller;

    // Escキーに関する処理
    [Header("UIをEscキーで消せるようにするために必要なGameObjectの注入")]
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject itemInventory;
    [SerializeField] private GameObject itemResetConfirmation;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject toTitleConfirmation;
    [SerializeField] private GameObject itemDescroptionButtons;

    [Header("Escキーを押した時のReturnボタンのスクリプトの注入")]
    [SerializeField] private GameObject toTitleReturn;
    [SerializeField] private GameObject itemReturn;
    [SerializeField] private GameObject itemResetReturn;
    [SerializeField] private GameObject settingsReturn;
    [SerializeField] private GameObject itemDescriptionReturn;

    [SerializeField] private ButtonSound buttonSound;
    private UnityEngine.UI.Button targetButton;


    private void Start()
    {
        // 最初はポーズ画面を非表示にしておく
        pauseMenuUI.SetActive(false);
    }

    // Input Systemから呼び出されるメソッド
    public void OnPause(InputAction.CallbackContext context)
    {
        if (controller.isStartGoal) return;
        if (isLoading) return;

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
        if (playMenu.activeSelf)
        {
            if (toTitleConfirmation.activeSelf)
            {
                targetButton = toTitleReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
            else
            {
                buttonSound.OnClick();
                pauseMenuUI.SetActive(false);
                Time.timeScale = 1f; // 時間の流れを元に戻す
                playerInput.SwitchCurrentActionMap("Player");
                isPaused = false;
                pauseMenuUIScript.ResetLastSelected();

                return;
            }
        }
        else if (itemInventory.activeSelf)
        {
            if (itemDescroptionButtons.activeSelf)
            {
                targetButton = itemDescriptionReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
            else if (itemResetConfirmation.activeSelf)
            {
                targetButton = itemResetReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
            else
            {
                targetButton = itemReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
        }
        else if (settings.activeSelf)
        {
            targetButton = settingsReturn.GetComponent<UnityEngine.UI.Button>();
            targetButton.onClick.Invoke();
            return;
        }
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

    public void ActiveIsLoading()
    {
        isLoading = true;
    }
}