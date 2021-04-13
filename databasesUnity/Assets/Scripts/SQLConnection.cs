using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;

public class SQLConnection
{
    MySqlConnection connection;

    public void Connect()
    {
        connection = new MySqlConnection("");
        
    }
}
