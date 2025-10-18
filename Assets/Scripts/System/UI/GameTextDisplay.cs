using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// ゲーム中にテキストを表示するシステム
/// </summary>
public class GameTextDisplay : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI messageText;
    
    [Header("表示設定")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float characterDisplayInterval = 0.05f;
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = true;
    
    private CanvasGroup canvasGroup;
    private bool isDisplaying = false;
    private CancellationTokenSource currentCts;

    private void Awake()
    {
        Debug.Log("=== GameTextDisplay Awake開始 ===");
        
        if (textPanel != null)
        {
            canvasGroup = textPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = textPanel.AddComponent<CanvasGroup>();
            }
            Debug.Log($"✅ textPanel設定完了: {textPanel.name}");
        }
        else
        {
            Debug.LogError("❌ textPanelがnullです");
        }
        
        if (messageText != null)
        {
            Debug.Log($"✅ messageText設定完了: {messageText.name}");
        }
        else
        {
            Debug.LogError("❌ messageTextがnullです");
        }
        
        HideImmediate();
        Debug.Log("=== GameTextDisplay Awake完了 ===");
    }

    public async UniTask ShowText(string message)
    {
        Debug.Log($"=== ShowText呼び出し: [{message}] ===");
        
        if (textPanel == null || messageText == null || canvasGroup == null)
        {
            Debug.LogError("❌ UI要素がnullです");
            return;
        }
        
        if (isDisplaying)
        {
            Debug.Log("既に表示中のため、前の表示をキャンセル");
            currentCts?.Cancel();
            currentCts?.Dispose();
        }

        currentCts = new CancellationTokenSource();
        isDisplaying = true;

        try
        {
            messageText.text = "";
            
            textPanel.SetActive(true);
            Debug.Log($"✅ Panel表示: {textPanel.activeSelf}");
            
            Debug.Log("フェードイン開始");
            await FadeIn(currentCts.Token);
            Debug.Log($"フェードイン完了: alpha={canvasGroup.alpha}");
            
            Debug.Log("文字表示開始");
            await ShowTextGradually(message, currentCts.Token);
            Debug.Log("文字表示完了");
            
            Debug.Log("⏸️ テキスト表示中（コリジョンから外れるまで待機）...");
            
            await UniTask.WaitUntilCanceled(currentCts.Token);
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("⚠️ テキスト表示がキャンセルされました（コリジョンから外れた）");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ テキスト表示中にエラー: {e.Message}\n{e.StackTrace}");
        }
        finally
        {
            try
            {
                Debug.Log("フェードアウト開始");
                await FadeOut(CancellationToken.None);
                Debug.Log("フェードアウト完了");
            }
            catch { }
            
            if (textPanel != null)
            {
                textPanel.SetActive(false);
                Debug.Log($"✅ Panel非表示: {textPanel.activeSelf}");
            }
            
            isDisplaying = false;
            currentCts?.Dispose();
            currentCts = null;
            Debug.Log("=== ShowText完了 ===\n");
        }
    }

    public void HideText()
    {
        Debug.Log("HideText実行（コリジョンから外れた）");
        
        if (isDisplaying && currentCts != null)
        {
            currentCts.Cancel();
        }
    }

    private async UniTask ShowTextGradually(string fullText, CancellationToken token)
    {
        for (int i = 0; i <= fullText.Length; i++)
        {
            messageText.text = fullText.Substring(0, i);
            await UniTask.Delay(
                System.TimeSpan.FromSeconds(characterDisplayInterval),
                cancellationToken: token
            );
        }
    }

    public bool IsDisplaying => isDisplaying;

    public void HideImmediate()
    {
        if (showDebugLogs) Debug.Log("HideImmediate実行");
        
        currentCts?.Cancel();
        currentCts?.Dispose();
        currentCts = null;
        
        isDisplaying = false;
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        if (textPanel != null)
        {
            textPanel.SetActive(false);
        }
    }

    private async UniTask FadeIn(CancellationToken token)
    {
        if (canvasGroup == null) return;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            await UniTask.Yield(token);
        }
        
        canvasGroup.alpha = 1f;
    }

    private async UniTask FadeOut(CancellationToken token)
    {
        if (canvasGroup == null) return;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeOutDuration));
            await UniTask.Yield(token);
        }
        
        canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        currentCts?.Cancel();
        currentCts?.Dispose();
    }
}
