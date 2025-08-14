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
    
    // 消防士に関するパーツ
    private bool canWalkOnFire; // 炎の上を歩けるかどうか
    private bool canShootWater; // 水を出せるかどうか

    // アサシンに関するパーツ
    private bool canDoubleJump; // 音を殺して歩けるかどうか
    private bool canThrowKnife; // ナイフを投げれるかどうか

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
        set { moveSpeed = value; }
    }

    public bool CanUnlock {
        get { return canUnlock; }
        set { canUnlock = value; }
    }

    public float JumpForce {
        get { return jumpForce; }
        set { jumpForce = value; }
    }

    public bool CanPushHeavyObject {
        get { return canPushHeavyObject; }
        set { canPushHeavyObject = value; }
    }

    public bool CanWalkOnFire {
        get { return canWalkOnFire; }
        set { canWalkOnFire = value; }
    }

    public bool CanShootWater {
        get { return canShootWater; }
        set { canShootWater = value; }
    }

    public bool CanDoubleJump {
        get { return canDoubleJump; }
        set { canDoubleJump = value; }
    }

    public bool CanThrowKnife {
        get { return canThrowKnife; }
        set { canThrowKnife = value; }
    }
}
