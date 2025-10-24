using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using VContainer;
using Parts.Types; // 追加

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
        partsRatios.Clear();

        List<PartsChara> allParts = new List<PartsChara>
        {
            playerParts.LeftArm,
            playerParts.RightArm,
            playerParts.LeftLeg,
            playerParts.RightLeg
        };

        var partsCounts = allParts.GroupBy(p => p)
                                   .ToDictionary(g => g.Key, g => g.Count());

        foreach (var parts in partsCounts)
        {
            float ratio = (parts.Value / 4f) * 100f;
            partsRatios[parts.Key] = ratio;
        }

        LogPartsRatio();
    }

    public float GetPartsRatio(PartsChara chara)
    {
        return partsRatios.ContainsKey(chara) ? partsRatios[chara] : 0f;
    }

    public PartsChara GetDominantParts()
    {
        if (partsRatios.Count == 0) return PartsChara.Normal;
        return partsRatios.OrderByDescending(x => x.Value).First().Key;
    }

    public PartsRatioState GetPartsRatioState(PartsChara chara)
    {
        float ratio = GetPartsRatio(chara);

        if (ratio >= 100f)
            return PartsRatioState.Full;
        else if (ratio >= 75f)
            return PartsRatioState.ThreeQuarters;
        else if (ratio >= 50f)
            return PartsRatioState.Half;
        else if (ratio >= 25f)
            return PartsRatioState.Quarter;
        else
            return PartsRatioState.None;
    }

    public List<PartsChara> GetPartsAboveRatio(float threshold)
    {
        return partsRatios.Where(x => x.Value >= threshold)
                          .Select(x => x.Key)
                          .ToList();
    }

    public Dictionary<PartsChara, float> GetAllRatios()
    {
        return new Dictionary<PartsChara, float>(partsRatios);
    }

    public bool HasFullRatioParts()
    {
        return partsRatios.Any(x => x.Value >= 100f);
    }

    public PartsChara GetFullRatioParts()
    {
        var fullParts = partsRatios.FirstOrDefault(x => x.Value >= 100f);
        return fullParts.Key;
    }

    private void LogPartsRatio()
    {
        Debug.Log("=== パーツ占有率 ===");
        foreach (var ratio in partsRatios)
        {
            PartsRatioState state = GetPartsRatioState(ratio.Key);
            Debug.Log($"{ratio.Key}: {ratio.Value}% ({state})");
        }
    }

    public bool IsAllQuarters()
    {
        return
            Mathf.Approximately(GetPartsRatio(PartsChara.Normal), 25f) &&
            Mathf.Approximately(GetPartsRatio(PartsChara.Thief), 25f) &&
            Mathf.Approximately(GetPartsRatio(PartsChara.Muscle), 25f) &&
            Mathf.Approximately(GetPartsRatio(PartsChara.Fire), 25f) &&
            Mathf.Approximately(GetPartsRatio(PartsChara.Assassin), 25f);
    }
}

/// <summary>
/// パーツ占有率の状態
/// </summary>
public enum PartsRatioState
{
    None,
    Quarter,
    Half,
    ThreeQuarters,
    Full
}
