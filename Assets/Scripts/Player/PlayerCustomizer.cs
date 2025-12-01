using UnityEngine;
using Parts.Types;
using VContainer;

/// <summary>
/// プレイヤーが装備しているパーツと能力を設定するスクリプト
/// </summary>

public class PlayerCustomizer : MonoBehaviour
{
    // シングルトンの取得
    private PlayerParts playerParts;
    [Inject] private PlayerStatus playerStatus;
    [SerializeField] private MenuStatusDisplay statusDisplay;

    // Controllerの取得
    [Inject] private Controller controller;

    // データファイルの取得
    [Inject] private PlayerStatusData statusData;

    // RunTimeStatusの取得
    [Inject] private PlayerRunTimeStatus runTimeStatus;

    // PlayerVisualCustomizerの取得
    [SerializeField] private PlayerVisualCustomizer playerVisualCustomizer;

    private void Awake()
    {
        playerParts = GameObject.Find ("PlayerParts").GetComponent<PlayerParts>();
        ChangePlayerStatus();
        controller.SetStatus();
    }

    // プレイヤーの能力値を初期状態にリセットする関数
    private void resetStatus()
    {
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
    private void ChangePlayerStatus()
    {

        if (playerParts == null)
        {
            Debug.LogError("PlayerCustomizer: PlayerPartsがnullです");
            return;
        }
        // 能力をリセットする
        resetStatus();

        // 能力を再計算する
        // 左腕の計算
        switch (playerParts.LeftArm)
        {
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
        switch (playerParts.RightArm)
        {
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
        switch (playerParts.LeftLeg)
        {
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
        switch (playerParts.RightLeg)
        {
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
    public PartsChara ChangePlayerParts(PartsSlot slot, PartsChara chara, MapPartsVisualCustomizer mapPartsVisualCustomizer)
    {
        if (playerParts == null)
        {
            Debug.LogError($"PlayerCustomizer: playerPartsがnullです");
            return PartsChara.None;
        }

        Debug.Log($"ChangePlayerParts - Slot: {slot}, Chara: {chara}");

        PartsChara befChara = PartsChara.None;

        Debug.Log("交換前のプレイヤーパーツの種類: LeftArm -> " + playerParts.LeftArm
         + ", RightArm -> " + playerParts.RightArm + ", LeftLeg -> "
          + playerParts.LeftLeg
           + ", RightLeg -> " + playerParts.RightLeg);

        try
        {
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
            playerVisualCustomizer.ChangeVisual(slot, befChara, chara);

            // マップ上のパーツの見た目も変更する
            mapPartsVisualCustomizer.ChangeVisual(slot, chara, befChara);

            // ステータス画面のテキストを変える
            statusDisplay.DisplayStatus();

            Debug.Log("交換後のプレイヤーパーツの種類: LeftArm -> " + playerParts.LeftArm
             + ", RightArm -> " + playerParts.RightArm + ", LeftLeg -> "
              + playerParts.LeftLeg
               + ", RightLeg -> " + playerParts.RightLeg);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"パーツ変更中にエラーが発生: {e.Message}\n{e.StackTrace}");
        }

        return befChara;
    }

    // セーブデータからのロード専用パーツ変更メソッド
    public void LoadPlayerParts(PartsSlot slot, PartsChara chara)
    {
        if (playerParts == null)
        {
            Debug.LogError($"PlayerCustomizer: playerPartsがnullです");
            return;
        }

        Debug.Log($"LoadPlayerParts - Slot: {slot}, Chara: {chara}");

        try
        {
            // パーツを変更する
            switch (slot)
            {
                case PartsSlot.LeftArm:
                    playerParts.LeftArm = chara;
                    break;

                case PartsSlot.RightArm:
                    playerParts.RightArm = chara;
                    break;

                case PartsSlot.LeftLeg:
                    playerParts.LeftLeg = chara;
                    break;

                case PartsSlot.RightLeg:
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
            playerVisualCustomizer.ChangeVisual(slot, PartsChara.Normal, chara);

            Debug.Log($"パーツロード完了 - {slot}: {chara}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"パーツロード中にエラーが発生: {e.Message}\n{e.StackTrace}");
            throw;
        }
    }

    // キャラクターの全能力と見た目をリセットする
    public void resetCharacter()
    {
        playerParts.LeftArm = PartsChara.Normal;
        playerParts.RightArm = PartsChara.Normal;
        playerParts.LeftLeg = PartsChara.Normal;
        playerParts.RightLeg = PartsChara.Normal;
        resetStatus();
    }
}