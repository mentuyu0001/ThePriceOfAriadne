using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// チュートリアル用の文章を出す
/// </summary>
public class ShowTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;

    [SerializeField] private string tutorialText;

    private bool isText = false;

    async Task OnTriggerStay2D(Collider2D other)
    {
        if (!isText)
        {
            if (other.gameObject.tag == "Player")
            {
                isText = true;
                await gameTextDisplay.ShowText(tutorialText);   
            }
        }
    }
}
