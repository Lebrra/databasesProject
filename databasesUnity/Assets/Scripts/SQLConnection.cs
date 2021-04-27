using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;

public static class SQLConnection
{
    static MySqlConnection connection = null;

	static string databasePrefix = "cs366-2211_blasczyklm05";
	static string netID = "blasczyklm05";
	static string hostName = "washington.uww.edu";
	static string password = "lb1574";

	public static void Connect()
    {
		try
		{
			string connectString = "Server=" + hostName + "; Database=" + databasePrefix + "; Uid=" + netID + "; Pwd=" + password + ";";
			//string connectString = "Server=" + hostName + "; Database=" + databasePrefix + "; Uid=" + netID + "; Pwd=" + password + "; CharSet=utf8;";
			Debug.Log(connectString);
			connection = new MySqlConnection(connectString);
			connection.Open();
			Debug.Log("Successfully connected to the database");
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning("Unsuccessful connection.");
			Debug.LogWarning(ex.Message);
		}
	}

	public static void testSQL()
	{
		try
		{
			MySqlCommand facultyCountCall = new MySqlCommand();
			facultyCountCall.Connection = connection;
			facultyCountCall.CommandType = CommandType.StoredProcedure;
			facultyCountCall.CommandText = "getTotalFaculty";

			facultyCountCall.Parameters.Add("@total", MySqlDbType.Int32);
			facultyCountCall.Parameters["@total"].Direction = ParameterDirection.InputOutput;

			facultyCountCall.ExecuteNonQuery();

			Debug.Log("Total Faculty: " + facultyCountCall.Parameters["@total"].Value);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}
	}

	public static void EndConnection()
    {
		connection?.Close();
		Debug.Log("Closed connection.");
	}
}
