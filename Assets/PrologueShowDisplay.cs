using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PrologueShowDisplay : MonoBehaviour
{
    [SerializeField, TextArea(3, 10)]
    private string inputText;

    [SerializeField]
    private TMP_Text displayText;

    private string[] textLines;
    private int currentLine = 0;

    private InputAction nextLineAction;

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
        InitializeText();
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
        if (textLines != null && currentLine + 1 < textLines.Length)
        {
            currentLine++;
            displayText.text = textLines[currentLine];
        }
    }
}
