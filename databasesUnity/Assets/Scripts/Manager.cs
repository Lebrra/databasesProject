using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager instance;

    private void Awake()
    {
        if (instance) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(gameObject);

        SQLConnection.Connect();
    }

    void Start()
    {
        Invoke("testGame", 3);
    }

    void testGame()
    {
        /*
        GameData g = SQLConnection.GetAllGameData(1);
        Debug.Log(g.name);
        Debug.Log(g.rank);
        Debug.Log(g.url);
        Debug.Log(g.year);
        Debug.Log(g.salesGlobal);
        Debug.Log(g.devID);
        Debug.Log(g.dev);*/

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }


    private void OnApplicationQuit()
    {
        SQLConnection.EndConnection();
    }
}
