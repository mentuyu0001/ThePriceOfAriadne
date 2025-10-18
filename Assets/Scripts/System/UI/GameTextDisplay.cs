using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

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
    [SerializeField] private bool showDebugLogs = false;
    
    private CanvasGroup canvasGroup;
    private bool isDisplaying = false;
    private CancellationTokenSource currentCts;
    private bool isDestroyed = false;

    private void Awake()
    {
        if (textPanel != null)
        {
            canvasGroup = textPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = textPanel.AddComponent<CanvasGroup>();
            }
        }
        else
        {
            Debug.LogError("❌ textPanelがnullです");
        }
        
        if (messageText == null)
        {
            Debug.LogError("❌ messageTextがnullです");
        }
        
        HideImmediate();
    }

    public async UniTask ShowText(string message)
    {
        if (isDestroyed || this == null) return;
        
        if (textPanel == null || messageText == null || canvasGroup == null)
        {
            Debug.LogError("❌ UI要素がnullです");
            return;
        }
        
        // 既に表示中の場合はキャンセルして新しいテキストを表示
        if (isDisplaying)
        {
            currentCts?.Cancel();
            currentCts?.Dispose();
            currentCts = null;
            await UniTask.Yield(); // 1フレーム待機
        }

        currentCts = new CancellationTokenSource();
        isDisplaying = true;

        try
        {
            if (messageText != null)
            {
                messageText.text = "";
            }
            
            if (textPanel != null)
            {
                textPanel.SetActive(true);
            }
            
            // フェードイン
            await FadeIn(currentCts.Token);
            
            // テキストを徐々に表示
            await ShowTextGradually(message, currentCts.Token);
            
            // 表示を維持（HideTextが呼ばれるまで待機）
            // ※ WaitUntilCanceledは削除して、明示的に表示を維持
        }
        catch (System.OperationCanceledException)
        {
            // キャンセル時は正常終了
        }
        catch (System.Exception e)
        {
            if (!isDestroyed && this != null)
            {
                Debug.LogError($"❌ テキスト表示中にエラー: {e.Message}");
            }
        }
    }

    public async void HideText()
    {
        if (!isDisplaying || isDestroyed || this == null) return;
        
        // キャンセルトークンをキャンセル
        currentCts?.Cancel();
        currentCts?.Dispose();
        currentCts = null;

        try
        {
            // フェードアウト
            await FadeOut(CancellationToken.None);
            
            if (textPanel != null && !isDestroyed && this != null)
            {
                textPanel.SetActive(false);
            }
        }
        catch (System.Exception e)
        {
            if (!isDestroyed && this != null)
            {
                Debug.LogError($"❌ テキスト非表示中にエラー: {e.Message}");
            }
        }
        finally
        {
            isDisplaying = false;
        }
    }

    private async UniTask ShowTextGradually(string fullText, CancellationToken token)
    {
        if (messageText == null || isDestroyed || this == null) return;
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (messageText == null || isDestroyed || this == null || token.IsCancellationRequested)
            {
                return;
            }
            
            messageText.text = fullText.Substring(0, i);
            
            try
            {
                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(characterDisplayInterval),
                    cancellationToken: token
                );
            }
            catch (System.OperationCanceledException)
            {
                return;
            }
        }
    }

    public bool IsDisplaying => isDisplaying;

    public void HideImmediate()
    {
        if (isDestroyed || this == null) return;
        
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
        if (canvasGroup == null || isDestroyed || this == null) return;

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            if (canvasGroup == null || isDestroyed || this == null || token.IsCancellationRequested)
            {
                return;
            }
            
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            
            try
            {
                await UniTask.Yield(token);
            }
            catch (System.OperationCanceledException)
            {
                return;
            }
        }
        
        if (canvasGroup != null && !isDestroyed && this != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private async UniTask FadeOut(CancellationToken token)
    {
        if (canvasGroup == null || isDestroyed || this == null) return;

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            if (canvasGroup == null || isDestroyed || this == null || token.IsCancellationRequested)
            {
                return;
            }
            
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeOutDuration));
            
            try
            {
                await UniTask.Yield(token);
            }
            catch (System.OperationCanceledException)
            {
                return;
            }
        }
        
        if (canvasGroup != null && !isDestroyed && this != null)
        {
            canvasGroup.alpha = 0f;
        }
    }

    private void OnDestroy()
    {
        isDestroyed = true;
        currentCts?.Cancel();
        currentCts?.Dispose();
        currentCts = null;
    }
}

/// <summary>
/// GameTextDisplayの拡張メソッド
/// </summary>
public static class GameTextDisplayExtensions
{
    /// <summary>
    /// パーツ占有率に基づいてテキストを表示
    /// </summary>
    public static async UniTaskVoid ShowTextByPartsRatio(
        this GameTextDisplay textDisplay,
        PlayerPartsRatio partsRatio,
        ObjectTextData objectTextData,
        int objectID,
        float delayBetweenTexts = 2f,
        bool showDebugLogs = false)
    {
        if (textDisplay == null || partsRatio == null || objectTextData == null)
        {
            Debug.LogError("必要なコンポーネントが不足しています");
            return;
        }
        
        // 既に表示中の場合は何もしない
        if (textDisplay.IsDisplaying)
        {
            if (showDebugLogs) Debug.Log("既にテキスト表示中のためスキップ");
            return;
        }
        
        // パーツ占有率を再計算
        partsRatio.CalculatePartsRatio();
        
        // パーツ占有率を取得
        var allRatios = partsRatio.GetAllRatios();
        
        if (showDebugLogs)
        {
            Debug.Log("=== パーツ占有率詳細 ===");
            foreach (var ratio in allRatios)
            {
                Debug.Log($"{ratio.Key}: {ratio.Value}%");
            }
        }
        
        if (allRatios.Count == 0)
        {
            Debug.LogWarning("パーツ占有率が取得できませんでした");
            return;
        }
        
        // 最大の占有率を取得
        float maxRatio = allRatios.Values.Max();
        
        if (showDebugLogs) Debug.Log($"最大占有率: {maxRatio}%");
        
        // 最大占有率のパーツを全て取得（同率の場合は複数）
        var dominantParts = allRatios.Where(x => x.Value == maxRatio)
                                      .Select(x => x.Key)
                                      .ToList();
        
        if (showDebugLogs)
        {
            Debug.Log($"最大占有率のパーツ: {string.Join(", ", dominantParts)}");
        }
        
        // テキストリストを生成
        var textList = new List<string>();
        foreach (var parts in dominantParts)
        {
            string text = objectTextData.GetTextByIDAndCharacter(objectID, parts);
            
            if (showDebugLogs)
            {
                Debug.Log($"{parts}のテキスト: \"{text}\"");
            }
            
            if (!string.IsNullOrEmpty(text))
            {
                textList.Add(text);
            }
        }
        
        if (textList.Count == 0)
        {
            Debug.LogWarning("表示するテキストが空です");
            return;
        }
        
        // テキストを順次表示
        await ShowTextsSequentially(textDisplay, textList, delayBetweenTexts, showDebugLogs);
    }
    
    /// <summary>
    /// 複数のテキストを順次表示
    /// </summary>
    private static async UniTask ShowTextsSequentially(
        GameTextDisplay textDisplay,
        List<string> textList,
        float delayBetweenTexts,
        bool showDebugLogs)
    {
        if (textList.Count == 1)
        {
            // 1つだけの場合は通常表示
            if (showDebugLogs) Debug.Log($"表示するテキスト:\n{textList[0]}");
            await textDisplay.ShowText(textList[0]);
        }
        else
        {
            // 複数ある場合は順次表示
            for (int i = 0; i < textList.Count; i++)
            {
                if (showDebugLogs) Debug.Log($"表示するテキスト ({i + 1}/{textList.Count}):\n{textList[i]}");
                
                await textDisplay.ShowText(textList[i]);
                
                // 最後のテキストでない場合は待機
                if (i < textList.Count - 1)
                {
                    await UniTask.Delay(
                        System.TimeSpan.FromSeconds(delayBetweenTexts),
                        cancellationToken: textDisplay.GetCancellationTokenOnDestroy()
                    );
                }
            }
        }
    }
}
