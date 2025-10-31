using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
/// <summary>
/// ゲーム全体のライフタイムスコープを設定するクラス
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private bool enableDebugLog = false; // デバッグログの表示制御

    protected override void Configure(IContainerBuilder builder)
    {
        if (enableDebugLog) Debug.Log("GameLifetimeScope 登録開始");


        #region Singleton_Objects
        // プレイヤーを自動検索（名前付きで登録）
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            builder.RegisterInstance(player).WithParameter("Player");
            if (enableDebugLog) Debug.Log($"Playerが正常に登録されました: {player.name}");
        }
        else
        {
            Debug.LogError("Playerタグの付いたGameObjectが見つかりません");
        }

        // Groundを自動検索
        var ground = Object.FindAnyObjectByType<Ground>();
        if (ground != null)
        {
            builder.RegisterInstance(ground);
            if (enableDebugLog) Debug.Log($"Groundコンポーネントが正常に登録されました: {ground.name}");
        }
        else
        {
            Debug.LogError("Groundコンポーネントが見つかりません");
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

        // ItemDataを自動検索（Addressableから)
        Debug.Log("ItemData読み込み開始...");
        var ItemDataHandle = Addressables.LoadAssetAsync<ItemData>("ItemData");
        var ItemData = ItemDataHandle.WaitForCompletion();
        if (ItemData != null)
        {
            Debug.Log($"読み込んだItemData名: {ItemData.name}");

            // ItemDataを登録
            builder.RegisterInstance(ItemData);
            Debug.Log($"ItemData の登録に成功しました: {ItemData.name}");
        }
        else
        {
            Debug.LogError("アドレス 'PlayerParts' のItemDataアセットが見つかりません。");
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

        // KnifePrefabを自動検索（ファクトリーとして登録）
        Debug.Log("KnifePrefab読み込み開始...");
        var handle = Addressables.LoadAssetAsync<GameObject>("KnifePrefab");
        var knifePrefabGo = handle.WaitForCompletion();

        if (knifePrefabGo != null)
        {
            Debug.Log($"読み込んだオブジェクト名: {knifePrefabGo.name}");

            var knifeComponent = knifePrefabGo.GetComponent<Knife>();
            if (knifeComponent != null)
            {
                // KnifeFactoryとして登録
                builder.Register<IKnifeFactory>(container => new KnifeFactory(knifePrefabGo), Lifetime.Singleton);
                Debug.Log($"Knife Factory の登録に成功しました: {knifePrefabGo.name}");
            }
            else
            {
                Debug.LogError($"読み込んだオブジェクト '{knifePrefabGo.name}' にKnifeコンポーネントが見つかりません");
            }
        }
        else
        {
            Debug.LogError("アドレス 'KnifePrefab' のアセットが見つかりません。");
        }

        // WaterPrefabを自動検索（ファクトリーとして登録）
        Debug.Log("WaterPrefab読み込み開始...");
        var waterHandle = Addressables.LoadAssetAsync<GameObject>("WaterPrefab");
        var waterPrefabGo = waterHandle.WaitForCompletion();

        if (waterPrefabGo != null)
        {
            Debug.Log($"読み込んだWaterオブジェクト名: {waterPrefabGo.name}");

            // WaterFactoryとして登録（GameObjectとしては登録しない）
            builder.Register<IWaterFactory>(container => new WaterFactory(waterPrefabGo), Lifetime.Singleton);
            Debug.Log($"Water Factory の登録に成功しました: {waterPrefabGo.name}");
        }
        else
        {
            Debug.LogError("アドレス 'WaterPrefab' のアセットが見つかりません。");
        }

        // GameOverManagerを自動検索
        var gameOverManager = Object.FindAnyObjectByType<GameOverManager>();
        if (gameOverManager != null)
        {
            builder.RegisterInstance(gameOverManager);
            if (enableDebugLog) Debug.Log($"GameOverManagerが正常に登録されました: {gameOverManager.name}");
        }
        else
        {
            Debug.LogError("GameOverManagerが見つかりません");
        }
        // GameDataManagerを自動検索
        var gameDataManager = Object.FindAnyObjectByType<GameDataManager>();
        if (gameDataManager != null)
        {
            builder.RegisterInstance(gameDataManager);
            builder.RegisterBuildCallback(resolver => resolver.Inject(gameDataManager)); // ← 依存注入を追加
            if (enableDebugLog) Debug.Log($"GameDataManagerが正常に登録されました: {gameDataManager.name}");
        }
        else
        {
            Debug.LogError("GameDataManagerが見つかりません");
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
                if (enableDebugLog) Debug.Log($"PlayerStatusが正常に登録されました: {playerStatus.name}");
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
                if (enableDebugLog) Debug.Log($"PlayerRunTimeStatusが正常に登録されました: {playerRunTimeStatus.name}");
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

        // Singletons/ItemDataから取得するオブジェクト
        var itemDataObject = GameObject.Find("Singletons/ItemData");
        if (itemDataObject != null)
        {
            // InventoryDataの登録
            var inventoryData = itemDataObject.GetComponentInChildren<InventoryData>();
            if (inventoryData != null)
            {
                builder.RegisterInstance(inventoryData);
                if (enableDebugLog) Debug.Log($"InventoryDataが正常に登録されました: {inventoryData.name}");
            }
            else
            {
                Debug.LogError("Singletons/ItemDataの子オブジェクトにInventoryDataコンポーネントが見つかりません");
            }

            // StageInventoryDataの登録
            var stageInventoryData = itemDataObject.GetComponentInChildren<StageInventoryData>();
            if (stageInventoryData != null)
            {
                builder.RegisterInstance(stageInventoryData);
                if (enableDebugLog) Debug.Log($"StageInventoryDataが正常に登録されました: {stageInventoryData.name}");
            }
            else
            {
                Debug.LogError("Singletons/ItemDataの子オブジェクトにStageInventoryDataコンポーネントが見つかりません");
            }
        }
        else
        {
            Debug.LogError("Singletons/ItemDataオブジェクトが見つかりません");
        }

        // PlayerPartsを登録
        var playerParts = Object.FindAnyObjectByType<PlayerParts>();
        if (playerParts != null)
        {
            builder.RegisterInstance(playerParts);
            if (enableDebugLog) Debug.Log($"PlayerPartsが正常に登録されました: {playerParts.name}");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにPlayerPartsコンポーネントが見つかりません");
        }

        // PartsManagerを登録
        var partsManager = Object.FindAnyObjectByType<PartsManager>();
        if (partsManager != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(partsManager);

            // 2. 構築後にDIを実行（これが不足していました）
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(partsManager);
            });

            if (enableDebugLog) Debug.Log($"PartsManagerが正常に登録されました: {partsManager.name}");
        }
        else
        {
            Debug.LogError("PartsManagerコンポーネントが見つかりません");
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
            if (enableDebugLog) Debug.Log($"PlayerAnimationManagerが正常に登録されました: {playerAnimationManager.name}");
        }
        else
        {
            Debug.LogError("PlayerAnimationManagerコンポーネントが見つかりません");
        }

        // PlayerCustomizerを登録
        var playerCustomizer = Object.FindAnyObjectByType<PlayerCustomizer>();
        if (playerCustomizer != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(playerCustomizer);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(playerCustomizer);
            });
            if (enableDebugLog) Debug.Log("PlayerCustomizerに注入予約しました");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにPlayerCustomizerコンポーネントが見つかりません");
        }

        // ItemManagerを登録
        var itemManager = Object.FindAnyObjectByType<ItemManager>();
        if (itemManager != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(itemManager);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(itemManager);
            });
            if (enableDebugLog) Debug.Log("ItemManagerに注入予約しました");
        }
        else
        {
            Debug.LogError("ItemManagerコンポーネントが見つかりません");
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
                if (enableDebugLog) Debug.Log("PlayerFallActionに注入予約しました");
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
                if (enableDebugLog) Debug.Log($"PlayerAirCheckerが正常に登録されました: {playerAirChecker.name}");
                // 2. 構築後にDIを実行
                builder.RegisterBuildCallback(resolver =>
                {
                    resolver.Inject(playerAirChecker);
                });
                if (enableDebugLog) Debug.Log("PlayerFallActionに注入予約しました");
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
                if (enableDebugLog) Debug.Log($"PlayerAnimatorが正常に登録されました: {playerAnimator.name}");
                // 2. 構築後にDIを実行
                builder.RegisterBuildCallback(resolver =>
                {
                    resolver.Inject(playerAnimator);
                });
                if (enableDebugLog) Debug.Log("PlayerAnimatorに注入予約しました");
            }
            else
            {
                Debug.LogError("PlayerオブジェクトにAnimatorコンポーネントが見つかりません");
            }

        }
        // GameTextDisplayの登録
        var textDisplay = Object.FindAnyObjectByType<GameTextDisplay>();
        if (textDisplay != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(textDisplay);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(textDisplay);
            });
            if (enableDebugLog) Debug.Log("GameTextDisplayに注入予約しました");
        }
        else
        {
            Debug.LogError("GameTextDisplayコンポーネントが見つかりません");
        }
        // MenuStatusDisplayの登録
        var menuStatusDisplay = Object.FindAnyObjectByType<MenuStatusDisplay>();
        if (menuStatusDisplay != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(menuStatusDisplay);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(menuStatusDisplay);
            });
            if (enableDebugLog) Debug.Log("MenuStatusDisplayに注入予約しました");
        }
        else
        {
            Debug.LogError("MenuStatusDisplayコンポーネントが見つかりません");
        }


        // PlayerPartsRatioの登録
        var partsRatio = Object.FindAnyObjectByType<PlayerPartsRatio>();
        if (partsRatio != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(partsRatio);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(partsRatio);
            });
            if (enableDebugLog) Debug.Log("PlayerPartsRatioに注入予約しました");
        }
        else
        {
            Debug.LogError("PlayerPartsRatioコンポーネントが見つかりません");
        }

        #endregion

        // HeavyObject
        var heavyObjects = Object.FindObjectsByType<HeavyObject>(FindObjectsSortMode.None);
        if (heavyObjects.Length > 0)
        {
            // 1. 他のクラスが注入できるよう、リストだけを登録する
            builder.RegisterInstance<IReadOnlyList<HeavyObject>>(heavyObjects);

            // 2. コンテナ構築後に、各インスタンスへDIを実行するよう予約する
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var heavyObject in heavyObjects)
                {
                    // resolver.Inject(instance)で、既存インスタンスにDIを実行
                    resolver.Inject(heavyObject);
                }
            });
            if (enableDebugLog) Debug.Log($"{heavyObjects.Length}個のHeavyObjectを登録 & 注入予約");
        }

        // DoorKey
        var doorKeys = Object.FindObjectsByType<DoorKey>(FindObjectsSortMode.None);
        if (doorKeys.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<DoorKey>>(doorKeys);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var doorKey in doorKeys)
                {
                    resolver.Inject(doorKey);
                }
            });
            if (enableDebugLog) Debug.Log($"{doorKeys.Length}個のDoorKeyを登録 & 注入予約");
        }

        // LockedDoor
        var lockedDoors = Object.FindObjectsByType<LockedDoor>(FindObjectsSortMode.None);
        if (lockedDoors.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<LockedDoor>>(lockedDoors);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var lockedDoor in lockedDoors)
                {
                    resolver.Inject(lockedDoor);
                }
            });
            if (enableDebugLog) Debug.Log($"{lockedDoors.Length}個のLockedDoorを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("LockedDoorコンポーネントが見つかりません");
        }

        // RustyLever
        var rustyLevers = Object.FindObjectsByType<RustyLever>(FindObjectsSortMode.None);
        if (rustyLevers.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<RustyLever>>(rustyLevers);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var rustyLever in rustyLevers)
                {
                    resolver.Inject(rustyLever);
                }
            });
            if (enableDebugLog) Debug.Log($"{rustyLevers.Length}個のRustyLeverを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("RustyLeverコンポーネントが見つかりません");
        }

        // FallingCeilingScript
        var fallingCeilingScripts = Object.FindObjectsByType<FallingCeilingScript>(FindObjectsSortMode.None);
        if (fallingCeilingScripts.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<FallingCeilingScript>>(fallingCeilingScripts);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var fallingCeiling in fallingCeilingScripts)
                {
                    resolver.Inject(fallingCeiling);
                }
            });
            if (enableDebugLog) Debug.Log($"{fallingCeilingScripts.Length}個のFallingCeilingScriptを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("FallingCeilingScriptコンポーネントが見つかりません");
        }
        // PressMachoinePlate
        var pressMachinePlates = Object.FindObjectsByType<PressMachinePlate>(FindObjectsSortMode.None);
        if (pressMachinePlates.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<PressMachinePlate>>(pressMachinePlates);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var pressMachinePlate in pressMachinePlates)
                {
                    resolver.Inject(pressMachinePlate);
                }
            });
            if (enableDebugLog) Debug.Log($"{pressMachinePlates.Length}個のPressMachinePlateを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("PressMachinePlateコンポーネントが見つかりません");
        }
        // PressMachineBase
        var pressMachineBases = Object.FindObjectsByType<PressMachineBase>(FindObjectsSortMode.None);
        if (pressMachineBases.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<PressMachineBase>>(pressMachineBases);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var pressMachineBase in pressMachineBases)
                {
                    resolver.Inject(pressMachineBase);
                }
            });
            if (enableDebugLog) Debug.Log($"{pressMachineBases.Length}個のPressMachine  Baseを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("PressMachineBaseコンポーネントが見つかりません");
        }

        // Laser
        var lasers = Object.FindObjectsByType<Laser>(FindObjectsSortMode.None);
        if (lasers.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<Laser>>(lasers);

            // 2. LaserFactoryを登録
            builder.Register<ILaserFactory>(container =>
                new LaserFactory(lasers), Lifetime.Singleton);

            // 3. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var laser in lasers)
                {
                    resolver.Inject(laser);
                }
            });
            if (enableDebugLog) Debug.Log($"{lasers.Length}個のLaserを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("Laserコンポーネントが見つかりません");
        }

        // LaserTarget
        var laserTargets = Object.FindObjectsByType<LaserTarget>(FindObjectsSortMode.None);
        if (laserTargets.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<LaserTarget>>(laserTargets);

            // 2. LaserTargetファクトリーを登録
            builder.Register<ILaserTargetFactory>(container =>
                new LaserTargetFactory(laserTargets), Lifetime.Singleton);

            // 3. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var laserTarget in laserTargets)
                {
                    resolver.Inject(laserTarget);
                }
            });
            if (enableDebugLog) Debug.Log($"{laserTargets.Length}個のLaserTargetを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("LaserTargetコンポーネントが見つかりません");
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
            if (enableDebugLog) Debug.Log("Controllerに注入予約しました");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにControllerコンポーネントが見つかりません");
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
            if (enableDebugLog) Debug.Log("EnterKeyActionTriggerに注入予約しました");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにEnterKeyActionTriggerコンポーネントが見つかりません");
        }

        // ThrowKnifeController
        var throwKnifeController = player.GetComponent<ThrowKnifeController>();
        if (throwKnifeController != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(throwKnifeController);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(throwKnifeController);
            });
            if (enableDebugLog) Debug.Log("ThrowKnifeControllerに注入予約しました");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにThrowKnifeControllerコンポーネントが見つかりません");
        }

        if (enableDebugLog) Debug.Log("GameLifetimeScope 登録完了");

        // ShootWaterController
        var shootWaterController = player.GetComponent<ShootWaterController>();
        if (shootWaterController != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(shootWaterController);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(shootWaterController);
            });
            if (enableDebugLog) Debug.Log("ShootWaterControllerに注入予約しました");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにShootWaterControllerコンポーネントが見つかりません");
        }

        // TrackingCamera
        var trackingCamera = Object.FindObjectOfType<TrackingCamera>();
        if (trackingCamera != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(trackingCamera);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(trackingCamera);
            });
            if (enableDebugLog) Debug.Log("TrackingCameraに注入予約しました");
        }
        else
        {
            Debug.LogError("TrackingCameraコンポーネントが見つかりません");
        }

        // WaterTank
        var waterTank = Object.FindAnyObjectByType<WaterTank>();
        if (waterTank != null)
        {
            // 1. インスタンスを登録
            builder.RegisterInstance(waterTank);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                resolver.Inject(waterTank);
            });
            if (enableDebugLog) Debug.Log("WaterTankに注入予約しました");
        }
        else
        {
            Debug.LogWarning("WaterTankコンポーネントが見つかりません");
        }

        // FireCheckZone
        var fireCheckZones = Object.FindObjectsByType<FireCheckZone>(FindObjectsSortMode.None);
        if (fireCheckZones.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<FireCheckZone>>(fireCheckZones);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var fireCheckZone in fireCheckZones)
                {
                    resolver.Inject(fireCheckZone);
                }
            });
            if (enableDebugLog) Debug.Log($"{fireCheckZones.Length}個のFireCheckZoneを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("FireCheckZoneコンポーネントが見つかりません");
        }
        // BurningFireCheckZone
        var burningFireCheckZones = Object.FindObjectsByType<BurningFireCheckZone>(FindObjectsSortMode.None);
        if (burningFireCheckZones.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<BurningFireCheckZone>>(burningFireCheckZones);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var burningFireCheckZone in burningFireCheckZones)
                {
                    resolver.Inject(burningFireCheckZone);
                }
            });
            if (enableDebugLog) Debug.Log($"{burningFireCheckZones.Length}個のBurningFireCheckZoneを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("BurningFireCheckZoneコンポーネントが見つかりません");
        }
        
        // CollectibleItem
        var collectibleItems = Object.FindObjectsByType<CollectibleItem>(FindObjectsSortMode.None);
        if (collectibleItems.Length > 0)
        {
            // 1. リストを登録
            builder.RegisterInstance<IReadOnlyList<CollectibleItem>>(collectibleItems);
            // 2. 構築後にDIを実行
            builder.RegisterBuildCallback(resolver =>
            {
                foreach (var collectibleItem in collectibleItems)
                {
                    resolver.Inject(collectibleItem);
                }
            });
            if (enableDebugLog) Debug.Log($"{collectibleItems.Length}個のCollectibleItemを登録 & 注入予約");
        }
        else
        {
            Debug.LogWarning("CollectibleItemコンポーネントが見つかりません");
        }   
    }

}