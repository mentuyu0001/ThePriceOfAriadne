using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Parts.Types;
using VContainer;

/// <summary>
/// プレイヤーのパーツ占有率を管理するクラス
/// </summary>
public class PlayerPartsRatio : MonoBehaviour
{
    [Inject] private PlayerParts playerParts;
    
    private Dictionary<PartsChara, float> partsRatios = new Dictionary<PartsChara, float>();

    private void Start()
    {
        CalculatePartsRatio();
    }

    public void CalculatePartsRatio()
    {
        // 辞書をクリア
        partsRatios.Clear();

        // 4つのパーツを取得
        List<PartsChara> allParts = new List<PartsChara>
        {
            playerParts.LeftArm,
            playerParts.RightArm,
            playerParts.LeftLeg,
            playerParts.RightLeg
        };

        // 各パーツの出現回数をカウント
        var partsCounts = allParts.GroupBy(p => p)
                                   .ToDictionary(g => g.Key, g => g.Count());

        // 占有率を計算（各パーツの数 / 4 * 100）
        foreach (var parts in partsCounts)
        {
            float ratio = (parts.Value / 4f) * 100f;
            partsRatios[parts.Key] = ratio;
        }

        LogPartsRatio();
    }

    // 特定のパーツの占有率を取得
    public float GetPartsRatio(PartsChara chara)
    {
        return partsRatios.ContainsKey(chara) ? partsRatios[chara] : 0f;
    }

    // 最も占有率が高いパーツを取得
    public PartsChara GetDominantParts()
    {
        if (partsRatios.Count == 0) return PartsChara.Normal;
        
        return partsRatios.OrderByDescending(x => x.Value).First().Key;
    }

    // パーツ占有率の状態を取得（100%, 75%, 50%, 25%）
    public PartsRatioState GetPartsRatioState(PartsChara chara)
    {
        float ratio = GetPartsRatio(chara);

        if (ratio >= 100f)
            return PartsRatioState.Full;        // 100% (4/4)
        else if (ratio >= 75f)
            return PartsRatioState.ThreeQuarters; // 75% (3/4)
        else if (ratio >= 50f)
            return PartsRatioState.Half;        // 50% (2/4)
        else if (ratio >= 25f)
            return PartsRatioState.Quarter;     // 25% (1/4)
        else
            return PartsRatioState.None;        // 0% (0/4)
    }

    // 特定の占有率以上のパーツを取得
    public List<PartsChara> GetPartsAboveRatio(float threshold)
    {
        return partsRatios.Where(x => x.Value >= threshold)
                          .Select(x => x.Key)
                          .ToList();
    }

    // すべての占有率を取得
    public Dictionary<PartsChara, float> GetAllRatios()
    {
        return new Dictionary<PartsChara, float>(partsRatios);
    }

    // 100%のパーツがあるかチェック
    public bool HasFullRatioParts()
    {
        return partsRatios.Any(x => x.Value >= 100f);
    }

    // 100%のパーツを取得（なければNone）
    public PartsChara GetFullRatioParts()
    {
        var fullParts = partsRatios.FirstOrDefault(x => x.Value >= 100f);
        return fullParts.Key != default ? fullParts.Key : PartsChara.None;
    }

    // デバッグ用ログ
    private void LogPartsRatio()
    {
        Debug.Log("=== パーツ占有率 ===");
        foreach (var ratio in partsRatios)
        {
            PartsRatioState state = GetPartsRatioState(ratio.Key);
            Debug.Log($"{ratio.Key}: {ratio.Value}% ({state})");
        }
    }
}

/// <summary>
/// パーツ占有率の状態
/// </summary>
public enum PartsRatioState
{
    None,           // 0%
    Quarter,        // 25% (1/4)
    Half,           // 50% (2/4)
    ThreeQuarters,  // 75% (3/4)
    Full            // 100% (4/4)
}
