using UnityEngine;

public class PlayerAnimationParameters : MonoBehaviour
{
    /// <summary>
    /// プレイヤーアニメーションのtrue/falseを設定するマネージャー
    /// </summary>

    [SerializeField] private Animator playerAnimator;

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

    // レバーをおろしたりするなど、インタラクト的処理時のアニメーション
    public void AniInteractTrue()
    {
        playerAnimator.SetBool("InteractBool", true);
    }

    public void AniInteractFalse()
    {
        playerAnimator.SetBool("InteractBool", false);
    }
}
