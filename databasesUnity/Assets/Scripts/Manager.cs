using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
		
        //Invoke("testGame", 3);
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

	public static GameData GetAllGameData(int id)
	{
		//Copied ad pasted from other script.
		//Maybe make the manager able to perform all stored procedures? 
		//Should there be a seperate output screen for the results?
		GameData gameData = new GameData();

		if (id < 0 || id > 55793)
		{
			gameData.rank = -1;
			return gameData;
		}

		//allGameDataByRank(gameRank)
		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = SQLConnection.connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "allGameDataByRank";

			gameCall.Parameters.AddWithValue("@gameRank", id);
			gameCall.Parameters["@gameRank"].Direction = ParameterDirection.Input;

			Debug.Log("Getting game data of rank " + id);

			// Loading Game Data:
			MySqlDataReader reader = gameCall.ExecuteReader();

			while (reader.Read())   // this should only run once because of only having one row
			{
				try
				{
					// I don't think a loop is needed
					if (!int.TryParse(reader[0].ToString(), out gameData.rank)) gameData.rank = -1;
					gameData.name = reader[1].ToString();
					gameData.platform = reader[2].ToString();
					gameData.genre = reader[3].ToString();
					gameData.esrb = reader[4].ToString();
					if (!int.TryParse(reader[5].ToString(), out gameData.year)) gameData.year = -1;

					gameData.url = reader[6].ToString();
					gameData.urlImg = reader[7].ToString();

					if (!int.TryParse(reader[8].ToString(), out gameData.devID)) gameData.devID = -1;
					gameData.pub = reader[9].ToString();

					if (!float.TryParse(reader[10].ToString(), out gameData.ratingCritic)) gameData.ratingCritic = -1;
					if (!float.TryParse(reader[11].ToString(), out gameData.ratingUser)) gameData.ratingUser = -1;

					if (!float.TryParse(reader[13].ToString(), out gameData.salesGlobal)) gameData.salesGlobal = -1;
					if (gameData.salesGlobal > 0)
					{
						// spread data exists, load it
						if (!float.TryParse(reader[14].ToString(), out gameData.salesNA)) gameData.salesNA = -1;
						if (!float.TryParse(reader[15].ToString(), out gameData.salesPAL)) gameData.salesPAL = -1;
						if (!float.TryParse(reader[16].ToString(), out gameData.salesJP)) gameData.salesJP = -1;
						if (!float.TryParse(reader[17].ToString(), out gameData.salesOther)) gameData.salesOther = -1;
					}
					else
					{
						// only total shipped exists, put that in for global
						if (!float.TryParse(reader[12].ToString(), out gameData.salesGlobal)) gameData.salesGlobal = -1;
						gameData.salesNA = gameData.salesPAL = gameData.salesJP = gameData.salesOther = -1;
					}
				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}
			}

			reader.Close();

			// check for dev
			if (gameData.devID != -1)
			{
				// do another query getting the dev name
				gameData.dev = SQLConnection.GetAllDevData(gameData.devID).name;
			}
			else gameData.dev = "---";
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.Log("Finished single game query.");
		return gameData;
	}


	private void OnApplicationQuit()
    {
        SQLConnection.EndConnection();
    }
}
