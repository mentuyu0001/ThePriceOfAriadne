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
        // Singletons/ItemDataから取得するオブジェクト
        var itemDataObject = GameObject.Find("Singletons/ItemData");
        if (itemDataObject != null)
        {
            // InventoryDataの登録
            var inventoryData = itemDataObject.GetComponentInChildren<InventoryData>();
            if (inventoryData != null)
            {
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
        }
        else
        {
            Debug.LogError("PlayerオブジェクトにPlayerPartsコンポーネントが見つかりません");
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
                }
            });
        }

        // GameDataManagerを自動検索
        var gameDataManager = Object.FindAnyObjectByType<GameDataManager>();
        if (gameDataManager != null)
        {
            builder.RegisterInstance(gameDataManager);
            builder.RegisterBuildCallback(resolver => resolver.Inject(gameDataManager)); // ← 依存注入を追加
            
        }
        else
        {
            Debug.LogError("GameDataManagerが見つかりません");
        }
    }
}
