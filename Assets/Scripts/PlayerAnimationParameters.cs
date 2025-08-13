using UnityEngine;

public class PlayerAnimationParameters : MonoBehaviour
{
    // PlayerのAnimatorを管理する変数
    private bool isJump = false;
    private bool isWalk = false;

    public bool IsJump {
        get { return isJump; }
        set { isJump = value; }
    }

    public bool IsWalk {
        get { return isWalk; }
        set { isWalk = value; }
    }
}
