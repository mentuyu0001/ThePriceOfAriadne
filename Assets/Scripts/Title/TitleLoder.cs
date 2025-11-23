using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;

public class TitleLoder : MonoBehaviour
{
    [SerializeField] private GameSceneManager gameSceneManager;
    [SerializeField] private FadeController fadeController;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private Clickable clickable;

    public async void GameStart()
    {
        EventSystem.current.SetSelectedGameObject(null);
        clickable.DisClickable();

        gameSceneManager.LoadPlorogue();
    }
}
