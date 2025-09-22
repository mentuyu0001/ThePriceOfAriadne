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
        // InventoryDataをシングルトンとして登録
        builder.Register<InventoryData>(Lifetime.Singleton);

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
    }
}
