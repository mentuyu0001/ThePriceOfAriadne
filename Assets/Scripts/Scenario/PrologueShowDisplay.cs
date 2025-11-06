using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class PrologueShowDisplay : MonoBehaviour
{
    [SerializeField]
    private GameSceneManager gameSceneManager;

    [SerializeField, TextArea(3, 10)]
    private string inputText;

    [SerializeField]
    private TMP_Text displayText;

    [SerializeField] float letterDelay = 0.05f; // 1文字ごとの表示遅延時間

    [SerializeField]
    private FadeController fadeController;

    private string[] textLines;
    private int currentLine = 0;

    private InputAction nextLineAction;

    private bool isTextTyping = false; // テキストが表示中かどうかのフラグ
   
    private CancellationTokenSource cts;

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
    
        InitializeText();
    }

    private async void InitializeText()
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
        if (isTextTyping) // 表示途中で押されたら
        {
            cts.Cancel(); // 現在の表示をキャンセル
            displayText.text = textLines[currentLine]; // 現在の行を完全に表示
            isTextTyping = false;
            cts = new CancellationTokenSource(); // 新しいCancellationTokenSourceを作成
            return;
        }
        
        if (textLines != null && currentLine + 1 < textLines.Length)
        {
            currentLine++;
            await ShowLineLetterByLetter(textLines[currentLine], cts.Token).SuppressCancellationThrow();
        }
        else if (currentLine + 1 >= textLines.Length) // 最後の行が表示されている状態
        {
            // ステージ1へ移行
            fadeController.FadeOut(2.0f).Forget();
            await UniTask.Delay(2000); // フェードアウトの完了を待つ 
            gameSceneManager.LoadStage(1);  // 変更
        }
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
