using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class DevLoader : MonoBehaviour
{
    public GameObject DevPanel;

    [Header("Left Texts")]
    int currentDevID = -1;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI cityText;
    public TextMeshProUGUI countryText;
    public TextMeshProUGUI estabText;
    public TextMeshProUGUI noteText;

    public TextMeshProUGUI totGamesText;
    public TextMeshProUGUI percentGamesText;

    [Header("Image References")]
    public Sprite defaultGame;
    public RawImage[] images;

    [Header("Games List")]
    public TextMeshProUGUI[] games;

    public void EnablePanel(bool enable)
    {
        DevPanel.SetActive(enable);
    }

    /// <summary>
    /// Loads game data into panel
    /// **This will need data parameters
    /// </summary>
    public void LoadDevData(DevData dev)
    {
        if (dev.id < 1 || dev.id > 687)
        {
            EnablePanel(false);
            Debug.LogWarning("Invalid dev entry.");
            return;
        }

        //if (dev.urlImg != "") StartCoroutine(LoadImageFromURL(dev.urlImg));
        //else image.texture = defaultGame.texture;

        //load basic texts: 
        titleText.text = dev.name;
        cityText.text = dev.city;
        countryText.text = dev.country;
        estabText.text = dev.established.ToString();
        noteText.text = dev.note;

        dev.gamesCount = SQLConnection.GamesCountFromDev(dev.id);
        if (dev.gamesCount > 0)
        {
            totGamesText.text = dev.gamesCount.ToString();
            float percent = Mathf.Round((float)dev.gamesCount / 55793F * 1000F) / 1000F;
            percentGamesText.text = percent.ToString() + "%";

            Debug.Log("Getting Games list for dev " + dev.id);
            LoadNotableGames(SQLConnection.TopThreeGamesFromDev(dev));
        }
        else
        {
            games[1].text = games[2].text = "";
            games[0].text = "No notable games found.";

            images[0].transform.parent.gameObject.SetActive(false);
            images[1].transform.parent.gameObject.SetActive(false);
            images[2].transform.parent.gameObject.SetActive(false);
        }
    }

    void LoadNotableGames(DevData dev)
    {
        for(int i = 0; i < 3; i++)
        {
            if (dev.notableGameRanks[i] > 0)
            {
                // load image and name
                games[i].text = dev.notableGames[i];
                images[i].transform.parent.gameObject.SetActive(true);
                StartCoroutine(LoadImageFromURL(dev.notableGameImgURLs[i], i));
            }
            else
            {
                images[i].transform.parent.gameObject.SetActive(false);
                games[i].text = "";
            }
        }
    }

    /// <summary>
    /// Load game image from url. This should also be a clickable link to its url (non-image)
    /// </summary>
    IEnumerator LoadImageFromURL(string url, int index)
    {
        string fullURL = "https://www.vgchartz.com" + url;

        UnityWebRequest link = UnityWebRequestTexture.GetTexture(fullURL);
        yield return link.SendWebRequest();

        if (link.isNetworkError || link.isHttpError)
        {
            images[index].texture = defaultGame.texture;
            Debug.LogError(link.error);
        }
        else
        {
            images[index].texture = ((DownloadHandlerTexture)link.downloadHandler).texture;
        }

        link.Dispose();
    }

    private void Start()
    {
        //LoadDevData(SQLConnection.GetAllDevData(415));
        LoadDevData(SQLConnection.GetAllDevData(70));
    }
}
