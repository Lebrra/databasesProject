using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool buttonEnabled = true;

    public int devID = -1;

    Color32 defaultText = new Color32(30, 72, 149, 255);
    Color32 hoverText = new Color32(89, 210, 231, 255);
    Color32 clickText = new Color32(241, 241, 241, 255);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (devID > 0)
        {
            if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = clickText;

            DataUIPooler.poolInstance.GetDevPanel(devID);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = hoverText;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = defaultText;
    }

    private void OnEnable()
    {
        if (!buttonEnabled) return;

        if (GetComponent<TMPro.TextMeshProUGUI>()) GetComponent<TMPro.TextMeshProUGUI>().color = defaultText;
    }

    public void EnableButton(int id)
    {
        buttonEnabled = id > 0;
        devID = id;
    }
}
