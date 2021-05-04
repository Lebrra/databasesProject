using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    bool setupNeeded = true;
    TMPro.TextMeshProUGUI myText;
    TMPro.TextMeshProUGUI myLabel;
    TMPro.TextMeshProUGUI myLabel2;
    UnityEngine.UI.RawImage myRawImage;
    UnityEngine.UI.Image myImage;

    public bool buttonEnabled = true;

    public int gameRank = -1;
    public (int, string) alternateAction = (-1, "");

    Color32 defaultText = new Color32(30, 72, 149, 255);
    Color32 hoverText = new Color32(89, 210, 231, 255);
    Color32 clickText = new Color32(241, 241, 241, 255);

    Color defaultImg = Color.white;
    Color32 hoverImg = new Color32(215, 255, 189, 255);
    Color32 clickImg = new Color32(176, 243, 130, 255);

    public void OnPointerClick(PointerEventData eventData)
    {
        if(alternateAction.Item1 > -1)
        {
            SearchManager.instance.DataTypeDrop.value = 0;

            // do this instead
            if(alternateAction.Item1 == 0)
            {
                // do a platform search using Item2
                SearchManager.instance.GamesOptDrop.value = 4;
            }
            else
            {
                // do a genre search using Item2
                SearchManager.instance.GamesOptDrop.value = 5;
            }

            SearchManager.instance.SearchBar.text = alternateAction.Item2;
            SearchManager.instance.Search();

            return;
        }

        if (gameRank > 0)
        {
            if (myText) myText.color = clickText;
            if (myRawImage) myRawImage.color = clickImg;
            if (myImage) myImage.color = clickImg;

            DataUIPooler.poolInstance.GetGamePanel(gameRank);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (myText) myText.color = hoverText;
        if (myRawImage) myRawImage.color = hoverImg;
        if (myImage) myImage.color = hoverImg;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!buttonEnabled) return;

        if (myText) myText.color = defaultText;
        if (myRawImage) myRawImage.color = defaultImg;
        if (myImage) myImage.color = defaultImg;
    }

    private void OnEnable()
    {
        if (setupNeeded)
        {
            myText = GetComponent<TMPro.TextMeshProUGUI>();
            myRawImage = GetComponent<UnityEngine.UI.RawImage>();
            myImage = GetComponent<UnityEngine.UI.Image>();
            if (myImage)
            {
                myLabel = transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
                myLabel2 = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
            }
            setupNeeded = false;
        }

        if (!buttonEnabled) return;

        if (myText) myText.color = defaultText;
        if (myRawImage) myRawImage.color = defaultImg;
        if (myImage) myImage.color = defaultImg;
    }

    public void EnableButton(int rank)
    {
        buttonEnabled = rank > 0;
        gameRank = rank;
    }

    public void SetValues(string name, int rank, string platform)
    {
        if (myLabel) myLabel.text = name;
        if (myLabel2) myLabel2.text = platform;
        gameRank = rank;
        alternateAction.Item1 = -1;
    }

    public void SetValues(string name, string amount, int action)
    {
        alternateAction = (action, name);
        myLabel.text = name;
        myLabel2.text = amount;
    }
}
