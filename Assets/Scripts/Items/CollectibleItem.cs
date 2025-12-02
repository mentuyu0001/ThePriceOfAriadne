using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using Parts.Types;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro; 
using System.Threading;

public class CollectibleItem : MonoBehaviour
{
    /// <summary>
    /// コレクター要素のあるアイテムの取得に関するスクリプト
    /// </summary>

    //アイテムのID(外部キーの役割)
    [SerializeField] private int itemID;
    private float textDisplayDuration = 0f; // テキスト表示秒数
    // アイテムのスプライトレンダラー
    [SerializeField] private SpriteRenderer spriteRenderer;

    // ItemManagerの参照
    [Inject] private ItemManager itemManager;
    // ItemDataの参照 
    [Inject] private ItemData itemData;

    // GameTextDisplayの参照
    [Inject] private GameTextDisplay gameTextDisplay;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI messageText1;
    [SerializeField] private TextMeshProUGUI messageText2;
    [SerializeField] private GameObject textBackground; 

    // アイテム名
    private string itemName;
    // アイテムの説明文
    private string itemDescription;
    // アイテムのスプライト
    private string itemText;

    // アイテム獲得フラグ
    private bool isCollected = false;

    private PlayerPartsRatio partsRatio;

    private CancellationToken dct; // DestroyCancellationToken

    private void Start()
    {
        partsRatio = GameObject.Find("PlayerPartsRatio").GetComponent<PlayerPartsRatio>();

        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    // アイテムを取得情報をインベントリに保存するメソッド
    public void CollectItem()
    {
        itemManager.ObtainItem(itemID);
    }

    // アイテム名を取得するメソッド
    public string GetItemName()
    {
        return itemData.GetItemNameByID(itemID);
    }

    // アイテムのテキストを表示するメソッド
    public string GetItemText()
    {
        return itemData.GetItemTextByID(itemID);
    }

    // プレイヤーが触れた時の処理など
    async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCollected) return;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(4);
            }

            isCollected = true;

            Debug.Log($"取得したアイテム: {GetItemName()}");
            Debug.Log($"アイテムテキスト: {GetItemText()}");


            var item = itemData.GetItemByID(itemID);
            var descriptions = item?.descriptions;
            if (item == null || descriptions == null)
            {
                Debug.LogWarning("アイテム情報または説明文がありません");
                return;
            }

            partsRatio.CalculatePartsRatio();
            var allRatios = partsRatio.GetAllRatios();
            if (allRatios.Count == 0)
            {
                Debug.LogWarning("パーツ占有率が取得できませんでした");
                return;
            }
            float maxRatio = allRatios.Values.Max();
            var dominantParts = allRatios.Where(x => x.Value == maxRatio)
                                         .Select(x => (PartsChara)x.Key)
                                         .ToList();

            List<string> textList = new List<string>();

            spriteRenderer.enabled = false; // アイテムのスプライトを非表示にする

            UnityEngine.Debug.Log("dominantParts.Coount: " + dominantParts.Count);
            // 全て25%のとき
            if (dominantParts.Count == 4)
            {
                CollectItem();
                if (gameTextDisplay != null)
                {
                    var allQuartersText = itemData.GetAllQuartersTone(itemID);
                    
                    // パネルとバックグラウンドを表示
                    //textPanel.SetActive(true);
                    //textBackground.SetActive(true);

                    // キメラ状態用の特別なテキストを表示
                    await ShowTextsSequentially(
                        gameTextDisplay,
                        new List<string> { allQuartersText },
                        2f,
                        true 
                    );

                    // 表示時間待機
                    await WaitAndDestroy();

                    // テキストをクリアして非表示
                    //textPanel.SetActive(false);
                    //textBackground.SetActive(false);
                } else
                {
                    Destroy(gameObject);
                }
                return;
            }
            // 50%:50%のとき
            else if (dominantParts.Count == 2)
            {
                Debug.Log($"テキストリスト：{dominantParts.Count}");
                foreach (var chara in dominantParts.Distinct())
                {
                    switch (chara)
                    {
                        case PartsChara.Normal:
                            textList.Add(descriptions.playerTone);
                            break;
                        case PartsChara.Thief:
                            textList.Add(descriptions.theifTone);
                            break;
                        case PartsChara.Muscle:
                            textList.Add(descriptions.muscleTone);
                            break;
                        case PartsChara.Fire:
                            textList.Add(descriptions.fireTone);
                            break;
                        case PartsChara.Assassin:
                            textList.Add(descriptions.assassinTone);
                            break;
                    }
                }
            }
            // 100%かつアイテム所有者と一致
            else if (Mathf.Approximately(maxRatio, 100f) && dominantParts.Count == 1 && dominantParts[0].ToString() == item.ownerType.ToString())
            {
                CollectItem();
                if (gameTextDisplay != null)
                {
                    textList.Add(descriptions.ownFullTone);
                    await ShowOwnTextsSequentiallyFromItemData(
                    gameTextDisplay,
                    textList.Select((_, i) => dominantParts.ElementAtOrDefault(i)).ToList(), // charaListは使わないが型合わせ
                    2f // ディレイ
                );
                await WaitAndDestroy();
                }
                else
                {
                    Destroy(gameObject);
                }
                return;
            }
            // 通常パターン
            else
            {
                Debug.Log($"テキストリスト：{dominantParts.Count}");
                var maxPartsChara = partsRatio.GetDominantParts();
                switch (maxPartsChara)
                {
                    case PartsChara.Normal:
                        textList.Add(descriptions.playerTone);
                        break;
                    case PartsChara.Thief:
                        textList.Add(descriptions.theifTone);
                        break;
                    case PartsChara.Muscle:
                        textList.Add(descriptions.muscleTone);
                        break;
                    case PartsChara.Fire:
                        textList.Add(descriptions.fireTone);
                        break;
                    case PartsChara.Assassin:
                        textList.Add(descriptions.assassinTone);
                        break;
                    default:
                        textList.Add(descriptions.playerTone);
                        break;
                }
            }
            CollectItem();
            // 共通の表示処理
            if (gameTextDisplay != null && textList.Count > 0)
            {
                await ShowTextsSequentiallyFromItemData(
                    gameTextDisplay,
                    textList.Select((_, i) => dominantParts.ElementAtOrDefault(i)).ToList(), // charaListは使わないが型合わせ
                    2f // ディレイ
                );

                await WaitAndDestroy();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private async UniTask WaitAndDestroy()
    {
        // もし「最低3秒」よりさらに長く表示させたい場合だけここを使う
        if (textDisplayDuration > 0)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(textDisplayDuration), cancellationToken: dct);
        }

        // GameTextDisplayに「もう隠していいよ」と伝える
        gameTextDisplay.HideText(dct).Forget();

        // ★ここが大事: GameTextDisplayが「フェードアウト完了して非表示状態(IsDisplaying == false)」になるまで待つ
        // これを待たずにDestroyすると、GameTextDisplayの処理がキャンセルされてパッと消えてしまう
        await UniTask.WaitWhile(() => gameTextDisplay.IsDisplaying, cancellationToken: dct);

        // 完全に消えたのを見届けてから、自分を消す
        Destroy(gameObject);
    }
    
    private async UniTask ShowTextsSequentiallyFromItemData(
        GameTextDisplay textDisplay,
        List<PartsChara> charaList,
        float delayBetweenTexts = 0f,
        bool showDebugLogs = false
    )
    {
        var item = itemData.GetItemByID(itemID);
        var descriptions = item?.descriptions;
        if (descriptions == null) return;

        var textList = new List<string>();
        foreach (var chara in charaList)
        {
            string text = itemData.GetToneTextByPartsChara(itemID, chara);
            if (!string.IsNullOrEmpty(text))
            {
                textList.Add(text);
            }
        }
        textList = textList.Distinct().ToList();

        // パネルとバックグラウンドを表示
        //textPanel.SetActive(true);
        //textBackground.SetActive(true);

        // ShowTextsSequentiallyを使用してテキストを表示
        await ShowTextsSequentially(textDisplay, textList, delayBetweenTexts, showDebugLogs);

        // 表示時間待機
        //await UniTask.Delay(System.TimeSpan.FromSeconds(textDisplayDuration), cancellationToken: dct);

        // テキストをクリアして非表示
        //textPanel.SetActive(false);
        //textBackground.SetActive(false);
    }

    private async UniTask ShowOwnTextsSequentiallyFromItemData(
        GameTextDisplay textDisplay,
        List<PartsChara> charaList,
        float delayBetweenTexts = 0f,
        bool showDebugLogs = false
    )
    {
        var item = itemData.GetItemByID(itemID);
        var descriptions = item?.descriptions;
        if (descriptions == null) return;

        var textList = new List<string>();
        foreach (var chara in charaList)
        {
            string text = itemData.GetOwnFullTone(itemID);
            if (!string.IsNullOrEmpty(text))
            {
                textList.Add(text);
            }
        }
        textList = textList.Distinct().ToList();

        // パネルとバックグラウンドを表示
        //textPanel.SetActive(true);
        //textBackground.SetActive(true);

        // ShowTextsSequentiallyを使用してテキストを表示
        await ShowTextsSequentially(textDisplay, textList, delayBetweenTexts, showDebugLogs);

        // 表示時間待機
        //await UniTask.Delay(System.TimeSpan.FromSeconds(textDisplayDuration), cancellationToken: dct);

        // テキストをクリアして非表示
        //textPanel.SetActive(false);
        //textBackground.SetActive(false);
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
            // 複数ある場合の表示
            if (showDebugLogs) Debug.Log($"表示するテキスト1:\n{textList[0]}");
            if (showDebugLogs) Debug.Log($"表示するテキスト2:\n{textList[1]}");
            await textDisplay.ShowText(textList[0], textList[1]);
        }
    }
}
