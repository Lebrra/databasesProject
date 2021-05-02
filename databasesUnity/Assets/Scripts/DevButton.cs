using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool setupNeeded = true;
    TMPro.TextMeshProUGUI myText;
    TMPro.TextMeshProUGUI myLabel;
    UnityEngine.UI.Image myImage;

    public bool buttonEnabled = true;

    public int devID = -1;

    Color32 defaultText = new Color32(30, 72, 149, 255);
    Color32 hoverText = new Color32(89, 210, 231, 255);
    Color32 clickText = new Color32(241, 241, 241, 255);

    Color defaultImg = Color.white;
    Color32 hoverImg = new Color32(215, 255, 189, 255);
    Color32 clickImg = new Color32(176, 243, 130, 255);

    public void OnPointerClick(PointerEventData eventData)
    {
        if (devID > 0)
        {
            if (myText) myText.color = clickText;
            if (myImage) myImage.color = clickImg;

            DataUIPooler.poolInstance.GetDevPanel(devID);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (myText) myText.color = hoverText;
        if (myImage) myImage.color = hoverImg;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (myText) myText.color = defaultText;
        if (myImage) myImage.color = defaultImg;
    }

    private void OnEnable()
    {
        if (setupNeeded)
        {
            myText = GetComponent<TMPro.TextMeshProUGUI>();
            myImage = GetComponent<UnityEngine.UI.Image>();
            if (myImage) myLabel = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            setupNeeded = false;
        }

        if (!buttonEnabled) return;

        if (myText) myText.color = defaultText;
        if (myImage) myImage.color = defaultImg;
    }

    public void EnableButton(int id)
    {
        buttonEnabled = id > 0;
        devID = id;
    }

    public void SetValues(string name, int id)
    {
        if (myLabel) myLabel.text = name;
        devID = id;
    }
}
