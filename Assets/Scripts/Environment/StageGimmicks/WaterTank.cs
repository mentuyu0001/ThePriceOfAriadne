using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 水をチャージするためのクラス
/// </summary>
public class WaterTank : MonoBehaviour
{
    [Inject] private GameObject player;
    [Inject] private PlayerStatus playerStatus;
    [Inject] private PlayerRunTimeStatus playerRunTimeStatus;
    [Inject] private GameTextDisplay textDisplay;
    [Inject] private PlayerPartsRatio partsRatio;
    [Inject] private ObjectTextData objectTextData;
    
    [SerializeField] private int waterTankID = 4; // WaterTankのID
    
    [Header("テキスト表示設定")]
    [SerializeField] private float delayBetweenTexts = 2f; // 複数テキスト間の待機時間
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = false;
    
    private bool isPlayerInZone = false;
    private bool hasShownText = false; // テキストを表示済みかどうか

    private CancellationToken dct; // DestroyCancellationToken

    void Start()
    {
        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            if (isPlayerInZone)
            {
                return;
            }
            
            isPlayerInZone = true;
            
            // 水をチャージできない場合はテキストを表示
            if (playerStatus != null && !playerStatus.CanChargeWater && !hasShownText)
            {
                if (showDebugLogs) Debug.Log("水をチャージできません。");
                ShowWaterTankWarningText();
                hasShownText = true;
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            isPlayerInZone = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            if (showDebugLogs) Debug.Log("水タンクから退出");
            
            isPlayerInZone = false;
            hasShownText = false;
            
            // テキストを閉じる
            if (textDisplay != null && textDisplay.IsDisplaying)
            {
                textDisplay.HideText(dct).Forget();
            }
        }
    }

    public void ChargeWater()
    {
        // プレイヤーが水をチャージできるかどうかをチェック
        if (playerStatus != null && playerStatus.CanChargeWater)
        {
            if (showDebugLogs) Debug.Log("水をチャージしました！");

            // 水をチャージできる状態なら発射できるようにする
            playerRunTimeStatus.CanShootWater = true;

            // オプション：効果音やエフェクトの再生
            PlayChargeEffect();
        }
        else
        {
            if (showDebugLogs) Debug.Log("水をチャージできません。");
        }
    }
    
    private void ShowWaterTankWarningText()
    {
        // 拡張メソッドを使用してテキストを表示
        textDisplay.ShowTextByPartsRatio(
            partsRatio,
            objectTextData,
            waterTankID,
            dct,
            delayBetweenTexts,
            showDebugLogs
        ).Forget();
    }
    
    private void PlayChargeEffect()
    {
        // 水をチャージする効果音やエフェクトを再生
        // AudioSource等を使用して実装
    }
}
