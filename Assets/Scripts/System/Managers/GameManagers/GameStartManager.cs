using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///  ステージスタート時に呼び出すスクリプト
/// </summary>
public class GameStartManager : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Controller controller;
    [SerializeField] private PlayerAnimationManager playerAnimationManager;
    [SerializeField] private GameObject startObg;

    [SerializeField] private ItemManager itemManager;
    [SerializeField] private MenuStatusDisplay menuStatus;
    [SerializeField] private GameDataManager gameDataManager;
    [SerializeField] private PlayerInput playerInput;

    // プレイヤーの速度(単位はs)
    private float dashTime = 2.0f;
    private float stopTime = 1.0f;

    // アニメーション時間
    private float animationTime;

    private CancellationToken dct; // DestroyCancellationToken

    async UniTaskVoid Start()
    {
        // 初期化
        playerInput.SwitchCurrentActionMap("UI");
        KeyManager.ResetKey();
        gameDataManager.SaveGame(1);
        gameDataManager.LoadItemData();
        itemManager.SyncInventoryToStage();
        menuStatus.DisplayStatus();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(6, true);
        }


        startObg.SetActive(false);

        animationTime = dashTime + stopTime;

        // 黒画像をフェードインさせる
        fadeController.FadeIn(animationTime).Forget();

        // 入力を止める
        controller.isStartGoal = true;

        // プレイヤーを動かす
        playerAnimationManager.AniWalkTrue();

        controller.StartAndGoalSetFrictionZero();

        controller.StartAndGoalVelocity();

        dct = this.GetCancellationTokenOnDestroy();

        try{
            await UniTask.Delay((int)(dashTime * 1000), cancellationToken: dct);

            controller.StartAndGoalSetFrictionAdd();
            playerAnimationManager.AniWalkFalse();

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopSE();
            }


            await UniTask.Delay((int)(stopTime * 1000), cancellationToken: dct);

            // プレイヤーを止める

            // スタートに障害物を置く
            startObg.SetActive(true);

            // 入力を再開する
            playerInput.SwitchCurrentActionMap("Player");
            controller.isStartGoal = false;
        } finally
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopSE();
            }
        }
    }
}
