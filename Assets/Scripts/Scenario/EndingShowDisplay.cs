using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using VContainer;
using Parts.Types;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using System.Threading;

public class EndingShowDisplay : MonoBehaviour
{
    [SerializeField, TextArea(3, 10)]
    private string normalEnding;
    [SerializeField, TextArea(3, 10)]
    private string thiefEnding;
    [SerializeField, TextArea(3, 10)]
    private string mussuleEnding;
    [SerializeField, TextArea(3, 10)]
    private string fireEnding;
    [SerializeField, TextArea(3, 10)]
    private string assasinEnding;
    [SerializeField, TextArea(3, 10)]
    private string chimeraEnding;

    [SerializeField]
    private TMP_Text displayText;

    [SerializeField]
    private Sprite normalEndingImage;
    [SerializeField]
    private Sprite thiefEndingImage;
    [SerializeField]
    private Sprite mussuleEndingImage;
    [SerializeField]
    private Sprite fireEndingImage;
    [SerializeField]
    private Sprite assasinEndingImage;
    [SerializeField]
    private Sprite chimeraEndingImage;

    [SerializeField]
    private Image endingIllustration;  // エンディングイラスト表示用

    [SerializeField]
    private GameObject blackBackground;  // 黒背景
    [SerializeField]
    private GameObject whiteBackground;  // 白背景

    [SerializeField]
    private FadeController fadeController;

    [SerializeField] float fadeDelay = 3.0f; // フェードイン・アウトの時間

    [SerializeField] float letterDelay = 0.05f; // 1文字ごとの表示遅延時間

    [SerializeField]
    private PlayerPartsRatio partsRatio;

    [SerializeField]
    private SoundManager soundManager;
    [SerializeField] private int bgmIndex;

    [SerializeField] private float fadeInDuration;

    private string inputText;
    private string[] textLines;
    private int currentLine = 0;
    private InputAction nextLineAction;
    private Sprite selectedEndingImage;
    private bool isTextCompleted = false;
    private bool isIllustrationShown = false;

    private bool isTextTyping = false; // テキストが表示中かどうかのフラグ
   
    private CancellationTokenSource cts;

    private GameSceneManager sceneManager;

    private bool isFading = false;

    [Inject]
    public void Initialize(PlayerPartsRatio ratio)
    {
        partsRatio = ratio;
    }

    private void Awake()
    {
        // 入力アクションの設定（interactionsを"tap"に変更）
        nextLineAction = new InputAction(binding: "<Keyboard>/enter");
        nextLineAction.AddBinding("<Mouse>/leftButton");
        
        nextLineAction.performed += ctx => ShowNextLine();
        
        nextLineAction.Enable();
    }

    private void OnDestroy()
    {
        // クリーンアップ
        nextLineAction.Disable();
        nextLineAction.Dispose();
    }

    private void Start()
    {
        // CancellationTokenSourceの生成  
        cts = new CancellationTokenSource();

        sceneManager = FindObjectOfType<GameSceneManager>();

        // 初期設定
        if (endingIllustration != null)
        {
            endingIllustration.gameObject.SetActive(false);
        }
        
        // 背景の初期設定
        if (whiteBackground != null)
        {
            whiteBackground.SetActive(false);
        }
        if (blackBackground != null)
        {
            blackBackground.SetActive(true);
        }
        
        DetermineEnding();
        InitializeText();
    }

    private void DetermineEnding()
    {
        if (!partsRatio.HasFullRatioParts())
        {
            inputText = chimeraEnding;
            selectedEndingImage = chimeraEndingImage;
            return;
        }

        PartsChara fullParts = partsRatio.GetFullRatioParts();
        (inputText, selectedEndingImage) = fullParts switch
        {
            PartsChara.Normal => (normalEnding, normalEndingImage),
            PartsChara.Thief => (thiefEnding, thiefEndingImage),
            PartsChara.Muscle => (mussuleEnding, mussuleEndingImage),
            PartsChara.Fire => (fireEnding, fireEndingImage),
            PartsChara.Assassin => (assasinEnding, assasinEndingImage),
            _ => (chimeraEnding, chimeraEndingImage)
        };

        Debug.Log($"Selected Ending for {fullParts}: {inputText}");
    }

    private async Task InitializeText()
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            textLines = inputText.Split('\n');
            if (textLines.Length > 0)
            {
                await ShowLineLetterByLetter(textLines[0], cts.Token).SuppressCancellationThrow();
            }
        }
    }

    private async void ShowNextLine()
    {
        if (textLines == null) return;

        if (isIllustrationShown)
        {
            UnityEngine.Debug.Log("Ending display completed. Transitioning to title.");
            FadeOutAndLoadTitle();
            return;
        }

        if (isTextTyping) // 表示途中で押されたら
        {
            cts.Cancel(); // 現在の表示をキャンセル
            displayText.text = textLines[currentLine]; // 現在の行を完全に表示
            isTextTyping = false;
            cts = new CancellationTokenSource(); // 新しいCancellationTokenSourceを作成
            return;
        }

        if (!isTextCompleted && currentLine + 1 < textLines.Length)
        {
            currentLine++;
            await ShowLineLetterByLetter(textLines[currentLine], cts.Token).SuppressCancellationThrow();
        }
        else if (!isTextCompleted && currentLine + 1 >= textLines.Length)
        {
            isTextCompleted = true;

            await ShowEndingIllustration(cts.Token).SuppressCancellationThrow();
        }
    }

    private async UniTask ShowEndingIllustration(CancellationToken token)
    {
        fadeController.FadeIn(fadeDelay).Forget();

        // soundManager.PlayBGMFadeIn(2, fadeDelay - 0.5f).Forget();

        // テキストを非表示
        displayText.gameObject.SetActive(false);
        
        // 背景を切り替え
        if (blackBackground != null) blackBackground.SetActive(false);
        if (whiteBackground != null) whiteBackground.SetActive(true);

        // エンディングイラストを表示
        if (endingIllustration != null && selectedEndingImage != null)
        {
            endingIllustration.gameObject.SetActive(true);
            endingIllustration.sprite = selectedEndingImage;
        }
        isIllustrationShown = true;
        // BGMフェードイン
        await soundManager.PlayBGMFadeIn(bgmIndex, fadeInDuration, token, true);
    }

    private async void FadeOutAndLoadTitle()
    {
        if (isFading) return;
        isFading = true;
        cts.Cancel(); // もし何か処理中ならキャンセル
        sceneManager.LoadTitle();
    }
    
    private async UniTask ShowLineLetterByLetter(string line, CancellationToken token)
    {
        isTextTyping = true;
        int lineLength = line.Length;
        displayText.text = "";
        for (int i = 0; i < lineLength; i++)
        {
            displayText.text += line[i];
            await UniTask.Delay(TimeSpan.FromSeconds(letterDelay), cancellationToken: token);
        }
        isTextTyping = false;
    }
}
