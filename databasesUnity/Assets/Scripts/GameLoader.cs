using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GameLoader : DataLoader
{
    [Header("Basic Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI platformText;
    public TextMeshProUGUI genreText;
    public TextMeshProUGUI esrbText;
    public TextMeshProUGUI yearText;
    public TextMeshProUGUI devText;     // will need more complexity than this
    int currentDevID = -1;
    public TextMeshProUGUI pubText;
    public TextMeshProUGUI criticText;
    public TextMeshProUGUI userText;

    public TextMeshProUGUI rankText;

    [Header("Image References")]
    public RawImage image;
    public Sprite defaultGame;
    string gameURL = "";

    [Header("Ranking References")]
    public TextMeshProUGUI globalText;
    public Image globalFill;
    public TextMeshProUGUI naText;
    public Image naFill;
    public TextMeshProUGUI palText;
    public Image palFill;
    public TextMeshProUGUI jpText;
    public Image jpFill;
    public TextMeshProUGUI otherText;
    public Image otherFill;

    float maxSales;
    public TextMeshProUGUI graphTopText;
    public TextMeshProUGUI graphBotText;

    /// <summary>
    /// Loads game data into panel
    /// **This will need data parameters
    /// </summary>
    public void LoadGameData(GameData game)
    {
        if (game.rank < 1 || game.rank > 55793)
        {
            EnablePanel(false);
            Debug.LogWarning("Invalid game entry. Rank = " + game.rank);
            return;
        }

        if (game.urlImg != "") StartCoroutine(LoadImageFromURL(game.urlImg));
        else image.texture = defaultGame.texture;

        gameURL = game.url;

        //load basic texts: 
        rankText.text = game.rank.ToString();
        titleText.text = game.name;
        platformText.text = game.platform;
        genreText.text = game.genre;
        esrbText.text = game.esrb;
        yearText.text = game.year.ToString();

        currentDevID = game.devID;
        devText.text = game.dev;
        if (currentDevID == -1)
        {
            devText.color = new Color32(24, 24, 24, 255);
            devText.fontStyle ^= FontStyles.Underline;
            devText.GetComponent<DevButton>().EnableButton(-1);
        }
        else
        {
            devText.color = new Color32(30, 72, 149, 255);
            devText.fontStyle = FontStyles.Underline;
            devText.GetComponent<DevButton>().EnableButton(currentDevID);
        }


        pubText.text = game.pub;

        if (game.ratingCritic > 0) criticText.text = game.ratingCritic.ToString();
        else criticText.text = "NA";
        if (game.ratingUser > 0) userText.text = game.ratingUser.ToString();
        else userText.text = "NA";

        // load bars and numbers
        if (game.salesGlobal > -1)
        {
            globalFill.fillAmount = 1F;
            globalText.text = game.salesGlobal.ToString();

            maxSales = game.salesGlobal;
            float lineOne = (float)Mathf.Round(maxSales * 0.75F * 10F) / 10F;
            graphTopText.text = lineOne.ToString();

            float lineTwo = (float)Mathf.Round(maxSales * 0.375F * 10F) / 10F;
            graphBotText.text = lineTwo.ToString();
        }
        else
        {
            globalFill.fillAmount = 0F;
            globalText.text = "NA";
            graphTopText.text = graphBotText.text = "";
        }

        if (game.salesNA > -1)  // assuming if NA exists, all locations do
        {
            naFill.fillAmount = game.salesNA / maxSales;
            naText.text = game.salesNA.ToString();

            palFill.fillAmount = game.salesPAL / maxSales;
            palText.text = game.salesPAL.ToString();

            jpFill.fillAmount = game.salesJP / maxSales;
            jpText.text = game.salesJP.ToString();

            otherFill.fillAmount = game.salesOther / maxSales;
            otherText.text = game.salesOther.ToString();
        }
        else
        {
            naFill.fillAmount = palFill.fillAmount = jpFill.fillAmount = otherFill.fillAmount = 0F;
            naText.text = palText.text = jpText.text = otherText.text = "NA";
        }
    }

    /// <summary>
    /// Load game image from url. This should also be a clickable link to its url (non-image)
    /// </summary>
    IEnumerator LoadImageFromURL(string url)
    {
        // url header: https://www.vgchartz.com
        string fullURL = "https://www.vgchartz.com" + url;

        UnityWebRequest link = UnityWebRequestTexture.GetTexture(fullURL);
        yield return link.SendWebRequest();

        if (link.isNetworkError || link.isHttpError)
        {
            image.texture = defaultGame.texture;
            Debug.LogError(link.error);
        }
        else
        {
            image.texture = ((DownloadHandlerTexture)link.downloadHandler).texture;
        }
        
        link.Dispose();
    }

    public override void ResetPanel()
    {
        // is this needed?
    }

    public void OpenURL()
    {
        if (gameURL != "")
            Application.OpenURL(gameURL);
    }

    //LoadGameData(SQLConnection.GetAllGameData(157));
}
