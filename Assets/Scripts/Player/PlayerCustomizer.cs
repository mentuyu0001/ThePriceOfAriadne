using UnityEngine;
using Parts.Types;
using VContainer;

/// <summary>
/// プレイヤーが装備しているパーツと能力を設定するスクリプト
/// </summary>

public class PlayerCustomizer : MonoBehaviour
{
    // シングルトンの取得
    [Inject] private PlayerParts playerParts;
    [Inject] private PlayerStatus playerStatus;

    // Controllerの取得
    [Inject] private Controller controller;

    // データファイルの取得
    [Inject] private PlayerStatusData statusData;

    // RunTimeStatusの取得
    [Inject] private PlayerRunTimeStatus runTimeStatus;

    // PlayerVisualCustomizerの取得
    [SerializeField] private PlayerVisualCustomizer visualCustomizer;

    private void Awake() {
        ChangePlayerStatus();
        controller.SetStatus();
    }

    // プレイヤーの能力値を初期状態にリセットする関数
    private void resetStatus() {
        playerStatus.Friction = statusData.NormalFriction;
        playerStatus.AirResistance = statusData.NormalAirResistance;
        playerStatus.MoveSpeed = statusData.NormalMoveSpeed;
        playerStatus.CanUnlock = statusData.NormalCanUnlock;
        playerStatus.JumpForce = statusData.NormalJumpForce;
        playerStatus.CanPushHeavyObject = statusData.NormalCanPushHeavyObject;
        playerStatus.CanWalkOnFire = statusData.NormalCanWalkOnFire;
        playerStatus.CanShootWater = statusData.NormalCanShootWater;
        playerStatus.CanDoubleJump = statusData.NormalCanDoubleJump;
        playerStatus.CanThrowKnife = statusData.NormalCanThrowKnife;
        runTimeStatus.ResetRunTimeStatus();
    }

    // 装備したパーツの能力を設定する関数
    private void ChangePlayerStatus() {
        
        // 能力をリセットする
        resetStatus();

        // 能力を再計算する
        // 左腕の計算
        switch(playerParts.LeftArm) {
            case PartsChara.Normal:
                break;

            case PartsChara.Thief:
                playerStatus.CanUnlock = statusData.ThiefCanUnlock;
                break;

            case PartsChara.Muscle:
                playerStatus.CanPushHeavyObject = statusData.MuscleCanPushHeavyObject;
                break;
            //ShotWater->ChargeWaterに変更
            case PartsChara.Fire:
                playerStatus.CanChargeWater = statusData.FireCanChargeWater;
                break;
            
            case PartsChara.Assassin:
                playerStatus.CanThrowKnife = statusData.AssassinCanThrowKnife;
                runTimeStatus.CanThrowKnife = true;
                break;

            default:
                Debug.LogError("不明な装備");
                break;
        }

        // 右腕の計算
        switch(playerParts.RightArm) {
            case PartsChara.Normal:
                break;

            case PartsChara.Thief:
                playerStatus.CanUnlock = statusData.ThiefCanUnlock;
                break;

            case PartsChara.Muscle:
                playerStatus.CanPushHeavyObject = statusData.MuscleCanPushHeavyObject;
                break;
            
            //ShotWater->ChargeWaterに変更
            case PartsChara.Fire:
                playerStatus.CanChargeWater = statusData.FireCanChargeWater;
                break;
            
            case PartsChara.Assassin:
                playerStatus.CanThrowKnife = statusData.AssassinCanThrowKnife;
                runTimeStatus.CanThrowKnife = true;
                break;

            default:
                Debug.LogError("不明な装備");
                break;
        }

        // 左脚の計算
        switch(playerParts.LeftLeg) {
            case PartsChara.Normal:
                break;

            case PartsChara.Thief:
                playerStatus.Friction = statusData.TheifFriction;
                playerStatus.AirResistance = statusData.TheifAirResistance;
                playerStatus.MoveSpeed = statusData.ThiefMoveSpeed;
                break;
            
            case PartsChara.Muscle:
                playerStatus.JumpForce = statusData.MuscleJumpForce;
                break;
            
            case PartsChara.Fire:
                playerStatus.CanWalkOnFire = statusData.FireCanWalkOnFire;
                break;
            
            case PartsChara.Assassin:
                playerStatus.CanDoubleJump = statusData.AssassinCanDoubleJump;
                runTimeStatus.CanDoubleJump = true;
                break;
            
            default:
                Debug.LogError("不明な装備");
                break;
        }

        // 右脚の計算
        switch(playerParts.RightLeg) {
            case PartsChara.Normal:
                break;

            case PartsChara.Thief:
                playerStatus.Friction = statusData.TheifFriction;
                playerStatus.AirResistance = statusData.TheifAirResistance;
                playerStatus.MoveSpeed = statusData.ThiefMoveSpeed;
                break;
            
            case PartsChara.Muscle:
                playerStatus.JumpForce = statusData.MuscleJumpForce;
                break;
            
            case PartsChara.Fire:
                playerStatus.CanWalkOnFire = statusData.FireCanWalkOnFire;
                break;
            
            case PartsChara.Assassin:
                playerStatus.CanDoubleJump = statusData.AssassinCanDoubleJump;
                runTimeStatus.CanDoubleJump = true;
                break;
            
            default:
                Debug.LogError("不明な装備");
                break;
        }
    }

    // 取得したパーツの装備箇所と種類を引数にし、
    // 元々装備してたパーツの種類を返すメソッド
    public PartsChara ChangePlayerParts(PartsSlot slot, PartsChara chara) {
        PartsChara befChara = PartsChara.None;
        
        Debug.Log("交換前のプレイヤーパーツの種類: LeftArm -> " + playerParts.LeftArm
         + ", RightArm -> " + playerParts.RightArm + ", LeftLeg -> "
          + playerParts.LeftLeg
           + ", RightLeg -> " + playerParts.RightLeg);
        

        // すでに装備しているパーツを取り出し、新しいものと交換する
        switch (slot)
        {
            case PartsSlot.LeftArm:
                befChara = playerParts.LeftArm;
                playerParts.LeftArm = chara;
                break;

            case PartsSlot.RightArm:
                befChara = playerParts.RightArm;
                playerParts.RightArm = chara;
                break;

            case PartsSlot.LeftLeg:
                befChara = playerParts.LeftLeg;
                playerParts.LeftLeg = chara;
                break;

            case PartsSlot.RightLeg:
                befChara = playerParts.RightLeg;
                playerParts.RightLeg = chara;
                break;

            default:
                Debug.LogError("不明な装備スロット");
                break;
        }

        // 能力を変更する
        ChangePlayerStatus();

        // 変更した能力をコントローラーに反映させる
        controller.SetStatus();

        // 見た目を変更する
        visualCustomizer.ChangeVisual(slot, befChara, chara);

        Debug.Log("交換後のプレイヤーパーツの種類: LeftArm -> " + playerParts.LeftArm
         + ", RightArm -> " + playerParts.RightArm + ", LeftLeg -> "
          + playerParts.LeftLeg
           + ", RightLeg -> " + playerParts.RightLeg);

        return befChara;
    }
}
