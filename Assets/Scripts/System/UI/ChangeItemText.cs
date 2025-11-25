using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ChangeItemText : MonoBehaviour, ISelectHandler
{
    [SerializeField] string changedText = "sample";

    [SerializeField] TextMeshProUGUI itemText1;
    [SerializeField] TextMeshProUGUI itemText2;


    public void OnSelect(BaseEventData eventData)
    {
        if (itemText1 != null) itemText1.text = changedText;
        if (itemText2 != null) itemText2.text = "";
    }
}
