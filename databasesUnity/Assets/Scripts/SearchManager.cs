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
    public int dataType = 0;

    [Tooltip("value = 0: game_name\nvalue = 1: rank\nvalue = 2: devID\nvalue = 3: publisher\nvalue = 4: platform\nvalue = 5: genre")]
    public TMP_Dropdown GamesOptDrop;

    [Tooltip("value = 0: dev_name\nvalue = 1: city\nvalue = 2: country")]
    public TMP_Dropdown DevsOptDrop;

    public TMP_InputField SearchBar;

    [Header("Results References")]
    public TextMeshProUGUI resultsHeader;
    public GameObject[] resultsButtons;
    public GameObject noResultsText;

    public GameObject leftButton;
    public GameObject rightButton;
    public TextMeshProUGUI bottomText;

    [Header("Special Searching")]
    public GameObject SpecialPanel;
    public bool inSpecial = false;

    [Header("Results Data")]
    public bool searchActive = false;
    public int resultsPage = 0;     // 15 fit in a 'page'
    public int currentResultsCount = -1;

    public List<GameData> gameResults;
    public List<DevData> devResults;
    public List<(string, string)> otherResults;

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
        dataType = DataTypeDrop.value;

        if (dataType == 0)
        {
            GamesOptDrop.gameObject.SetActive(true);
            DevsOptDrop.gameObject.SetActive(false);
        }
        else if (dataType == 1)
        {
            GamesOptDrop.gameObject.SetActive(false);
            DevsOptDrop.gameObject.SetActive(true);
        }
        else Debug.LogWarning("Invalid data type.", DataTypeDrop.gameObject);
    }

    private void OnEnable()
    {
        OnTypeChange();
        SearchBar.text = "";
    }

    public void EnableSearchMenu(bool enable)
    {
        EnableSpecialPanel(false);
        EnableSearchPanel(enable);
        EnableResultsPanel(enable && searchActive);
        BackButton.SetActive(enable);

        if (enable)
        {
            OnTypeChange();
            SearchBar.text = "";
        }
    }

    public void EnableSearchPanel(bool enable)
    {
        SearchingPanel.SetActive(enable);
    }

    public void EnableResultsPanel(bool enable)
    {
        ResultsPanel.SetActive(enable);
    }

    public void EnableSpecialPanel(bool enable)
    {
        SpecialPanel.SetActive(enable);
        inSpecial = enable;
    }

    public void Search()
    {
        if (SearchBar.text == "")
        {
            Debug.Log("nothing searched.");
            return;
        }
        dataType = DataTypeDrop.value;
        if (dataType == 0) GameSearch(SearchBar.text);
        else if (dataType == 1) DevSearch(SearchBar.text);
        else Debug.LogError("Invalid data type.", DataTypeDrop.gameObject);
    }

    void GameSearch(string search)
    {
        Debug.Log("Starting search for Game...");
        LoadingScreen.instance?.EnableScreen(true);

        searchActive = true;
        resultsHeader.text = '"'.ToString() + search + '"'.ToString() + ":";

        switch (GamesOptDrop.value)
        {
            case 0:     // game_name
                RecieveResults(SQLConnection.GamesNameSearch(search));
                break;

            case 1:     // rank
                int rank = -1;
                if (int.TryParse(search, out rank))
                {
                    searchActive = false;   // do not return to search results
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
        resultsHeader.text = '"'.ToString() + search + '"'.ToString() + ":";

        switch (DevsOptDrop.value)
        {
            case 0:     // dev_name
                RecieveResults(SQLConnection.DevsNameSearch(search));
                break;

            case 1:     // city
                RecieveResults(SQLConnection.DevsCitySearch(search));
                break;

            case 2:     // country
                RecieveResults(SQLConnection.DevsCountrySearch(search));
                break;

            default:
                Debug.LogWarning("Invalid dev option type.", DevsOptDrop.gameObject);
                return;
        }
    }

    public void CustomSearch(int searchType)
    {
        Debug.Log("Starting custom search...");
        LoadingScreen.instance?.EnableScreen(true);

        searchActive = true;
        EnableSpecialPanel(false);

        switch (searchType)
        {
            case 0:
                DataTypeDrop.value = dataType = 0;
                resultsHeader.text = '"'.ToString() + "Best sold in Japan" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.GamesJPSearch());
                break;

            case 1:
                DataTypeDrop.value = dataType = 1;
                resultsHeader.text = '"'.ToString() + "Most Games By Developer" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.DevsMostSearch());
                break;

            case 2:
                DataTypeDrop.value = dataType = 0;
                resultsHeader.text = '"'.ToString() + "Best Critic-Rated Games" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.GamesCriticSearch());
                break;

            case 3:
                DataTypeDrop.value = dataType = 0;
                resultsHeader.text = '"'.ToString() + "Best User-Rated Games" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.GamesUserSearch());
                break;

            case 4:
                DataTypeDrop.value = dataType = 0;
                resultsHeader.text = '"'.ToString() + "Older Games" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.GamesOlderSearch());
                break;

            case 5:
                DataTypeDrop.value = dataType = 0;
                resultsHeader.text = '"'.ToString() + "Newer Games" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.GamesNewerSearch());
                break;

            case 6:     // special platform search !
                dataType = 3;
                resultsHeader.text = '"'.ToString() + "Platforms" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.PlatformSearch());
                break;

            case 7:     // special genre search !
                dataType = 2;
                resultsHeader.text = '"'.ToString() + "Genres" + '"'.ToString() + ":";
                RecieveResults(SQLConnection.GenreSearch());
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

            if (dataType == 0)
            {
                // game query
                resultsButtons[i].GetComponent<GameButton>().enabled = true;
                resultsButtons[i].GetComponent<DevButton>().enabled = false;

                resultsButtons[i].GetComponent<GameButton>().SetValues(gameResults[resultsIterator].name, gameResults[resultsIterator].rank, gameResults[resultsIterator].platform);
            }
            else if (dataType == 1)
            {
                // dev query
                resultsButtons[i].GetComponent<GameButton>().enabled = false;
                resultsButtons[i].GetComponent<DevButton>().enabled = true;

                resultsButtons[i].GetComponent<DevButton>().SetValues(devResults[resultsIterator].name, devResults[resultsIterator].id, devResults[resultsIterator].gamesCount);
            }
            else if (dataType == 2 || dataType == 3)
            {
                // other query - 2 = platform or 3 = genre
                resultsButtons[i].GetComponent<GameButton>().enabled = true;
                resultsButtons[i].GetComponent<DevButton>().enabled = false;

                resultsButtons[i].GetComponent<GameButton>().SetValues(otherResults[resultsIterator].Item1, otherResults[resultsIterator].Item2 + " Games", dataType - 2);
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

    public void RecieveResults(List<(string, string)> data)
    {
        resultsPage = 0;
        currentResultsCount = data.Count;

        otherResults = new List<(string, string)>();

        foreach ((string, string) g in data) otherResults.Add((g.Item1, g.Item2));

        ShowSearchResults();
    }

    public void GoBack()
    {
        if (searchActive)
        {
            Debug.Log("exit results...");
            EnableResultsPanel(false);
            SearchBar.text = "";

            gameResults.Clear();
            devResults.Clear();
            searchActive = false;
        }
        else if (inSpecial)
        {
            Debug.Log("exit special panel...");
            EnableSpecialPanel(false);
        }
        else
        {
            // go back before search panel
            Debug.Log("returning to Menu...");
            Manager.instance.NavigateTo(1);
        }
    }
}
