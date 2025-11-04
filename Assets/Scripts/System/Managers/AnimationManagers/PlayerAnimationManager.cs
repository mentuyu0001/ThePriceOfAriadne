using UnityEngine;
using VContainer;

public class PlayerAnimationManager : MonoBehaviour
{
    /// <summary>
    /// プレイヤーアニメーションのtrue/falseを設定するマネージャー
    /// </summary>

    [Inject] private Animator playerAnimator;

    // 歩くアニメーション
    public void AniWalkTrue()
    {
        playerAnimator.SetBool("WalkBool", true);
    }

    public void AniWalkFalse()
    {
        playerAnimator.SetBool("WalkBool", false);
    }

    // ジャンプアニメーション
    public void AniJumpTrue()
    {
        playerAnimator.SetBool("JumpBool", true);
    }

    public void AniJumpFalse()
    {
        playerAnimator.SetBool("JumpBool", false);
    }

    // Pushアニメーション
    public void AniPushTrue()
    {
        playerAnimator.SetBool("PushBool", true);
    }
    public void AniPushFalse()
    {
        playerAnimator.SetBool("PushBool", false);
    }

    // ダブルジャンプアニメーション

    public void AniDoubleJumpTrue()
    {
        playerAnimator.SetBool("DoubleJumpBool", true);
    }
    public void AniDoubleJumpFalse()
    {
        playerAnimator.SetBool("DoubleJumpBool", false);
    }

    // インタラクト的処理時のアニメーション
    public void AniInteractTrue()
    {
        playerAnimator.SetBool("InteractTrigger", true);
    }

    // レバーのアニメーション
    public void AniLeverTrue()
    {
        playerAnimator.SetBool("LeverTrigger", true);
    }

    // ナイフのアニメーション
    public void AniKnifeTrue()
    {
        playerAnimator.SetBool("ThrowKnifeTrigger", true);
    }

    // 消火のアニメーション
    public void AniShootWaterTrue()
    {
        playerAnimator.SetBool("ShootWaterTrigger", true);
    }

    // ボタンのアニメーション
    public void AniButtonTrue()
    {
        playerAnimator.SetBool("ButtonTrigger", true);
    }

    public bool GetHeavyBool()
    {
        return playerAnimator.GetBool("PushBool");
    }

}
