using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchManager : MonoBehaviour
{
    public static SearchManager instance;

    public GameObject SearchingPanel;
    public GameObject ResultsPanel;
    public GameObject BackButton;

    [Header("Searching References")]
    [Tooltip("value = 0: Game Query\nvalue = 1: Dev Query")]
    public TMP_Dropdown DataTypeDrop;

    [Tooltip("value = 0: game_name\nvalue = 1: rank\nvalue = 2: devID\nvalue = 3: publisher\nvalue = 4: platform\nvalue = 5: genre")]
    public TMP_Dropdown GamesOptDrop;

    [Tooltip("value = 0: dev_name\nvalue = 1: city\nvalue = 2: country")]
    public TMP_Dropdown DevsOptDrop;

    public TMP_InputField SearchBar;

    [Header("Results References")]
    public GameObject[] resultsButtons;
    public GameObject noResultsText;

    public GameObject leftButton;
    public GameObject rightButton;
    public TextMeshProUGUI bottomText;

    [Header("Results Data")]
    public bool searchActive = false;
    public int resultsPage = 0;     // 15 fit in a 'page'
    public int currentResultsCount = -1;
    public List<GameData> gameResults;
    public List<DevData> devResults;

    private void Awake()
    {
        if (instance)
            if (instance != this) Destroy(instance);

        instance = this;
        EnableResultsPanel(false);
        foreach (GameObject b in resultsButtons) b.SetActive(false);
    }

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

    public void EnableSearchMenu(bool enable)
    {
        EnableSearchPanel(enable);
        EnableResultsPanel(enable);
        BackButton.SetActive(enable);
    }

    public void EnableSearchPanel(bool enable)
    {
        SearchingPanel.SetActive(enable);
    }

    public void EnableResultsPanel(bool enable)
    {
        ResultsPanel.SetActive(enable);
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

        searchActive = true;

        switch (GamesOptDrop.value)
        {
            case 0:     // game_name
                RecieveResults(SQLConnection.GamesNameSearch(search));
                break;

            case 1:     // rank
                int rank = -1;
                if (int.TryParse(search, out rank))
                {
                    EnableSearchPanel(false);
                    DataUIPooler.poolInstance.GetGamePanel(rank);
                }
                else Debug.LogWarning("Invalid search string for rank search.");

                break;

            case 2:     // devID
                RecieveResults(SQLConnection.GamesDevSearch(search));
                break;

            case 3:     // publisher
                RecieveResults(SQLConnection.GamesPubSearch(search));
                break;

            case 4:     // platform     <- enum?
                RecieveResults(SQLConnection.GamesPlatSearch(search));
                break;

            case 5:     // genre        <- enum?
                RecieveResults(SQLConnection.GamesGenreSearch(search));
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

        searchActive = true;

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

    void ShowSearchResults()
    {
        if (!ResultsPanel.activeInHierarchy) EnableResultsPanel(true);

        UpdatePageButtons();
        foreach (GameObject b in resultsButtons) b.SetActive(false);

        if (currentResultsCount < 1)
        {
            noResultsText.SetActive(true);
            return;
        }

        noResultsText.SetActive(false);

        for(int i = 0; i < 15; i++)     // ** changing search results button count will need to have a lot of 15s updated !!
        {
            int resultsIterator = (resultsPage * 15) + i;
            if (resultsIterator >= currentResultsCount)
            {
                Debug.Log("Last result has been loaded. Ending results load.");
                break;
            }

            resultsButtons[i].SetActive(true);

            if (DataTypeDrop.value == 0)
            {
                // game query
                resultsButtons[i].GetComponent<GameButton>().enabled = true;
                resultsButtons[i].GetComponent<DevButton>().enabled = false;

                resultsButtons[i].GetComponent<GameButton>().SetValues(gameResults[resultsIterator].name, gameResults[resultsIterator].rank, gameResults[resultsIterator].platform);
            }
            else if (DataTypeDrop.value == 1)
            {
                // dev query
                resultsButtons[i].GetComponent<GameButton>().enabled = false;
                resultsButtons[i].GetComponent<DevButton>().enabled = true;

                resultsButtons[i].GetComponent<DevButton>().SetValues(devResults[resultsIterator].name, devResults[resultsIterator].id);
            }
            else
            {
                // error
                Debug.LogError("Data type error.", DataTypeDrop.gameObject);
            }
        }

        Debug.Log("Loaded search results successfully.");
        LoadingScreen.instance?.EnableScreen(false);
    }

    void UpdatePageButtons()
    {
        if (resultsPage > 0) leftButton.SetActive(true);
        else leftButton.SetActive(false);

        int minIndex, maxIndex;
        if (currentResultsCount < 1)
        {
            rightButton.SetActive(false);
            minIndex = maxIndex = 0;
        }
        else if ((resultsPage + 1) * 15 < currentResultsCount)
        {
            rightButton.SetActive(true);
            minIndex = resultsPage * 15 + 1;
            maxIndex = (resultsPage + 1) * 15;
        }
        else
        {
            rightButton.SetActive(false);
            minIndex = resultsPage * 15 + 1;
            maxIndex = currentResultsCount;
        }

        bottomText.text = "Showing <#ADFFF4>" + minIndex + "</color>-<#ADFFF4>" + maxIndex + "</color> of <#FFE000>" + currentResultsCount.ToString();
    }

    public void IncrementSearchResults()
    {
        if ((resultsPage + 1) * 15 < currentResultsCount)
        {
            resultsPage++;
            ShowSearchResults();
        }
        else
        {
            // right button should not be enabled, disable it
            rightButton.SetActive(false);
        }
    }

    public void DecrementSearchResults()
    {
        if (resultsPage > 0)
        {
            resultsPage--;
            ShowSearchResults();
        }
        else
        {
            // left button should not be enabled, disable it
            leftButton.SetActive(false);
        }
    }

    public void RecieveResults(List<GameData> data)
    {
        resultsPage = 0;
        currentResultsCount = data.Count;

        gameResults = new List<GameData>();
        devResults = new List<DevData>();

        foreach (GameData g in data) gameResults.Add(g);

        ShowSearchResults();
    }

    public void RecieveResults(List<DevData> data)
    {
        resultsPage = 0;
        currentResultsCount = data.Count;

        gameResults = new List<GameData>();
        devResults = new List<DevData>();

        foreach (DevData d in data) devResults.Add(d);

        ShowSearchResults();
    }

    public void GoBack()
    {
        if (searchActive)
        {
            EnableResultsPanel(false);
            SearchBar.text = "";

            gameResults.Clear();
            devResults.Clear();
        }
        else
        {
            // go back before search panel
        }
    }
}
