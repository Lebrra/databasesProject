using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUIPooler : MonoBehaviour
{
    public static DataUIPooler poolInstance;

    public GameObject GamePanel;
    public GameObject DevPanel;

    List<GameObject> gamePanelPool;
    List<GameObject> devPanelPool;

    public List<DataLoader> currentList;

    public Transform BackButton; // there's definitely a better way to do this

    private void Awake()
    {
        if (poolInstance) Destroy(this);
        else poolInstance = this;

        gamePanelPool = new List<GameObject>();
        devPanelPool = new List<GameObject>();

        currentList = new List<DataLoader>();
    }

    public void GetGamePanel(int rank)  // might be data not int
    {
        GameData gameData = SQLConnection.GetAllGameData(rank);

        if(gameData.rank == -1)
        {
            Debug.LogWarning("Cannot load game data.");
            return;
        }

        LoadingScreen.instance?.EnableScreen(true);
        // disable search screens
        GameLoader panel;

        if (gamePanelPool.Count > 0)
        {
            // use an old one
            panel = gamePanelPool[0].GetComponent<GameLoader>();
            gamePanelPool.Remove(panel.gameObject);

            panel.transform.SetSiblingIndex(transform.childCount - 1);
            panel.EnablePanel(true);
        }
        else
        {
            // make a new one
            panel = Instantiate(GamePanel, transform).GetComponent<GameLoader>();
        }

        panel.LoadGameData(gameData);
        currentList.Add(panel);

        BackButton.SetSiblingIndex(transform.childCount - 1);
    }

    public void GetDevPanel(int id)
    {
        DevData devData = SQLConnection.GetAllDevData(id);

        if (devData.id == -1)
        {
            Debug.LogWarning("Cannot load dev data.");
            return;
        }

        LoadingScreen.instance?.EnableScreen(true);
        // disable search screens
        DevLoader panel;

        if (devPanelPool.Count > 0)
        {
            // use an old one
            panel = devPanelPool[0].GetComponent<DevLoader>();
            devPanelPool.Remove(panel.gameObject);

            panel.transform.SetSiblingIndex(transform.childCount - 1);
            panel.EnablePanel(true);
        }
        else
        {
            // make a new one
            panel = Instantiate(DevPanel, transform).GetComponent<DevLoader>();
        }

        panel.LoadDevData(devData);
        currentList.Add(panel);

        BackButton.SetSiblingIndex(transform.childCount - 1);
    }

    public void GoBack()
    {
        if(currentList.Count == 1)
        {
            // return to query page
            Debug.Log("Returning to search...");
        }
        else
        {
            // remove latest page
            DataLoader lastUsed = currentList[currentList.Count - 1];
            currentList.Remove(lastUsed);
            ReturnPanel(lastUsed);
        }
    }

    public void ReturnPanel(DataLoader panel)
    {
        panel.EnablePanel(false);
        if (panel.GetComponent<GameLoader>()) gamePanelPool.Add(panel.gameObject);
        else devPanelPool.Add(panel.gameObject);
    }
}
