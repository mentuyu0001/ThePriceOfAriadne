using UnityEngine;

public class PlayerRunTimeStatus : MonoBehaviour
{
    /// <summary>
    /// パーツによって起こる、特殊行動のプロパティ
    /// </summary>
    
    private bool canDoubleJump = false; // 二段ジャンプ
    private bool canShootWater = false; // 水を撃つ
    private bool canThrowKnife = false; // ナイフを投げる

    public bool CanDoubleJump {
        get {return canDoubleJump;}
        set {canDoubleJump = value;}
    }

    public bool CanShootWater {
        get {return canShootWater;}
        set {canShootWater = value;}
    }

    public bool CanThrowKnife {
        get {return canThrowKnife;}
        set {canThrowKnife = value;}
    }

    public void ResetRunTimeStatus() {
        CanDoubleJump = false;
        CanThrowKnife = false;
    }
}
