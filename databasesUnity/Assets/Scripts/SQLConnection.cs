using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;

public static class SQLConnection
{
	/*
	 * References:
	 *	Games database link:		https://www.kaggle.com/ashaheedq/video-games-sales-2019 
	 *	Developers database link:	https://www.kaggle.com/andreshg/videogamescompaniesregions 
	 */

	public static MySqlConnection connection = new MySqlConnection(); // start with a new until a valid one is created.
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
			//Debug.Log(connectString);
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

	public static void Connect(string dp, string nid, string hn, string pswd)
	{
		// This method is used by LogOn scene
		// This method creates the packet string using string builder
		var con = new MySqlConnection();

		try
		{
			MySqlConnectionStringBuilder strb = new MySqlConnectionStringBuilder();
			
			strb.CharacterSet = "latin1";
			//strb.CertificatePassword = pswd;
			strb.UserID = nid;
			strb.Password = pswd;
			strb.Database = dp;
			strb.Server = hn;
			//strb.Port = 22; breaks the connection
			string cs = strb.GetConnectionString(true);
			Debug.Log(cs);
			con = new MySqlConnection(cs);
			con.Open();
			Debug.Log("Successfully connected to the database");
			// update class connection instance
			connection = con;
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning("Unsuccessful connection.");
			Debug.LogWarning(ex.Message);
		}
				
		//return con;
	}


	public static GameData GetAllGameData(int id)
    {
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
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "allGameDataByRank";

			gameCall.Parameters.AddWithValue("@gameRank", id);
			gameCall.Parameters["@gameRank"].Direction = ParameterDirection.Input;

			Debug.Log("Getting game data of rank " + id);

			// Loading Game Data:
			MySqlDataReader reader = gameCall.ExecuteReader();

			while (reader.Read())	// this should only run once because of only having one row
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
					gameData.dev = reader[18].ToString();
					gameData.pub = reader[9].ToString();

					if (!float.TryParse(reader[10].ToString(), out gameData.ratingCritic)) gameData.ratingCritic = -1;
					if (!float.TryParse(reader[11].ToString(), out gameData.ratingUser)) gameData.ratingUser = -1;

					if (!float.TryParse(reader[13].ToString(), out gameData.salesGlobal)) gameData.salesGlobal = -1;
					if(gameData.salesGlobal > 0)
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
						gameData.salesNA = gameData.salesPAL = gameData.salesJP  = gameData.salesOther = -1;
					}
				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}
			}

			reader.Close();
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.Log("Finished single game query.");
		return gameData;
    }

	public static GameData GetMinorGameData(int id)
    {
		GameData gameData = new GameData();

		if (id < 0 || id > 55793)
		{
			gameData.rank = -1;
			return gameData;
		}

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
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
					gameData.urlImg = reader[7].ToString();
				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}
			}

			reader.Close();
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.Log("Finished single simple game query.");
		return gameData;
	}

	public static DevData GetAllDevData(int id)
    {
		DevData devData = new DevData();

		if (id < 0 || id > 686)
		{
			devData.id = -1;
			return devData;
		}

		try
		{
			MySqlCommand devCall = new MySqlCommand();
			devCall.Connection = connection;
			devCall.CommandType = CommandType.StoredProcedure;
			devCall.CommandText = "allDevDataByDevID";

			devCall.Parameters.AddWithValue("@devID", id);
			devCall.Parameters["@devID"].Direction = ParameterDirection.Input;

			Debug.Log("Getting dev data of id " + id);

			// Loading Dev Data:
			MySqlDataReader reader = devCall.ExecuteReader();

			while (reader.Read())   // this should only run once because of only having one row
			{
				try
				{
					// I don't think a loop is needed
					if (!int.TryParse(reader[0].ToString(), out devData.id)) devData.id = -1;
					devData.name = reader[1].ToString();
					devData.city = reader[2].ToString();
					devData.country = reader[3].ToString();
					if (!int.TryParse(reader[4].ToString(), out devData.established)) devData.established = -1;

					devData.note = reader[5].ToString();
				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}
			}

			reader.Close();
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.Log("Finished single dev query.");
		return devData;
	}

	public static DevData TopThreeGamesFromDev(DevData dev)
    {
		//gets the top 3 game ranks with matching devID, -1 if none
		try
		{
			MySqlCommand gamesListCall = new MySqlCommand();
			gamesListCall.Connection = connection;
			gamesListCall.CommandType = CommandType.StoredProcedure;
			gamesListCall.CommandText = "topThreeGamesFromDev";

			gamesListCall.Parameters.AddWithValue("@devID", dev.id);
			gamesListCall.Parameters["@devID"].Direction = ParameterDirection.Input;

			MySqlDataReader reader = gamesListCall.ExecuteReader();
			int currentGame = 0;

			dev.notableGames = new string[3];
			dev.notableGameImgURLs = new string[3];
			dev.notableGameRanks = new int[3] {-1, -1, -1 };

			while (reader.Read())
            {
				if (currentGame > 2) 
				{
					Debug.Log("I have run more than 3 times...");
					break;
				}
				else
				{
					if (int.TryParse(reader[0].ToString(), out dev.notableGameRanks[currentGame]))
					{
						dev.notableGames[currentGame] = reader[1].ToString();
						dev.notableGameImgURLs[currentGame] = reader[2].ToString();
                    }

				}

				currentGame++;
			}

			reader.Close();
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.Log("Loaded top 3 games into dev.");
		return dev;
    }

	public static int GamesCountFromDev(int devID)
    {
		int count = -1;

		try
		{
			MySqlCommand gamesCountCall = new MySqlCommand();
			gamesCountCall.Connection = connection;
			gamesCountCall.CommandType = CommandType.StoredProcedure;
			gamesCountCall.CommandText = "gamesCountFromDev";

			gamesCountCall.Parameters.AddWithValue("@devID", devID);
			gamesCountCall.Parameters["@devID"].Direction = ParameterDirection.InputOutput;

			gamesCountCall.ExecuteNonQuery();

			Debug.Log("Total Games from id " + devID + ": " + gamesCountCall.Parameters["@devID"].Value);
			int.TryParse(gamesCountCall.Parameters["@devID"].Value.ToString(), out count);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.Log("Found games count from dev id " + devID);
		return count;
	}



    #region Search Methods

	// Game Searches
    public static List<GameData> GamesNameSearch(string search)
    {
		// procedure: searchGameName(IN gameName VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for game name using: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGameName";

			gameCall.Parameters.AddWithValue("@gameName", search);
			gameCall.Parameters["@gameName"].Direction = ParameterDirection.Input;

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
    }
	public static List<GameData> GamesPubSearch(string search)
	{
		// procedure: searchGamePub(IN gamePub VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for game publisher using: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGamePub";

			gameCall.Parameters.AddWithValue("@gamePub", search);
			gameCall.Parameters["@gamePub"].Direction = ParameterDirection.Input;

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesPlatSearch(string search)
	{
		// procedure: searchGamePlat(IN gamePlat VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for game platform using: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGamePlat";

			gameCall.Parameters.AddWithValue("@gamePlat", search);
			gameCall.Parameters["@gamePlat"].Direction = ParameterDirection.Input;

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesGenreSearch(string search)
	{
		// procedure: searchGameGenre(IN gameGenre VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for game genre using: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGameGenre";

			gameCall.Parameters.AddWithValue("@gameGenre", search);
			gameCall.Parameters["@gameGenre"].Direction = ParameterDirection.Input;

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesDevSearch(string search)
	{
		// procedure: searchGameDev(IN gameDev VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for game dev: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGameDev";

			gameCall.Parameters.AddWithValue("@gameDev", search);
			gameCall.Parameters["@gameDev"].Direction = ParameterDirection.Input;

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}

	static List<GameData> GamesSearchData(MySqlCommand command)		// gives games in: { rank, game_name, platform }
    {
		List<GameData> dataList = new List<GameData>();
		int dataLimiter = 1;

		MySqlDataReader reader = command.ExecuteReader();
		while (reader.Read())   // this should only run once because of only having one row
		{
			if(dataLimiter > 1000)
            {
				Debug.LogWarning("I have loaded 1000 entries, stopping here.");
				break;
            }

			try
			{
				GameData game = new GameData();

				if (!int.TryParse(reader[0].ToString(), out game.rank)) game.rank = -1;
				game.name = reader[1].ToString();
				game.platform = reader[2].ToString();

				dataList.Add(game);
				
			}
			catch (MySqlException ex)
			{
				Debug.LogWarning(ex.ToString());
			}

			dataLimiter++;
		}

		reader.Close();

		Debug.Log("Search Completed. " + dataList.Count + " results found.");
		return dataList;
	}

	// Dev Searches
	public static List<DevData> DevsNameSearch(string search)
	{
		// procedure: searchDevName(IN devName VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for dev by name: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchDevName";

			gameCall.Parameters.AddWithValue("@devName", search);
			gameCall.Parameters["@devName"].Direction = ParameterDirection.Input;

			return DevsSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<DevData> DevsCitySearch(string search)
	{
		// procedure: searchDevCity(IN devCity VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for dev by city: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchDevCity";

			gameCall.Parameters.AddWithValue("@devCity", search);
			gameCall.Parameters["@devCity"].Direction = ParameterDirection.Input;

			return DevsSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<DevData> DevsCountrySearch(string search)
	{
		// procedure: searchDevCountry(IN devCoun VARCHAR(255))

		// adjust search string:
		if (search.Length > 253) search = search.Substring(0, 253);
		search = "%" + search + "%";
		Debug.Log("Searching for dev by country: " + search);

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchDevCountry";

			gameCall.Parameters.AddWithValue("@devCoun", search);
			gameCall.Parameters["@devCoun"].Direction = ParameterDirection.Input;

			return DevsSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}

	static List<DevData> DevsSearchData(MySqlCommand command)     // gives games in: { dev_id, dev_name }
	{
		List<DevData> dataList = new List<DevData>();
		int dataLimiter = 1;

		MySqlDataReader reader = command.ExecuteReader();
		while (reader.Read())   // this should only run once because of only having one row
		{
			if (dataLimiter > 1000)
			{
				Debug.LogWarning("I have loaded 1000 entries, stopping here.");
				break;
			}

			try
			{
				DevData dev = new DevData();

				if (!int.TryParse(reader[0].ToString(), out dev.id)) dev.id = -1;
				dev.name = reader[1].ToString();
				dev.gamesCount = -1;

				dataList.Add(dev);

			}
			catch (MySqlException ex)
			{
				Debug.LogWarning(ex.ToString());
			}

			dataLimiter++;
		}

		reader.Close();

		Debug.Log("Search Completed. " + dataList.Count + " results found.");
		return dataList;
	}


	// Custom Searches
	public static List<GameData> GamesJPSearch()
    {
		// procedure: searchGamesBestJP()
		Debug.Log("Searching for most sold in JP...");

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGamesBestJP";

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesCriticSearch()
	{
		// procedure: searchGamesBestCritic()
		Debug.Log("Searching for best critic-reviewed games...");

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGamesBestCritic";

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesUserSearch()
	{
		// procedure: searchGamesBestUser()
		Debug.Log("Searching for best user-reviewed games...");

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGamesBestUser";

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesOlderSearch()
	{
		// procedure: searchGameOlder()
		Debug.Log("Searching for games created before 2000...");

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGameOlder";

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<GameData> GamesNewerSearch()
	{
		// procedure: searchGameNewer()
		Debug.Log("Searching for games created after 2015...");

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchGameNewer";

			return GamesSearchData(gameCall);
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<DevData> DevsMostSearch()
	{
		// procedure: searchDevMostGames()
		Debug.Log("Searching for most apparent developers...");

		try
		{
			MySqlCommand gameCall = new MySqlCommand();
			gameCall.Connection = connection;
			gameCall.CommandType = CommandType.StoredProcedure;
			gameCall.CommandText = "searchDevMostGames";

			List<DevData> dataList = new List<DevData>();
			int dataLimiter = 1;

			MySqlDataReader reader = gameCall.ExecuteReader();
			while (reader.Read())   // this should only run once because of only having one row
			{
				if (dataLimiter > 1000)
				{
					Debug.LogWarning("I have loaded 1000 entries, stopping here.");
					break;
				}

				try
				{
					DevData dev = new DevData();

					if (!int.TryParse(reader[0].ToString(), out dev.id)) dev.id = -1;
					dev.name = reader[1].ToString();
					if (!int.TryParse(reader[2].ToString(), out dev.gamesCount)) dev.gamesCount = -1;

					dataList.Add(dev);

				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}

				dataLimiter++;
			}

			reader.Close();

			Debug.Log("Search Completed. " + dataList.Count + " results found.");
			return dataList;
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}

	public static List<(string, string)> GenreSearch()
	{
		// procedure: searchGenreMost()
		Debug.Log("Searching for most apparent genres...");

		try
		{
			MySqlCommand genreCall = new MySqlCommand();
			genreCall.Connection = connection;
			genreCall.CommandType = CommandType.StoredProcedure;
			genreCall.CommandText = "searchGenreMost";

			List<(string, string)> dataList = new List<(string, string)>();
			int dataLimiter = 1;

			MySqlDataReader reader = genreCall.ExecuteReader();
			while (reader.Read())   // this should only run once because of only having one row
			{
				if (dataLimiter > 1000)
				{
					Debug.LogWarning("I have loaded 1000 entries, stopping here.");
					break;
				}

				try
				{
					(string, string) genreData = (reader[0].ToString(), reader[1].ToString());
					dataList.Add(genreData);
				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}

				dataLimiter++;
			}

			reader.Close();

			Debug.Log("Search Completed. " + dataList.Count + " results found.");
			return dataList;
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}
	public static List<(string, string)> PlatformSearch()
	{
		// procedure: searchPlatMost()
		Debug.Log("Searching for most apparent platforms...");

		try
		{
			MySqlCommand genreCall = new MySqlCommand();
			genreCall.Connection = connection;
			genreCall.CommandType = CommandType.StoredProcedure;
			genreCall.CommandText = "searchPlatMost";

			List<(string, string)> dataList = new List<(string, string)>();
			int dataLimiter = 1;

			MySqlDataReader reader = genreCall.ExecuteReader();
			while (reader.Read())   // this should only run once because of only having one row
			{
				if (dataLimiter > 1000)
				{
					Debug.LogWarning("I have loaded 1000 entries, stopping here.");
					break;
				}

				try
				{
					(string, string) genreData = (reader[0].ToString(), reader[1].ToString());
					dataList.Add(genreData);
				}
				catch (MySqlException ex)
				{
					Debug.LogWarning(ex.ToString());
				}

				dataLimiter++;
			}

			reader.Close();

			Debug.Log("Search Completed. " + dataList.Count + " results found.");
			return dataList;
		}
		catch (MySqlException ex)
		{
			Debug.LogWarning(ex.ToString());
		}

		Debug.LogWarning("Query error.");
		return null;
	}

	#endregion

	public static void EndConnection()
    {
		connection?.Close();
		Debug.Log("Closed connection.");
	}
}
