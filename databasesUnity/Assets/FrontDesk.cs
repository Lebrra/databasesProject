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

    [SerializeField] InputField shadow;
    [SerializeField] InputField prefix;
    [SerializeField] InputField nID;
    [SerializeField] InputField hostname;
    MySqlConnection userSession;

    void Start()
    {
        userSession = new MySqlConnection();
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
        
        userSession = SQLConnection.Connect(databasePrefix, netID, hostName, password);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
