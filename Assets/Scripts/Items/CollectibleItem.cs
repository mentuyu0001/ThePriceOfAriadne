using UnityEngine;
using VContainer;
using Cysharp.Threading.Tasks;
using Parts.Types;
using System.Collections.Generic;
using System.Linq;
public class CollectibleItem : MonoBehaviour
{
    /// <summary>
    /// コレクター要素のあるアイテムの取得に関するスクリプト
    /// </summary>

    //アイテムのID(外部キーの役割)
    [SerializeField] private int itemID;
    [SerializeField] private float textDisplayDuration = 1f; // テキスト表示秒数

    // ItemManagerの参照
    [Inject] private ItemManager itemManager;
    // ItemDataの参照 
    [Inject] private ItemData itemData;
    // PlayerPartsRatioの参照
    [Inject] private PlayerPartsRatio partsRatio;
    // GameTextDisplayの参照
    [Inject] private GameTextDisplay gameTextDisplay;

    // アイテム名
    private string itemName;
    // アイテムの説明文
    private string itemDescription;
    // アイテムのスプライト
    private string itemText;

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
            Debug.Log($"取得したアイテム: {GetItemName()}");
            Debug.Log($"アイテムテキスト: {GetItemText()}");


            var item = itemData.GetItemByID(itemID);
            var descriptions = item?.descriptions;
            if (item == null || descriptions == null)
            {
                Debug.LogWarning("アイテム情報または説明文がありません");
                return;
            }
            
            CollectItem();
            Destroy(gameObject);

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

            // 全て25%のとき
            if (dominantParts.Count == 4)
            {
                textList.Add(descriptions.allQuartersTone);
            }
            // 50%:50%のとき
            else if (dominantParts.Count == 2)
            {
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
                textList.Add(descriptions.ownFullTone);
            }
            // 通常パターン
            else
            {
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

            // 共通の表示処理
            if (gameTextDisplay != null && textList.Count > 0)
            {
                await ShowTextsSequentiallyFromItemData(
                    gameTextDisplay,
                    textList.Select((_, i) => dominantParts.ElementAtOrDefault(i)).ToList(), // charaListは使わないが型合わせ
                    2f // ディレイ
                );
            }
        }
    }
    
    private async UniTask ShowTextsSequentiallyFromItemData(
        GameTextDisplay textDisplay,
        List<PartsChara> charaList,
        float delayBetweenTexts = 2f,
        bool showDebugLogs = false
    )
    {
        var item = itemData.GetItemByID(itemID);
        var descriptions = item?.descriptions;
        if (descriptions == null) return;

        var textList = new List<string>();
        foreach (var chara in charaList)
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
        textList = textList.Distinct().ToList();

        for (int i = 0; i < textList.Count; i++)
        {
            if (showDebugLogs) Debug.Log($"表示するテキスト ({i + 1}/{textList.Count}):\n{textList[i]}");
            await textDisplay.ShowText(textList[i]);
            // フェードインが完了するまで待機
            await UniTask.WaitUntil(() => !textDisplay.IsFading);
            // 表示秒数だけ待機
            await UniTask.Delay(System.TimeSpan.FromSeconds(textDisplayDuration));
            // テキストを非表示
            textDisplay.HideText();
            // フェードアウトが完了するまで待機（最後以外）
            if (i < textList.Count - 1)
            {
                await UniTask.WaitUntil(() => !textDisplay.IsFading);
                // テキスト間のディレイ
                await UniTask.Delay(System.TimeSpan.FromSeconds(delayBetweenTexts));
            }
        }
    }

}
