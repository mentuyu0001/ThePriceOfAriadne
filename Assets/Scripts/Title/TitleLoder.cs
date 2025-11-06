using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

public class TitleLoder : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private FadeController fadeController;

    public async void GameStart()
    {
        EventSystem.current.SetSelectedGameObject(null);
        await fadeController.FadeOut(3.0f);
        await UniTask.Delay(TimeSpan.FromSeconds(3.0f));
    }
}
