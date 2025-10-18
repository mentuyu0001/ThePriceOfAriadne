using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;

/// <summary>
/// プレイヤーが炎の上を歩けるかどうかをチェックするゾーン
/// </summary>
public class FireCheckZone : MonoBehaviour
{
    [Inject] private GameObject player;
    [Inject] private PlayerStatus playerStatus;
    [Inject] private GameTextDisplay textDisplay;
    [Inject] private PlayerPartsRatio partsRatio;
    [Inject] private ObjectTextData objectTextData;
    
    [SerializeField] private int emberFireID = 0; // EmberFireのID
    [SerializeField] private Collider2D fireFieldCollider;
    [SerializeField] private Collider2D fireFieldColliderOpposite;
    
    [Header("テキスト表示設定")]
    [SerializeField] private float delayBetweenTexts = 2f; // 複数テキスト間の待機時間
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = true;
    
    private bool isPlayerInZone = false;
    private bool hasShownText = false; // テキストを表示済みかどうか
    
    private void Start()
    {
        if (player != null && playerStatus != null && fireFieldCollider != null)
        {
            fireFieldCollider.enabled = !playerStatus.CanWalkOnFire;
            fireFieldColliderOpposite.enabled = !playerStatus.CanWalkOnFire;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player && playerStatus != null && fireFieldCollider != null)
        {
            // 既にゾーン内にいる場合は何もしない
            if (isPlayerInZone)
            {
                return;
            }
            
            isPlayerInZone = true;
            
            if (!playerStatus.CanWalkOnFire)
            {
                if (showDebugLogs) Debug.Log("炎で通れない！");
                
                // テキスト表示（まだ表示していない場合のみ）
                if (!hasShownText)
                {
                    ShowFireWarningText();
                    hasShownText = true;
                }
                
                fireFieldCollider.enabled = true;
                fireFieldColliderOpposite.enabled = true;
            }
            else
            {
                if (showDebugLogs) Debug.Log("炎を通り抜けられる");
                fireFieldCollider.enabled = false;
                fireFieldColliderOpposite.enabled = false;
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        // ゾーン内にいる間は何もしない（複数回の呼び出しを防ぐ）
        if (player != null && collision.gameObject == player)
        {
            isPlayerInZone = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player)
        {
            if (showDebugLogs) Debug.Log("炎ゾーンから退出");
            
            isPlayerInZone = false;
            hasShownText = false; // フラグをリセット
            
            // テキストを閉じる
            if (textDisplay != null && textDisplay.IsDisplaying)
            {
                textDisplay.HideText();
            }
        }
    }
    
    private void ShowFireWarningText()
    {
        // 拡張メソッドを使用してテキストを表示
        textDisplay.ShowTextByPartsRatio(
            partsRatio,
            objectTextData,
            emberFireID,
            delayBetweenTexts,
            showDebugLogs
        );
    }
}
