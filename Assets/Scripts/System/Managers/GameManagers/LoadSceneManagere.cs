using System.Threading.Tasks;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class LoadSceneManagere : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private FadeController fadeController;

    public async void GameLoad(int stage)
    {
        await fadeController.FadeOut(3.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f));
        gameSceneManager.LoadStage(stage);
    }
}
