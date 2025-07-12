using UnityEngine;

/// <summary>
/// プレイヤーの能力を保存するシングルトン
/// </summary>

public class PlayerStatus : MonoBehaviour
{

    // 各部品を保持する変数
    // 泥棒に関するパーツ
    private float friction; // 移動速度に応じた摩擦
    private float airResistance; // 移動速度に応じた空気抵抗
    private float moveSpeed; // 移動速度
    private bool canUnlock; // 鍵開け能力

    // ムキムキマッチョマンに関するパーツ
    private float jumpForce; // ジャンプ力
    private bool canPushHeavyObject; // 重いものを動かせるかどうか

    // プロパティ
    public float Friction {
        get { return friction; }
        set { friction = value; }
    }

    public float AirResistance {
        get { return airResistance; }
        set { airResistance = value; }
    }

    public float MoveSpeed {
        get { return moveSpeed; }
        set { moveSpeed = Mathf.Max(0f, value); }
    }

    public bool CanUnlock {
        get { return canUnlock; }
        set { canUnlock = value; }
    }

    public float JumpForce {
        get { return jumpForce; }
        set { jumpForce = Mathf.Max(0f, value); }
    }

    public bool CanPushHeavyObject {
        get { return canPushHeavyObject; }
        set { canPushHeavyObject = value; }
    }
}
