using UnityEngine;
using VContainer;
/// <summary>
/// 水をチャージするためのクラス
/// </summary>
public class WaterTank : MonoBehaviour
{
    [Inject] private PlayerStatus playerStatus; // プレイヤーのステータスを参照する
    [Inject] private PlayerRunTimeStatus playerRunTimeStatus; // プレイヤーのランタイムステータスを参照する

    public void ChargeWater()
    {
        // プレイヤーが水をチャージできるかどうかをチェック
        if (playerStatus != null && playerStatus.CanChargeWater)
        {
            Debug.Log("水をチャージしました！");

            // 水をチャージできる状態なら発射できるようにする
            playerRunTimeStatus.CanShootWater = true;

            // オプション：効果音やエフェクトの再生
            PlayChargeEffect();
        }
        else
        {
            Debug.Log("水をチャージできません。");
        }
    }
    
    private void PlayChargeEffect()
    {
        // 水をチャージする効果音やエフェクトを再生
        // AudioSource等を使用して実装
    }
}
