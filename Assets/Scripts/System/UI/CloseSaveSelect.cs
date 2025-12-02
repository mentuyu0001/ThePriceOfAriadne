using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CloseSaveSele : MonoBehaviour, ISelectHandler
{
    [SerializeField] string changedText = "sample";
    [SerializeField] TextMeshProUGUI itemText1;
    [SerializeField] TextMeshProUGUI itemText2;
    [SerializeField] GameObject playerStatusDisplay;

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("CloseSaveSelect");
        playerStatusDisplay.SetActive(false);
        if (itemText1 != null) itemText1.text = changedText;
        if (itemText2 != null) itemText2.text = "";
    }
}
