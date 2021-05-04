using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class FrontDesk : MonoBehaviour
{
    // Named this script to remember the fuction of what I want it to do -N
    // Should deal with all the authentication stuff and parse incoming data into memory
    // Start is called before the first frame update
    // Uses SQLConnection to create and store a session based on the credentials entered by the user

    [SerializeField] InputField shadow;
    [SerializeField] InputField prefix;
    [SerializeField] InputField nID;
    [SerializeField] InputField hostname;
    [SerializeField] Button status;
    [SerializeField] Text text;
    protected Credentials mycred = null;
    internal static DevData ddat;

    //MySqlConnection userSession;


    void Awake()
    {
        //userSession = new MySqlConnection();
        InvokeRepeating("Main", 0f, 0.42f);
        // Begin checking for saved sucessful log-on
        try
        {
            // if temp.t does not exist, should jump to exception
            FileStream fs = new FileStream("Temp/Bin/temp.t", FileMode.Open);
            BinaryFormatter fm = new BinaryFormatter();
            mycred =  (Credentials) fm.Deserialize(fs); // Serialize the credentials file, not sure this is secure but it works for now
            //hostname.text = mycred.hostname;
        }
        catch(Exception e)
        {
            Debug.LogError("The kind people at the front desk notice there isn't a saved profile.");
            e.ToString();
        }
    }

    public void LogOnButton()
    {
        // routine that is called when log in button is pressed
        // initialize the following only temporarily, use only if needed
        Debug.Log("Attempting sign in on a connection in state: "+SQLConnection.connection.State);
        string hostName, databasePrefix, netID, password;
        hostName = hostname.text.Trim();
        databasePrefix = prefix.text.Trim();
        netID = nID.text.Trim();
        password = shadow.text.Trim();
        Credentials creds = new Credentials(netID, hostName, databasePrefix, password);
        
        mycred = creds; // set front desk local
        SQLConnection.Connect(databasePrefix, netID, hostName, password);
        Debug.Log("After attempt state: "+ SQLConnection.connection.State);
        // If the log-on was a sucess, state should be open.

        if (SQLConnection.connection.State == ConnectionState.Open )
        {
            FileStream fs = new FileStream("Temp/Bin/temp.t", FileMode.Create);
            BinaryFormatter fm = new BinaryFormatter();
            fm.Serialize(fs, creds); // Serialize the credentials file, not sure this is secure but it works for now
            Debug.Log("The kind people at the front desk have filed these credentials @ */Temp/Bin/temp.t");
        }

        //userSession = SQLConnection.connection;
    }
    public void Main()
    {
        //Debug.Log(userSession.ConnectionString);
        // Updates status indicator.
        if (mycred != null)
        {
            Debug.Log("credentials are not null!");
            Debug.Log(mycred.Check());
            shadow.text = mycred.Export();
            prefix.text = mycred.database;
            nID.text = mycred.userID;

        } else
        {
            //Debug.Log(mycred.Check());
            Debug.Log("Credentails currently invalid.");
        }
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

    [Serializable]
    public class Credentials
    {
        protected static string pswd;
        public string hostname;
        public string database;
        public string userID;

         public Credentials(string a, string b, string c, string d)
        {
            a = userID;
            b = hostname;
            c = database;
            d = pswd;

            
        }

        public bool IsFull()
        {

                if (pswd == "")
                    return false;
                else if (hostname == "")
                    return false;
                else if (database == "")
                    return false;
                else if (userID == "")
                    return false;

                // at this point in the code no values are empty.
                return true;
        }

        public string Export()
        {
            return pswd;
        }

        public string Check()
        {
            return "a: " + userID + "|b: " + hostname + "|c: " + database + "|d:" + Export();
        }
    }
}
