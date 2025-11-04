using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// チュートリアル用の文章を出す
/// </summary>
public class EnterHideTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;

    private bool isText = false;

    async Task OnTriggerStay2D(Collider2D other)
    {
        if (!isText)
        {
            if (other.gameObject.tag == "Player")
            {
                isText = true;
            }
        }
    }

    public void HideText()
    {
        if (isText)
        {
            gameTextDisplay.HideText();
        }
    }
}
