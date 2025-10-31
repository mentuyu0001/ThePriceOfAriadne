using UnityEngine;
using Parts.Types;
using System.Collections.Generic;
using VContainer;
using Unity.VisualScripting;

/// <summary>
/// プレイヤーが装備しているパーツの見た目に変更するスクリプト
/// </summary>

public class PlayerVisualCustomizer : MonoBehaviour
{
    // シングルトンの取得
    [SerializeField] private PlayerParts playerParts;

    // 主人公パーツの参照
    [SerializeField] private List<GameObject> NormalArmL;
    [SerializeField] private List<GameObject> NormalArmR;
    [SerializeField] private List<GameObject> NormalLegL;
    [SerializeField] private List<GameObject> NormalLegR;

    // 泥棒パーツの参照
    [SerializeField] private List<GameObject> TheifArmL;
    [SerializeField] private List<GameObject> TheifArmR;
    [SerializeField] private List<GameObject> TheifLegL;
    [SerializeField] private List<GameObject> TheifLegR;

    // 軍人パーツの参照
    [SerializeField] private List<GameObject> MuscleArmL;
    [SerializeField] private List<GameObject> MuscleArmR;
    [SerializeField] private List<GameObject> MuscleLegL;
    [SerializeField] private List<GameObject> MuscleLegR;   

    // アサシンパーツの参照
    [SerializeField] private List<GameObject> AssassinArmL;
    [SerializeField] private List<GameObject> AssassinArmR;
    [SerializeField] private List<GameObject> AssassinLegL;
    [SerializeField] private List<GameObject> AssassinLegR; 

    // 消防士パーツの参照
    [SerializeField] private List<GameObject> FireArmL;
    [SerializeField] private List<GameObject> FireArmR;
    [SerializeField] private List<GameObject> FireLegL;
    [SerializeField] private List<GameObject> FireLegR;

    public void Awake()
    {
        visuallizePlayerParts();
    }
    
    // プレイヤーの装備しているパーツに応じて見た目を変更するメソッド
    public void visuallizePlayerParts()
    {
        // 全てのパーツを格納したリストを構築
        List<GameObject> allParts = new List<GameObject>();
        allParts.AddRange(NormalArmL);
        allParts.AddRange(NormalArmR);
        allParts.AddRange(NormalLegL);
        allParts.AddRange(NormalLegR);
        allParts.AddRange(TheifArmL);
        allParts.AddRange(TheifArmR);
        allParts.AddRange(TheifLegL);
        allParts.AddRange(TheifLegR);
        allParts.AddRange(MuscleArmL);
        allParts.AddRange(MuscleArmR);
        allParts.AddRange(MuscleLegL);
        allParts.AddRange(MuscleLegR);
        allParts.AddRange(AssassinArmL);
        allParts.AddRange(AssassinArmR);
        allParts.AddRange(AssassinLegL);
        allParts.AddRange(AssassinLegR);
        allParts.AddRange(FireArmL);
        allParts.AddRange(FireArmR);
        allParts.AddRange(FireLegL);
        allParts.AddRange(FireLegR);

        // 全てのパーツを非表示にする
        foreach (GameObject obj in allParts)
        {
            obj.SetActive(false);
        }

        // 装備しているパーツのオブジェクトリストを取得
        List<GameObject> leftArms = getTargetObject(PartsSlot.LeftArm, playerParts.LeftArm);
        List<GameObject> rightArms = getTargetObject(PartsSlot.RightArm, playerParts.RightArm);
        List<GameObject> leftLegs = getTargetObject(PartsSlot.LeftLeg, playerParts.LeftLeg);
        List<GameObject> rightLegs = getTargetObject(PartsSlot.RightLeg, playerParts.RightLeg);

        // 存在確認
        if (leftArms == null || rightArms == null || leftLegs == null || rightLegs == null)
        {
            Debug.LogError("不明な装備");
            return;
        }

        // 左腕の表示
        foreach (GameObject obj in leftArms)
        {
            obj.SetActive(true);
        }
        // 右腕の表示
        foreach (GameObject obj in rightArms)
        {
            obj.SetActive(true);
        }
        // 左脚の表示
        foreach (GameObject obj in leftLegs)
        {
            obj.SetActive(true);
        }
        // 右脚の表示
        foreach (GameObject obj in rightLegs)
        {
            obj.SetActive(true);
        }
    }

    // パーツの装備箇所と種類を引数にし、対象のオブジェクトリストを取得するメソッド
    private List<GameObject> getTargetObject(PartsSlot slot, PartsChara chara)
    {
        if (slot == PartsSlot.LeftArm) // 左腕
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return NormalArmL;
                case PartsChara.Thief:
                    return TheifArmL;
                case PartsChara.Muscle:
                    return MuscleArmL;
                case PartsChara.Assassin:
                    return AssassinArmL;
                case PartsChara.Fire:
                    return FireArmL;
                default:
                    return null;
            }
        }
        else if (slot == PartsSlot.RightArm) // 右腕
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return NormalArmR;
                case PartsChara.Thief:
                    return TheifArmR;
                case PartsChara.Muscle:
                    return MuscleArmR;
                case PartsChara.Assassin:
                    return AssassinArmR;
                case PartsChara.Fire:
                    return FireArmR;
                default:
                    return null;
            }
        }
        else if (slot == PartsSlot.LeftLeg) // 左脚
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return NormalLegL;
                case PartsChara.Thief:
                    return TheifLegL;
                case PartsChara.Muscle:
                    return MuscleLegL;
                case PartsChara.Assassin:
                    return AssassinLegL;
                case PartsChara.Fire:
                    return FireLegL;
                default:
                    return null;
            }
        }
        else if (slot == PartsSlot.RightLeg) // 右脚
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return NormalLegR;
                case PartsChara.Thief:
                    return TheifLegR;
                case PartsChara.Muscle:
                    return MuscleLegR;
                case PartsChara.Assassin:
                    return AssassinLegR;
                case PartsChara.Fire:
                    return FireLegR;
                default:
                    return null;
            }
        }

        return null;
    }

    // パーツの装備箇所と交換前後の種類を引数にし、見た目を変更するメソッド
    public void ChangeVisual(PartsSlot slot, PartsChara preChara, PartsChara postChara)
    {
        // 交換前のパーツのオブジェクトリストを取得
        List<GameObject> preObjects = getTargetObject(slot, preChara);

        // 交換後のパーツのオブジェクトリストを取得
        List<GameObject> postObjects = getTargetObject(slot, postChara);

        if (preObjects == null || postObjects == null)
        {
            Debug.LogError("不明な装備");
            return;
        }

        // 交換前のパーツを非表示にする
        foreach (GameObject obj in preObjects)
        {
            obj.SetActive(false);
        }

        // 交換後のパーツを表示する
        foreach (GameObject obj in postObjects)
        {
            obj.SetActive(true);
        }
    }
}
