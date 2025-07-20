using UnityEngine;
using Parts.Types;

/// <summary>
/// プレイヤーが装備しているパーツを保持するシングルトン
/// </summary>

public class PlayerParts : MonoBehaviour
{

    // 各部品を保持する変数
    private PartsChara leftArm = PartsChara.Normal;
    private PartsChara rightArm = PartsChara.Normal;
    private PartsChara leftLeg = PartsChara.Normal;
    private PartsChara rightLeg = PartsChara.Normal;

    // 各部品を取得、設定するプロパティ
    public PartsChara LeftArm {
        get { return leftArm; }
        set { leftArm = value; }
    }

    public PartsChara RightArm {
        get { return rightArm; }
        set { rightArm = value; }
    }

    public PartsChara LeftLeg {
        get { return leftLeg; }
        set { leftLeg = value; }
    }

    public PartsChara RightLeg {
        get { return rightLeg; }
        set { rightLeg = value; }
    }
}
