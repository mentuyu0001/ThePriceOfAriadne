using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Controller controller;
    [SerializeField] private PlayerAnimationManager playerAnimationManager;
    [SerializeField] private GameObject GoalObg;
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private ItemManager itemManager;
    [SerializeField] private GameDataManager gameDataManager;

    [SerializeField] private SoundManager soundManager;

    private StageNumber stageNumber;

    // プレイヤーの速度(単位はs)
    private float dashTime = 2.0f;
    private float stopTime = 1.0f;

    // アニメーション時間
    private float animationTime;

    // 一度だけ実行するためのフラグ（目印）
    private bool hasTriggered = false;

    void Start()
    {
        stageNumber = GameObject.Find("StageNumber").GetComponent<StageNumber>();
    }
    private async void OnTriggerEnter2D(Collider2D other)
    {
        if (stageNumber.GetCurrentStage() == 5)
        {
            // 初期化
            itemManager.SyncStageToInventory();
            gameDataManager.SaveItemData();

            if (other.gameObject.tag == "Player" && !hasTriggered)
            {
                hasTriggered = true;

                GoalObg.SetActive(false);

                animationTime = dashTime + stopTime;

                // 入力を止める
                controller.isStartGoal = true;

                // プレイヤーを動かす
                playerAnimationManager.AniWalkTrue();
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlaySE(6, true);
                }

                controller.StartAndGoalSetFrictionZero();

                controller.StartAndGoalVelocity();

                // エンディングシーンへ移動
                gameSceneManager.LoadEpilogue();

                await UniTask.Delay((int)(dashTime * 1000));

                controller.StartAndGoalSetFrictionAdd();
                playerAnimationManager.AniWalkFalse();


                await UniTask.Delay((int)(stopTime * 1000));

                // プレイヤーを止める
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.StopSE();
                }
            }
            return;
        }
        // 初期化
        stageNumber.SetCurrentStage(stageNumber.GetCurrentStage() + 1);
        itemManager.SyncStageToInventory();
        gameDataManager.SaveItemData();

        // オートセーブ
        gameDataManager.SaveGame(1);

        if (other.gameObject.tag == "Player" && !hasTriggered)
        {
            hasTriggered = true;

            GoalObg.SetActive(false);

            animationTime = dashTime + stopTime;

            // 入力を止める
            controller.isStartGoal = true;

            // プレイヤーを動かす
            playerAnimationManager.AniWalkTrue();
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySE(6, true);
            }

            controller.StartAndGoalSetFrictionZero();

            controller.StartAndGoalVelocity();

            // セーブシーンへ移動
            gameSceneManager.LoadSaveScene(stageNumber.GetCurrentStage()-1);

            await UniTask.Delay((int)(dashTime * 1000));

            controller.StartAndGoalSetFrictionAdd();
            playerAnimationManager.AniWalkFalse();


            await UniTask.Delay((int)(stopTime * 1000));

            // プレイヤーを止める
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.StopSE();
            }
        }
    }
}
