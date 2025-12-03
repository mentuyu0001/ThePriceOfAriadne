using UnityEngine;

public class TitleFade : MonoBehaviour
{
    [SerializeField] private FadeController fadeController;

    void Start()
    {
        fadeController.FadeIn(3.0f);
    }
}
