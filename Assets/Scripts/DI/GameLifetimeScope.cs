using UnityEngine;
using VContainer;
using VContainer.Unity;
using System.Collections.Generic;

/// <summary>
/// ゲーム全体のライフタイムスコープを設定するクラス
/// </summary>
public class GameLifetimeScope : LifetimeScope
{
    [SerializeField] private bool enableDebugLog = false; // デバッグログの表示制御

    protected override void Configure(IContainerBuilder builder)
    {
        if (enableDebugLog) Debug.Log("GameLifetimeScope 登録開始");

        // Player, Ground, GameOverManager, PlayerStatusの登録（ここは変更なし）
        #region Singleton_Objects
        // プレイヤーを自動検索
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            builder.RegisterInstance(player);
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
            builder.RegisterInstance(partsManager);
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
            builder.RegisterInstance(playerAnimationManager);
            if (enableDebugLog) Debug.Log($"PlayerAnimationManagerが正常に登録されました: {playerAnimationManager.name}");
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにPlayerAnimationManagerコンポーネントが見つかりません");
        }

        // Playerにattachされているコンポーネントの登録
        if (player != null)
        {
            // PlayerAirCheckerを登録（Playerオブジェクトから取得）
            var playerAirChecker = player.GetComponent<PlayerAirChecker>();
            if (playerAirChecker != null)
            {
                builder.RegisterInstance(playerAirChecker);
                if (enableDebugLog) Debug.Log($"PlayerAirCheckerが正常に登録されました: {playerAirChecker.name}");
            }
            else
            {
                Debug.LogError("PlayerオブジェクトにPlayerAirCheckerコンポーネントが見つかりません");
            }
             
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
        
        if (enableDebugLog) Debug.Log("GameLifetimeScope 登録完了");
    }

}