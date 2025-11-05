using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
/// <summary>
/// タイトル画面のライフタイムスコープを設定するクラス
/// </summary>

public class TitleLifeTiimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            builder.RegisterInstance(player).WithParameter("Player");
        }
        else
        {
            Debug.LogError("Playerタグの付いたGameObjectが見つかりません");
        }

        // Singletons/ItemDataから取得するオブジェクト
        var itemDataObject = GameObject.Find("Singletons/ItemData");
        if (itemDataObject != null)
        {
            // InventoryDataの登録
            var inventoryData = itemDataObject.GetComponentInChildren<InventoryData>();
            if (inventoryData != null)
            {
                Debug.Log("InventoryDataが見つかりました。");
                builder.RegisterInstance(inventoryData);
            }
            else
            {
                Debug.LogError("Singletons/ItemDataの子オブジェクトにInventoryDataコンポーネントが見つかりません");
            }
        }
        else
        {
            Debug.LogError("シーン内に'Singletons/ItemData'オブジェクトが見つかりません");
        }

        // PlayerPartsを登録
        var playerParts = Object.FindAnyObjectByType<PlayerParts>();
        if (playerParts != null)
        {
            builder.RegisterInstance(playerParts);

            // PartsManagerを登録
            var partsManager = Object.FindAnyObjectByType<PartsManager>();
            if (partsManager != null)
            {
                builder.RegisterInstance(partsManager);
                builder.RegisterBuildCallback(resolver =>
                {
                    resolver.Inject(partsManager);
                });
            }
            else
            {
                Debug.LogError("PartsManagerが見つかりません。シーンに追加してください。");
            }

            // PlayerPartsRatioを直後に登録
            var partsRatio = Object.FindAnyObjectByType<PlayerPartsRatio>();
            if (partsRatio != null)
            {
                builder.RegisterInstance(partsRatio);
                builder.RegisterBuildCallback(resolver => 
                {
                    resolver.Inject(partsRatio);
                    partsRatio.Initialize(); // 初期化を明示的に呼び出す
                });
            }
            else
            {
                Debug.LogError("PlayerPartsRatioが見つかりません");
            }
        }
        else
        {
            Debug.LogError("PlayerPartsコンポーネントが見つかりません");
        }

        // StageNumberを登録
        var stageNumber = Object.FindAnyObjectByType<StageNumber>();
        if (stageNumber != null)
        {
            builder.RegisterInstance(stageNumber);
        }
        else
        {
            Debug.LogError("StageNumberオブジェクトが見つかりません");
        }

        // ItemDataを自動検索（Addressableから)
        var ItemDataHandle = Addressables.LoadAssetAsync<ItemData>("ItemData");
        var ItemData = ItemDataHandle.WaitForCompletion();
        if (ItemData != null)
        {
            // ItemDataを登録
            builder.RegisterInstance(ItemData);
        }
        else
        {
            Debug.LogError("ItemDataアセットが見つかりません。");
        }

        // ItemSlotへの依存注入設定
        var itemSlots = Object.FindObjectsByType<ItemSlot>(FindObjectsSortMode.None);
        if (itemSlots.Length > 0)
        {
            builder.RegisterInstance<IReadOnlyList<ItemSlot>>(itemSlots);
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var slot in itemSlots)
                {
                    resolver.Inject(slot);
                    slot.Initialize(); // 依存注入後に初期化
                }
            });
        }

        // GameDataManagerを自動検索
        var gameDataManager = Object.FindAnyObjectByType<GameDataManager>();
        if (gameDataManager != null)
        {
            builder.RegisterInstance(gameDataManager);
            builder.RegisterBuildCallback(resolver => resolver.Inject(gameDataManager));

        }
        else
        {
            Debug.LogError("GameDataManagerが見つかりません");
        }
        
        // SceneManagerを自動検索
        var sceneManager = Object.FindAnyObjectByType<GameSceneManager>();
        if (sceneManager != null)
        {
            builder.RegisterInstance(sceneManager);
            builder.RegisterBuildCallback(resolver => resolver.Inject(sceneManager));
        }
        else
        {
            Debug.LogError("GameSceneManagerが見つかりません");
        }
        
        // PlayerCustomizerを登録
        var playerCustomizer = Object.FindAnyObjectByType<PlayerCustomizer>();
        if (playerCustomizer != null)
        {
            builder.RegisterInstance(playerCustomizer);
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(playerCustomizer);
            });
            Debug.Log("PlayerCustomizerを登録しました");
        }
        else
        {
            Debug.LogError("PlayerCustomizerコンポーネントが見つかりません");
        }

        // Controller
        var controller = player.GetComponent<Controller>();
        if (controller != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(controller);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(controller);
            });
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにControllerコンポーネントが見つかりません");
        }

        // PlayerStatusDataを自動検索（Addressableから）
        Debug.Log("PlayerStatusData読み込み開始...");
        var statusDataHandle = Addressables.LoadAssetAsync<PlayerStatusData>("PlayerStatus");
        var statusData = statusDataHandle.WaitForCompletion();

        if (statusData != null)
        {
            Debug.Log($"読み込んだPlayerStatusData名: {statusData.name}");

            // PlayerStatusDataを登録
            builder.RegisterInstance(statusData);
            Debug.Log($"PlayerStatusData の登録に成功しました: {statusData.name}");
        }
        else
        {
            Debug.LogError("アドレス 'PlayerStatus' のPlayerStatusDataアセットが見つかりません。");
        }

        // Singletons/PlayerDataから取得するオブジェクト
        var playerDataObject = GameObject.Find("Singletons/PlayerData");
        if (playerDataObject != null)
        {
            //PlayerStatusの登録
            var playerStatus = playerDataObject.GetComponentInChildren<PlayerStatus>();
            if (playerStatus != null)
            {
                builder.RegisterInstance(playerStatus);
            }
            else
            {
                Debug.LogError("Singletons/PlayerDataの子オブジェクトにPlayerStatusコンポーネントが見つかりません");
            }
            // PlayerRunTimeStatusの登録
            var playerRunTimeStatus = playerDataObject.GetComponentInChildren<PlayerRunTimeStatus>();
            if (playerRunTimeStatus != null)
            {
                builder.RegisterInstance(playerRunTimeStatus);
            }
            else
            {
                Debug.LogError("Singletons/PlayerDataの子オブジェクトにPlayerRunTimeStatusコンポーネントが見つかりません");
            }
        }
        else
        {
            Debug.LogError("Singletons/PlayerDataオブジェクトが見つかりません");
        }
        // Playerにattachされているコンポーネントの登録
        if (player != null)
        {
            // PlayerFallAction
            var playerFallAction = player.GetComponent<PlayerFallAction>();
            if (playerFallAction != null)
            {
                // 1. インスタンスを登録
                builder.RegisterInstance(playerFallAction);
                // 2. 構築後にDIを実行
                builder.RegisterBuildCallback(resolver =>
                {
                    resolver.Inject(playerFallAction);
                });
            }
            else
            {
                Debug.LogError("PlayerオブジェクトにPlayerFallActionコンポーネントが見つかりません");
            }

            // PlayerAirCheckerを登録（Playerオブジェクトから取得）
            var playerAirChecker = player.GetComponent<PlayerAirChecker>();
            if (playerAirChecker != null)
            {
                // 1. インスタンスを登録
                builder.RegisterInstance(playerAirChecker);
                // 2. 構築後にDIを実行
                builder.RegisterBuildCallback(resolver =>
                {
                    resolver.Inject(playerAirChecker);
                });
            }
            else
            {
                Debug.LogError("PlayerオブジェクトにPlayerAirCheckerコンポーネントが見つかりません");
            }

            // PlayerAnimatorを登録（Playerオブジェクトから取得）
            var playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                // 1. インスタンスを登録
                builder.RegisterInstance(playerAnimator);
                // 2. 構築後にDIを実行
                builder.RegisterBuildCallback(resolver =>
                {
                    resolver.Inject(playerAnimator);
                });
            }
            else
            {
                Debug.LogError("PlayerオブジェクトにAnimatorコンポーネントが見つかりません");
            }
        }
        // PlayerAnimationManagerを登録
        var playerAnimationManager = Object.FindAnyObjectByType<PlayerAnimationManager>();
        if (playerAnimationManager != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(playerAnimationManager);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(playerAnimationManager);
            });
        }
        else
        {
            Debug.LogError("PlayerAnimationManagerコンポーネントが見つかりません");
        }

        // GameOverManagerを登録
        var gameOverManager = Object.FindAnyObjectByType<GameOverManager>();
        if (gameOverManager != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(gameOverManager);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(gameOverManager);
            });
        }
        else
        {
            Debug.LogError("GameOverManagerコンポーネントが見つかりません");
        }

        // EnterKeyActionTrigger
        var enterKeyActionTrigger = player.GetComponent<EnterKeyActionTrigger>();
        if (enterKeyActionTrigger != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(enterKeyActionTrigger);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(enterKeyActionTrigger);
            });
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにEnterKeyActionTriggerコンポーネントが見つかりません");
        }

        // GameTextDisplayを登録
        var gameTextDisplay = Object.FindAnyObjectByType<GameTextDisplay>();
        if (gameTextDisplay != null)
        {
            builder.RegisterInstance(gameTextDisplay);
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(gameTextDisplay);
            });
            Debug.Log("GameTextDisplayを登録しました");
        }
        else
        {
            Debug.LogError("GameTextDisplayコンポーネントが見つかりません");
            // オプション: 自動生成
            var textDisplayObject = new GameObject("GameTextDisplay");
            gameTextDisplay = textDisplayObject.AddComponent<GameTextDisplay>();
            builder.RegisterInstance(gameTextDisplay);
            Debug.Log("GameTextDisplayを自動生成しました");
        }

        // ObjectTextDataを自動検索（Addressableから)
        Debug.Log("ObjectTextData読み込み開始...");
        var objectTextDataHandle = Addressables.LoadAssetAsync<ObjectTextData>("ObjectTextData");
        var objectTextData = objectTextDataHandle.WaitForCompletion();
        if (objectTextData != null)
        {
            Debug.Log($"読み込んだObjectTextData名: {objectTextData.name}");

            // ObjectTextDataを登録
            builder.RegisterInstance(objectTextData);
            Debug.Log($"ObjectTextData の登録に成功しました: {objectTextData.name}");
        }
        else
        {
            Debug.LogError("アドレス 'ObjectTextData' のObjectTextDataアセットが見つかりません。");
        }

        // EndingShowDisplayを登録
        var endingShowDisplay = Object.FindAnyObjectByType<EndingShowDisplay>();
        if (endingShowDisplay != null)
        {
            builder.RegisterInstance(endingShowDisplay);
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(endingShowDisplay);
            });
            Debug.Log("EndingShowDisplayを登録しました");
        }
        else
        {
            Debug.LogError("EndingShowDisplayコンポーネントが見つかりません");
        }
    }
}
