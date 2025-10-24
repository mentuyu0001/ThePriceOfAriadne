using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Parts.Types; // 追加

/// <summary>
/// ゲーム中にテキストを表示するシステム
/// </summary>
public class GameTextDisplay : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject textBackground; 
    
    [Header("表示設定")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    [SerializeField] private float characterDisplayInterval = 0.05f;
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = false;
    
    private CanvasGroup canvasGroup;
    private CanvasGroup backgroundCanvasGroup; // 背景用のCanvasGroup
    private CanvasGroup textCanvasGroup; // テキスト用のCanvasGroup
    private bool isDisplaying = false;
    private bool isFading = false;
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
        
        // テキスト用のCanvasGroupを設定
        if (messageText != null)
        {
            textCanvasGroup = messageText.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                textCanvasGroup = messageText.gameObject.AddComponent<CanvasGroup>();
            }
        }
        else
        {
            Debug.LogError("❌ messageTextがnullです");
        }
        
        // 背景のCanvasGroupを設定
        if (textBackground != null)
        {
            backgroundCanvasGroup = textBackground.GetComponent<CanvasGroup>();
            if (backgroundCanvasGroup == null)
            {
                backgroundCanvasGroup = textBackground.gameObject.AddComponent<CanvasGroup>();
            }
        }
        else
        {
            Debug.LogWarning("⚠️ textBackgroundが設定されていません");
        }
        
        HideImmediate();
    }

    public async UniTask ShowText(string message)
    {
        if (isDestroyed || this == null) return;
        
        // フェード中(フェードイン・フェードアウト問わず)は新しいテキストを表示できない
        if (isFading)
        {
            if (showDebugLogs) Debug.Log("⚠️ フェード中のため表示をスキップ");
            return;
        }
        
        // 表示中の場合も新規表示をブロック
        if (isDisplaying)
        {
            if (showDebugLogs) Debug.Log("⚠️ 既に表示中のため表示をスキップ");
            return;
        }
        
        if (textPanel == null || messageText == null || canvasGroup == null)
        {
            Debug.LogError("❌ UI要素がnullです");
            return;
        }

        currentCts = new CancellationTokenSource();
        isDisplaying = true;
        isFading = true; // フェード開始

        try
        {
            if (messageText != null)
            {
                messageText.text = message; // 全文を即座に設定
            }
            
            if (textPanel != null)
            {
                textPanel.SetActive(true);
            }
            
            // フェードイン
            await FadeIn(currentCts.Token);
            
            isFading = false; // フェードイン完了
            
            // 表示を維持（HideTextが呼ばれるまで待機）
        }
        catch (System.OperationCanceledException)
        {
            // キャンセル時は正常終了
            isFading = false;
            isDisplaying = false;
        }
        catch (System.Exception e)
        {
            if (!isDestroyed && this != null)
            {
                Debug.LogError($"❌ テキスト表示中にエラー: {e.Message}");
            }
            isFading = false;
            isDisplaying = false;
        }
    }

    public async void HideText()
    {
        if (!isDisplaying || isDestroyed || this == null) return;
        
        // 既にフェード中なら何もしない
        if (isFading)
        {
            if (showDebugLogs) Debug.Log("⚠️ 既にフェード中のためスキップ");
            return;
        }
        
        isFading = true; // フェード開始
        
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
            isFading = false; // フェードアウト完了
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
    public bool IsFading => isFading; // フェード状態を外部から確認可能に

    public void HideImmediate()
    {
        if (isDestroyed || this == null) return;
        
        currentCts?.Cancel();
        currentCts?.Dispose();
        currentCts = null;
        
        isDisplaying = false;
        isFading = false; // フラグもリセット
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
        
        if (textCanvasGroup != null)
        {
            textCanvasGroup.alpha = 0f;
        }
        
        if (backgroundCanvasGroup != null)
        {
            backgroundCanvasGroup.alpha = 0f;
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
            float alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            
            canvasGroup.alpha = alpha;
            
            // テキストも同時にフェードイン
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = alpha;
            }
            
            // 背景も同時にフェードイン
            if (backgroundCanvasGroup != null)
            {
                backgroundCanvasGroup.alpha = alpha;
            }
            
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
            
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = 1f;
            }
            
            if (backgroundCanvasGroup != null)
            {
                backgroundCanvasGroup.alpha = 1f;
            }
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
            float alpha = Mathf.Clamp01(1f - (elapsed / fadeOutDuration));
            
            canvasGroup.alpha = alpha;
            
            // テキストも同時にフェードアウト
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = alpha;
            }
            
            // 背景も同時にフェードアウト
            if (backgroundCanvasGroup != null)
            {
                backgroundCanvasGroup.alpha = alpha;
            }
            
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
            
            if (textCanvasGroup != null)
            {
                textCanvasGroup.alpha = 0f;
            }
            
            if (backgroundCanvasGroup != null)
            {
                backgroundCanvasGroup.alpha = 0f;
            }
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

/// GameTextDisplayの拡張メソッド
public static class GameTextDisplayExtensions
{
    /// パーツ占有率に基づいてテキストを表示
    public static async UniTaskVoid ShowTextByPartsRatio(
        this GameTextDisplay textDisplay,
        PlayerPartsRatio partsRatio,
        ObjectTextData objectTextData,
        int objectID,
        float delayBetweenTexts = 1.5f,
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
                                     .Select(x => (PartsChara)x.Key)
                                     .ToList();
        
        if (showDebugLogs)
        {
            Debug.Log($"最大占有率のパーツ: {string.Join(", ", dominantParts)}");
        }
        
        // テキストリストを生成
        var textList = new List<string>();
        foreach (var parts in dominantParts)
        {
            // partsはPartsChara型
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
    
    // 複数のテキストを順次表示
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
                
                // 最後のテキストでない場合は待機してから非表示
                if (i < textList.Count - 1)
                {
                    await UniTask.Delay(
                        System.TimeSpan.FromSeconds(delayBetweenTexts),
                        cancellationToken: textDisplay.GetCancellationTokenOnDestroy()
                    );
                    
                    // 次のテキストを表示する前に現在のテキストを非表示
                    textDisplay.HideText();
                    
                    // フェードアウトが完了するまで待機
                    await UniTask.WaitUntil(() => !textDisplay.IsFading, 
                        cancellationToken: textDisplay.GetCancellationTokenOnDestroy());
                }
            }
        }
    }
}
