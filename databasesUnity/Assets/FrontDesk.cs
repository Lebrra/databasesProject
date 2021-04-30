using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using MySql.Data.MySqlClient;

public class FrontDesk : MonoBehaviour
{
    // Named this script to remember the fuction of what I want it to do
    // Should deal with all the authentication stuff and parse incoming data into memory
    // Start is called before the first frame update
    // Uses SQLConnection to create and store a session based on the credentials entered by the user

    [SerializeField] InputField shadow;
    [SerializeField] InputField prefix;
    [SerializeField] InputField nID;
    [SerializeField] InputField hostname;
    [SerializeField] Button status;
    [SerializeField] Text text;
    //MySqlConnection userSession;
    

    void Start()
    {
        //userSession = new MySqlConnection();
        InvokeRepeating("Main", 0f, 0.42f);
    }


    public void LogOnButton()
    {
        // routine that is called when log in button is pressed
        // initialize the following only temporarily, use only if needed
        string hostName, databasePrefix, netID, password;
        hostName = hostname.text.Trim();
        databasePrefix = prefix.text.Trim();
        netID = nID.text.Trim();
        password = shadow.text.Trim();
        
        SQLConnection.Connect(databasePrefix, netID, hostName, password);
        //userSession = SQLConnection.connection;
    }
    public void Main()
    {
        //Debug.Log(userSession.ConnectionString);
        // Updates status indicator.
        if (SQLConnection.connection.ConnectionString.Equals(""))
        {
            text.color = Color.red;
            text.text = "Not Connected.";
        }
        else
        {
            text.color = Color.green;
            text.text = "Connected.";
            UnityEngine.SceneManagement.SceneManager.LoadScene(1, LoadSceneMode.Single);
        }
    }
}
