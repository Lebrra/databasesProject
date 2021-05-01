using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchManager : MonoBehaviour
{
    public GameObject Panel;

    [Tooltip("value = 0: Game Query\nvalue = 1: Dev Query")]
    public TMP_Dropdown DataTypeDrop;

    [Tooltip("value = 0: game_name\nvalue = 1: rank\nvalue = 2: devID\nvalue = 3: publisher\nvalue = 4: platform\nvalue = 5: genre")]
    public TMP_Dropdown GamesOptDrop;

    [Tooltip("value = 0: dev_name\nvalue = 1: city\nvalue = 2: country")]
    public TMP_Dropdown DevsOptDrop;

    public TMP_InputField SearchBar;

    public void OnTypeChange()
    {
        if (DataTypeDrop.value == 0)
        {
            GamesOptDrop.gameObject.SetActive(true);
            DevsOptDrop.gameObject.SetActive(false);
        }
        else if (DataTypeDrop.value == 1)
        {
            GamesOptDrop.gameObject.SetActive(false);
            DevsOptDrop.gameObject.SetActive(true);
        }
        else Debug.LogError("Invalid data type.", DataTypeDrop.gameObject);
    }

    private void OnEnable()
    {
        OnTypeChange();
    }

    public void EnablePanel(bool enable)
    {
        Panel.SetActive(enable);
    }

    public void Search()
    {
        if (SearchBar.text == "")
        {
            Debug.Log("nothing searched.");
            return;
        }

        if (DataTypeDrop.value == 0) GameSearch(SearchBar.text);
        else if (DataTypeDrop.value == 1) DevSearch(SearchBar.text);
        else Debug.LogError("Invalid data type.", DataTypeDrop.gameObject);
    }

    void GameSearch(string search)
    {
        Debug.Log("Starting search for Game...");
        LoadingScreen.instance?.EnableScreen(true);

        switch (GamesOptDrop.value)
        {
            case 0:     // game_name

                break;

            case 1:     // rank
                int rank = -1;
                if (int.TryParse(search, out rank))
                {
                    EnablePanel(false);
                    DataUIPooler.poolInstance.GetGamePanel(rank);
                }
                else Debug.LogWarning("Invalid search string for rank search.");

                break;

            case 2:     // devID

                break;

            case 3:     // publisher

                break;

            case 4:     // platform     <- enum?

                break;

            case 5:     // genre        <- enum?

                break;

            default:
                Debug.LogWarning("Invalid game option type.", GamesOptDrop.gameObject);
                return;
        }
    }

    void DevSearch(string search)
    {
        Debug.Log("Starting search for Developer...");
        LoadingScreen.instance?.EnableScreen(true);

        switch (DevsOptDrop.value)
        {
            case 0:     // dev_name

                break;

            case 1:     // city

                break;

            case 2:     // country

                break;

            default:
                Debug.LogWarning("Invalid dev option type.", DevsOptDrop.gameObject);
                return;
        }
    }
}
