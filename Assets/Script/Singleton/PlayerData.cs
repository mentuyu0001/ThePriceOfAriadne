using UnityEngine;

/// <summary>
/// プレイヤーが保持するデータ（部品など）を保持するシングルトン
/// </summary>

public class PlayerData : MonoBehaviour
{

    // 各部品を保持する変数
    private int leftArm = 0;
    private int rightArm = 0;
    private int leftLeg = 0;
    private int rightLeg = 0;

    // 各部品を取得、設定するプロパティ
    public int LeftArm {
        get { return leftArm; }
        set { leftArm = Mathf.Max(0, value); }
    }

    public int RightArm {
        get { return rightArm; }
        set { rightArm = Mathf.Max(0, value); }
    }

    public int LeftLeg {
        get { return leftLeg; }
        set { leftLeg = Mathf.Max(0, value); }
    }

    public int RightLeg {
        get { return rightLeg; }
        set { rightLeg = Mathf.Max(0, value); }
    }
}
