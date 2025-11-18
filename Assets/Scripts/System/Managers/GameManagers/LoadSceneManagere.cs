using System.Threading.Tasks;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

public class LoadSceneManagere : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private Clickable clickable;

    async void Start()
    {
        await fadeController.FadeIn(3.0f);
    }
    public async void GameLoad(int stage)
    {
        EventSystem.current.SetSelectedGameObject(null);
        clickable.DisClickable();
        await fadeController.FadeOut(3.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f));
        gameSceneManager.LoadStage(stage);
    }
}
