using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool buttonEnabled = true;

    public int gameRank = -1;

    Color32 defaultText = new Color32(30, 72, 149, 255);
    Color32 hoverText = new Color32(89, 210, 231, 255);
    Color32 clickText = new Color32(241, 241, 241, 255);

    Color defaultImg = Color.white;
    Color32 hoverImg = new Color32(215, 255, 189, 255);
    Color32 clickImg = new Color32(176, 243, 130, 255);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameRank > 0)
        {
            if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = clickText;
            if (GetComponent<UnityEngine.UI.RawImage>()) GetComponent<UnityEngine.UI.RawImage>().color = clickImg;

            DataUIPooler.poolInstance.GetGamePanel(gameRank);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = hoverText;
        if (GetComponent<UnityEngine.UI.RawImage>()) GetComponent<UnityEngine.UI.RawImage>().color = hoverImg;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = defaultText;
        if (GetComponent<UnityEngine.UI.RawImage>()) GetComponent<UnityEngine.UI.RawImage>().color = defaultImg;
    }

    private void OnEnable()
    {
        if (!buttonEnabled) return;

        if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = defaultText;
        if (GetComponent<UnityEngine.UI.RawImage>()) GetComponent<UnityEngine.UI.RawImage>().color = defaultImg;
    }

    public void EnableButton(int rank)
    {
        buttonEnabled = rank > 0;
        gameRank = rank;
    }
}
