using System.Threading.Tasks;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class FirstShowTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;
    [SerializeField] private string tutorialText1;
    [SerializeField] private string tutorialText2;

    private bool isText = false;

    float secondsToWait = 4.0f;

    async void OnTriggerStay2D(Collider2D other)
    {
        if (!isText)
        {
            if (other.gameObject.tag == "Player")
            {
                isText = true;
                ShowAndHideTexts();
            }
        }
    }
    
    private async void ShowAndHideTexts()
    {
        await gameTextDisplay.ShowText(tutorialText1);

        await UniTask.Delay(TimeSpan.FromSeconds(secondsToWait));

        gameTextDisplay.HideText();

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));

        await gameTextDisplay.ShowText(tutorialText2);
    }
}
