﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class DevLoader : DataLoader
{
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
    public static DevData ddat;
    /// <summary>
    /// Loads game data into panel
    /// **This will need data parameters
    /// </summary>
    public void LoadDevData(DevData dev)
    {
        if (dev.id < 0 || dev.id > 687)
        {
            EnablePanel(false);
            Debug.LogWarning("Invalid dev entry. ID = " + dev.id);
            return;
        }

        // ADDED: one line to push dev up in visibility boy i really hope this idea works
        ddat = dev;

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

            games[0].GetComponent<GameButton>().EnableButton(-1);
            games[0].color = new Color32(24, 24, 24, 255);
            games[0].fontStyle ^= FontStyles.Underline;

            images[0].transform.parent.gameObject.SetActive(false);
            images[1].transform.parent.gameObject.SetActive(false);
            images[2].transform.parent.gameObject.SetActive(false);

            totGamesText.text = "0";
            percentGamesText.text = "0%";
        }
    }

    void LoadNotableGames(DevData dev)
    {
        games[0].color = new Color32(30, 72, 149, 255);
        games[0].fontStyle = FontStyles.Underline;

        for (int i = 0; i < 3; i++)
        {
            if (dev.notableGameRanks[i] > 0)
            {
                // load image and name
                games[i].text = dev.notableGames[i];
                images[i].transform.parent.gameObject.SetActive(true);
                StartCoroutine(LoadImageFromURL(dev.notableGameImgURLs[i], i));

                games[i].GetComponent<GameButton>().EnableButton(dev.notableGameRanks[i]);
                images[i].GetComponent<GameButton>().EnableButton(dev.notableGameRanks[i]);
            }
            else
            {
                images[i].transform.parent.gameObject.SetActive(false);
                games[i].text = "";

                games[i].GetComponent<GameButton>().EnableButton(-1);
                images[i].GetComponent<GameButton>().EnableButton(-1);
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

    //LoadDevData(SQLConnection.GetAllDevData(70));

    public override void ResetPanel()
    {
        // is this needed?
    }
}

public abstract class DataLoader : MonoBehaviour
{
    public GameObject Panel;
    public bool active = true;

    public void EnablePanel(bool enable)
    {
        Panel.SetActive(enable);
    }

    public virtual void ResetPanel() { }

    private void OnEnable()
    {
        LoadingScreen.instance?.EnableScreen(false);
    }
}
