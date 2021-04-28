using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using MySql.Data.MySqlClient;

public class FrontDesk : MonoBehaviour
{
    // Named this script to remember the fuction of what I want it to do
    // Should deal with all the authentication stuff and parse incoming data into memory
    // Start is called before the first frame update

    InputField shadow;
    InputField prefix;
    InputField nID;
    InputField hostname;

    void Start()
    {
        
    }

    public static MySqlConnection CreateSession(string hostName, string databasePrefix, string netID, string password)
    {
        MySqlConnection ssh = new MySqlConnection();

        try
        {
            string connectString = "Server=" + hostName + "; Database=" + databasePrefix + "; Uid=" + netID + "; Pwd=" + password + ";";
            
            Debug.Log(connectString);
            ssh = new MySqlConnection(connectString);
            ssh.Open();
            Debug.Log("Successfully connected to the database");
            Debug.Log("SSH assigned.");
        }
        catch (MySqlException ex)
        {
            Debug.LogWarning("Unsuccessful connection.");
            Debug.Log("SSH is not modified. The session could not be created.");
            Debug.LogWarning(ex.Message);
        }
        return ssh;
    }

    public void LogOnButton()
    {
        // routine that is called when log in button is pressed
        // initialize the following only temporarily, use only if needed
        string hostName, databasePrefix, netID, password;
        hostName = hostname.text;
        databasePrefix = prefix.text;
        netID = nID.text;
        password = shadow.text;
        
        SQLConnection.connection = CreateSession(hostName, databasePrefix, netID, password);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
