using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SearchManager : MonoBehaviour
{
    public static SearchManager instance;

    public GameObject SearchingPanel;
    public GameObject ResultsPanel;

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
    public int resultsPage = 0;     // 15 fit in a 'page'
    public int currentResultsCount = -1;
    public List<GameData> gameResults;
    public List<DevData> devResults;

    private void Awake()
    {
        if (instance)
            if (instance != this) Destroy(instance);

        instance = this;
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

        switch (GamesOptDrop.value)
        {
            case 0:     // game_name
                string param = "%" + search.ToLower() + "%";

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

    void ShowSearchResults()
    {
        if (!ResultsPanel.activeInHierarchy) EnableResultsPanel(true);

        UpdatePageButtons();

        if (currentResultsCount < 1)
        {
            noResultsText.SetActive(true);
            return;
        }

        noResultsText.SetActive(false);
        foreach (GameObject b in resultsButtons) b.SetActive(false);

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

                resultsButtons[i].GetComponent<GameButton>().SetValues(gameResults[resultsIterator].name, gameResults[resultsIterator].rank);
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

        if ((resultsPage + 1) * 15 < currentResultsCount)
        {
            rightButton.SetActive(true);
            bottomText.text = "Showing " + (resultsPage * 15 + 1) + "-" + ((resultsPage + 1) * 15).ToString() + " of " + currentResultsCount.ToString();
        }
        else
        {
            rightButton.SetActive(false);
            bottomText.text = "Showing " + (resultsPage * 15 + 1) + "-" + currentResultsCount.ToString() + " of " + currentResultsCount.ToString();
        }
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
}
