using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// チュートリアル用の文章を出す
/// </summary>
public class HideTutorial : MonoBehaviour
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
                gameTextDisplay.HideText();
            }
        }
    }
}
