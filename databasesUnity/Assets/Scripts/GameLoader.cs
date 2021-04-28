using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GameLoader : MonoBehaviour
{
    [Header("UI References")]
    public RawImage image;
    public Sprite defaultGame;

    /// <summary>
    /// Loads game data into panel
    /// **This will need data parameters
    /// </summary>
    public void LoadGameData()
    {
        
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

    private void Start()
    {
        //StartCoroutine(LoadImageFromURL("/games/boxart/full_8932480AmericaFrontccc.jpg"));
    }
}
