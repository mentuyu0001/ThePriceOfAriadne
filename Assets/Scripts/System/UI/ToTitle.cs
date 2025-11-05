using UnityEngine;

public class ToTitle : Button
{
    public async void ToTitle()
    {
        await fadeController.FadeOut(animationTime);

        gameSceneManager.LoadTitle();
    }
}
