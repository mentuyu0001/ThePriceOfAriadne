using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using VContainer;
using Parts.Types;
using UnityEngine.UI;

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

    [Inject]
    private PlayerPartsRatio partsRatio;

    private string inputText;
    private string[] textLines;
    private int currentLine = 0;
    private InputAction nextLineAction;
    private Sprite selectedEndingImage;
    private bool isTextCompleted = false;

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

    private void InitializeText()
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            textLines = inputText.Split('\n');
            if (textLines.Length > 0)
            {
                displayText.text = textLines[0];
            }
        }
    }

    private void ShowNextLine()
    {
        if (textLines == null) return;

        if (!isTextCompleted && currentLine + 1 < textLines.Length)
        {
            currentLine++;
            displayText.text = textLines[currentLine];
        }
        else if (!isTextCompleted && currentLine + 1 >= textLines.Length)
        {
            isTextCompleted = true;
            ShowEndingIllustration();
        }
    }

    private void ShowEndingIllustration()
    {
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
    }
}
