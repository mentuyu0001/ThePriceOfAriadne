using UnityEngine;
using Parts.Types;
using System.Collections.Generic;
using Unity.VisualScripting;

/// <summary>
/// マップ上に落ちているパーツの見た目を変更するスクリプト
/// </summary>

public class MapPartsVisualCustomizer : MonoBehaviour
{
    // マップ上のパーツの情報を保持するスクリプトの参照
    [SerializeField] private MapParts mapParts;

    // 主人公パーツの参照
    [SerializeField] private GameObject normalArmL;
    [SerializeField] private GameObject normalArmR;
    [SerializeField] private GameObject normalLegL;
    [SerializeField] private GameObject normalLegR;

    // 泥棒パーツの参照
    [SerializeField] private GameObject theifArmL;
    [SerializeField] private GameObject theifArmR;
    [SerializeField] private GameObject theifLegL;
    [SerializeField] private GameObject theifLegR;

    // 軍人パーツの参照
    [SerializeField] private GameObject muscleArmL;
    [SerializeField] private GameObject muscleArmR;
    [SerializeField] private GameObject muscleLegL;
    [SerializeField] private GameObject muscleLegR;   

    // アサシンパーツの参照
    [SerializeField] private GameObject assassinArmL;
    [SerializeField] private GameObject assassinArmR;
    [SerializeField] private GameObject assassinLegL;
    [SerializeField] private GameObject assassinLegR; 

    // 消防士パーツの参照
    [SerializeField] private GameObject fireArmL;
    [SerializeField] private GameObject fireArmR;
    [SerializeField] private GameObject fireLegL;
    [SerializeField] private GameObject fireLegR;

    public void Awake()
    {
        visuallizeMapParts();
    }
    
    // パーツの状態に応じて見た目を変更するメソッド
    public void visuallizeMapParts()
    {
        // 全てのパーツを非表示にする
        normalArmL.SetActive(false);
        normalArmR.SetActive(false);
        normalLegL.SetActive(false);
        normalLegR.SetActive(false);
        theifArmL.SetActive(false);
        theifArmR.SetActive(false);
        theifLegL.SetActive(false);
        theifLegR.SetActive(false);
        muscleArmL.SetActive(false);
        muscleArmR.SetActive(false);
        muscleLegL.SetActive(false);
        muscleLegR.SetActive(false);
        assassinArmL.SetActive(false);
        assassinArmR.SetActive(false);
        assassinLegL.SetActive(false);
        assassinLegR.SetActive(false);
        fireArmL.SetActive(false);
        fireArmR.SetActive(false);
        fireLegL.SetActive(false);
        fireLegR.SetActive(false);

        // パーツの状態に合うオブジェクトを取得
        GameObject obj = getTargetObject(mapParts.SlotType, mapParts.CharaType);

        // 存在確認
        if (obj == null)
        {
            Debug.LogError("不明なパーツ");
            return;
        }

        // パーツの表示
        obj.SetActive(true);
    }

    // パーツの装備箇所と種類を引数にし、対象のオブジェクトリストを取得するメソッド
    private GameObject getTargetObject(PartsSlot slot, PartsChara chara)
    {
        if (slot == PartsSlot.LeftArm) // 左腕
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return normalArmL;
                case PartsChara.Thief:
                    return theifArmL;
                case PartsChara.Muscle:
                    return muscleArmL;
                case PartsChara.Assassin:
                    return assassinArmL;
                case PartsChara.Fire:
                    return fireArmL;
                default:
                    return null;
            }
        }
        else if (slot == PartsSlot.RightArm) // 右腕
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return normalArmR;
                case PartsChara.Thief:
                    return theifArmR;
                case PartsChara.Muscle:
                    return muscleArmR;
                case PartsChara.Assassin:
                    return assassinArmR;
                case PartsChara.Fire:
                    return fireArmR;
                default:
                    return null;
            }
        }
        else if (slot == PartsSlot.LeftLeg) // 左脚
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return normalLegL;
                case PartsChara.Thief:
                    return theifLegL;
                case PartsChara.Muscle:
                    return muscleLegL;
                case PartsChara.Assassin:
                    return assassinLegL;
                case PartsChara.Fire:
                    return fireLegL;
                default:
                    return null;
            }
        }
        else if (slot == PartsSlot.RightLeg) // 右脚
        {
            switch (chara)
            {
                case PartsChara.Normal:
                    return normalLegR;
                case PartsChara.Thief:
                    return theifLegR;
                case PartsChara.Muscle:
                    return muscleLegR;
                case PartsChara.Assassin:
                    return assassinLegR;
                case PartsChara.Fire:
                    return fireLegR;
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
        GameObject preObject = getTargetObject(slot, preChara);

        // 交換後のパーツのオブジェクトリストを取得
        GameObject postObject = getTargetObject(slot, postChara);

        if (preObject == null || postObject == null)
        {
            Debug.LogError("不明なパーツ");
            return;
        }

        // 交換前のパーツを非表示にする
        preObject.SetActive(false);

        // 交換後のパーツを表示する
        postObject.SetActive(true);
    }
}
