using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 鍵のかかったドアのスクリプト
/// </summary>
public class LockedDoor : MonoBehaviour
{
    [SerializeField] private int doorID; // ドアのID，これと同じIDの鍵ならば開けられる
    [SerializeField] private int lockedDoorID = 2; // テキスト表示用のID

    [Inject] private GameObject player;
    [Inject] private PlayerStatus playerStatus;
    [Inject] private GameTextDisplay textDisplay;
    [Inject] private PlayerPartsRatio partsRatio;
    [Inject] private ObjectTextData objectTextData;
    
    [Header("テキスト表示設定")]
    [SerializeField] private float delayBetweenTexts = 2f; // 複数テキスト間の待機時間
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = false;

    private string playerName;
    private bool isPlayerInContact = false;
    private bool hasShownText = false; // テキストを表示済みかどうか

    private CancellationToken dct; // DestroyCancellationToken

    void Start()
    {
        // 注入されたコンポーネントの確認
        Debug.Log($"[LockedDoor] player: {(player != null ? "OK" : "NULL")}");
        Debug.Log($"[LockedDoor] playerStatus: {(playerStatus != null ? "OK" : "NULL")}");
        Debug.Log($"[LockedDoor] textDisplay: {(textDisplay != null ? "OK" : "NULL")}");
        Debug.Log($"[LockedDoor] partsRatio: {(partsRatio != null ? "OK" : "NULL")}");
        Debug.Log($"[LockedDoor] objectTextData: {(objectTextData != null ? "OK" : "NULL")}");
        
        // プレイヤーの名前を取得
        if (player != null)
        {
            playerName = player.name;
            if (showDebugLogs) Debug.Log($"プレイヤー名: {playerName}");
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned.");
        }

        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    // ドアを開く関数
    void OpenDoor()
    {
        if (showDebugLogs) Debug.Log("ドアを開きました");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(10);
        }
        
        // ドアを非表示にする
        gameObject.SetActive(false);
    }

    // ドアを開けるのを試みる関数
    public void TryToOpen()
    {
        // 対応する鍵を持っていれば開く
        if (KeyManager.TryToUseKey(doorID) || playerStatus.CanUnlock)
        {
            OpenDoor();

            // テキストを閉じる
            if (textDisplay != null && textDisplay.IsDisplaying)
            {
                textDisplay.HideText(dct).Forget();
            }
            hasShownText = false;
        }
        else
        {
            // 鍵が必要な時にテキスト表示
            if (!hasShownText)
            {
                if (showDebugLogs) Debug.Log("鍵が必要です。");
                ShowLockedDoorWarningText();
                hasShownText = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーが扉に触れたら開けるのを試みる
        if (player != null && collision.gameObject == player)
        {
            isPlayerInContact = true;
            TryToOpen();
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        // コリジョン内にいる間はフラグを維持
        if (player != null && collision.gameObject == player)
        {
            isPlayerInContact = true;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        // プレイヤーがドアから離れた時
        if (player != null && collision.gameObject == player)
        {
            if (showDebugLogs) Debug.Log("ドアから離れました");
            
            isPlayerInContact = false;
            hasShownText = false;
            
            // テキストを閉じる
            if (textDisplay != null && textDisplay.IsDisplaying)
            {
                textDisplay.HideText(dct).Forget();
            }
        }
    }
    
    private void ShowLockedDoorWarningText()
    {
        Debug.Log($"[ShowLockedDoorWarningText] 開始");
        Debug.Log($"  textDisplay: {textDisplay != null}");
        Debug.Log($"  partsRatio: {partsRatio != null}");
        Debug.Log($"  objectTextData: {objectTextData != null}");
        
        if (textDisplay == null || partsRatio == null || objectTextData == null)
        {
            Debug.LogError($"必要なコンポーネントが不足: textDisplay={textDisplay != null}, partsRatio={partsRatio != null}, objectTextData={objectTextData != null}");
            return;
        }
        
        // 拡張メソッドを使用してテキストを表示
        textDisplay.ShowTextByPartsRatio(
            partsRatio,
            objectTextData,
            lockedDoorID,
            dct,
            delayBetweenTexts,
            showDebugLogs
        ).Forget();
    }
}
