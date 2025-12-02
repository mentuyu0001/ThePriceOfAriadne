using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using Parts.Types;
using System;

/// <summary>
/// ゲーム中にテキストを表示するシステム
/// </summary>
public class GameTextDisplay : MonoBehaviour
{
    [Header("UI要素")]
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI messageText1;
    [SerializeField] private TextMeshProUGUI messageText2;
    [SerializeField] private GameObject textBackground; 
    
    [Header("表示設定")]
    private float fadeInDuration = 0.3f;
    private float fadeOutDuration = 0.3f;
    [SerializeField] private float characterDisplayInterval = 0.05f;
    
    [Header("デバッグ")]
    [SerializeField] private bool showDebugLogs = false;
    
    private CanvasGroup canvasGroup;
    private CanvasGroup backgroundCanvasGroup; // 背景用のCanvasGroup
    private CanvasGroup textCanvasGroup; // テキスト用のCanvasGroup
    private bool isDisplaying = false;
    private bool isFading = false;
    private bool isFaded = false;
    private CancellationTokenSource currentCts;
    private CancellationToken dct; // DestroyCancellationToken
    private bool isDestroyed = false;
    private float minDisplayDuration = 3.0f;
    private float displayCompleteTime = 0f;

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
        if (messageText1 != null)
        {
            textCanvasGroup = messageText1.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                textCanvasGroup = messageText1.gameObject.AddComponent<CanvasGroup>();
            }
        }
        if (messageText2 != null)
        {
            textCanvasGroup = messageText2.GetComponent<CanvasGroup>();
            if (textCanvasGroup == null)
            {
                textCanvasGroup = messageText2.gameObject.AddComponent<CanvasGroup>();
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

        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
        
        HideImmediate();
    }

    public async UniTask ShowText(string message1, string message2 = "", CancellationToken token = default)
    {
        if (isDestroyed || this == null) return;
   
        // フェード中(フェードイン・フェードアウト問わず)は新しいテキストを表示できない
        if (isFading)
        {
            if (showDebugLogs) Debug.Log("⚠️ フェード中のため表示をスキップ");
            return;
        }

        // 現在フェード中かを見る
        if (!isFaded)
        {
            isFaded = true;
        }
        else 
        {
            return;
        }

        // 表示中の場合も新規表示をブロック
        if (isDisplaying)
        {
            if (showDebugLogs) Debug.Log("⚠️ 既に表示中のため表示をスキップ");
            return;
        }
        
        if (textPanel == null || messageText1 == null || messageText2 == null || canvasGroup == null)
        {
            Debug.LogError("❌ UI要素がnullです");
            return;
        }

        currentCts = new CancellationTokenSource();
        CancellationToken currentCt = currentCts.Token;
        isDisplaying = true;
        isFading = true; // フェード開始

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, currentCt, dct);
        CancellationToken linkedToken = linkedCts.Token;

        try
        {
            if (messageText1 != null)
            {
                messageText1.text = message1; // 全文を即座に設定
            }
            if (messageText2 != null)
            {
                messageText2.text = message2;
            }
            
            if (textPanel != null)
            {
                textPanel.SetActive(true);
            }
            
            // フェードイン
            await FadeIn(linkedToken);
            
            isFading = false; // フェードイン完了
            
            // 表示を維持（HideTextが呼ばれるまで待機）
            displayCompleteTime = Time.time;
            isFaded = false;
        }
        catch (System.OperationCanceledException)
        {
            // キャンセル時は正常終了
            isFading = false;
            isDisplaying = false;
            isFaded = false;
        }
        catch (System.Exception e)
        {
            if (!isDestroyed && this != null)
            {
                Debug.LogError($"❌ テキスト表示中にエラー: {e.Message}");
            }
            isFading = false;
            isDisplaying = false;
            isFaded = false;
        }
    }

    public async UniTaskVoid HideText(CancellationToken token)
    {
        if (!isDisplaying || isDestroyed || this == null) return;
        
        // 既にフェード中なら何もしない
        if (isFading)
        {
            if (showDebugLogs) Debug.Log("⏳ フェードイン中のため、完了を待ちます...");
             // isFadingがfalseになるまで待機
             await UniTask.WaitWhile(() => isFading, cancellationToken: token);
        }
        
        float timeSinceDisplay = Time.time - displayCompleteTime;
        float remainingTime = minDisplayDuration - timeSinceDisplay;

        // まだ3秒経っていなければ、残りの時間だけ待機する
        if (remainingTime > 0)
        {
            if (showDebugLogs) Debug.Log($"⏳ 最低表示時間のため、あと {remainingTime:F2}秒 待機します");
            
            // ★重要: ここではまだ currentCts をキャンセルしてはいけない！
            // 待機中もキャンセル可能にするため linkedToken を使う
            using CancellationTokenSource waitCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(remainingTime), cancellationToken: waitCts.Token);
            }
            catch (OperationCanceledException)
            {
                return; // 待機中に親がキャンセルされたら終了
            }
        }

        isFading = true; // フェード開始
        
        // キャンセルトークンをキャンセル
        currentCts?.Cancel();
        currentCts?.Dispose();
        currentCts = null;

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;

        try
        {
            // フェードアウト
            await FadeOut(linkedToken);
            
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
            isFaded = false;
        }
    }

    private async UniTask ShowTextGradually(string fullText, CancellationToken token)
    {
        if (messageText1 == null || isDestroyed || this == null) return;

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            if (messageText1 == null || isDestroyed || this == null || linkedToken.IsCancellationRequested)
            {
                return;
            }
            
            messageText1.text = fullText.Substring(0, i);
            
            try
            {
                await UniTask.Delay(
                    System.TimeSpan.FromSeconds(characterDisplayInterval),
                    cancellationToken: linkedToken
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

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;

        float elapsed = 0f;
        while (elapsed < fadeInDuration && !linkedToken.IsCancellationRequested)
        {
            if (canvasGroup == null || isDestroyed || this == null || linkedToken.IsCancellationRequested)
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
                await UniTask.Yield(linkedToken);
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

        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;

        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        while (elapsed < fadeOutDuration && !linkedToken.IsCancellationRequested)
        {
            if (canvasGroup == null || isDestroyed || this == null || linkedToken.IsCancellationRequested)
            {
                return;
            }
            
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(startAlpha - (elapsed / fadeOutDuration));
            
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
                await UniTask.Yield(linkedToken);
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
        CancellationToken token,
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
        
        if(textList.Count == 4)
        {
            // 全て25%のときは特別なテキストを表示
            string allQuartersText = objectTextData.GetChimeraToneByID(objectID);

            if (showDebugLogs) Debug.Log("全て25%のため特別テキストを表示");
            await ShowTextsSequentially(
                textDisplay,
                new List<string> { allQuartersText },
                delayBetweenTexts,
                showDebugLogs,
                token
            );
            return;
        }
        
        // テキストを順次表示
        await ShowTextsSequentially(textDisplay, textList, delayBetweenTexts, showDebugLogs, token);
    }
    
    // 複数のテキストを順次表示
    private static async UniTask ShowTextsSequentially(
        GameTextDisplay textDisplay,
        List<string> textList,
        float delayBetweenTexts,
        bool showDebugLogs,
        CancellationToken token)
    {
        if (textList.Count == 1)
        {
            // 1つだけの場合は通常表示
            if (showDebugLogs) Debug.Log($"表示するテキスト:\n{textList[0]}");
            await textDisplay.ShowText(textList[0], token: token);
        }
        else
        {
            // 複数ある場合の表示
            if (showDebugLogs) Debug.Log($"表示するテキスト1:\n{textList[0]}");
            if (showDebugLogs) Debug.Log($"表示するテキスト2:\n{textList[1]}");
            await textDisplay.ShowText(textList[0], textList[1], token);
        }
    }
}
