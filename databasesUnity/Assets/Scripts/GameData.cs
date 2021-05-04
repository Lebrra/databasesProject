using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Struct for holding all the data of a game from query
/// </summary>
[System.Serializable]
public struct GameData
{
    public string name;
    public int rank;

    public string platform;
    public string genre;
    public string esrb;
    public int year;

    public string url;
    public string urlImg;

    public int devID;
    public string dev;
    public string pub;

    public float ratingCritic;
    public float ratingUser;

    public float salesGlobal;
    public float salesNA;
    public float salesPAL;
    public float salesJP;
    public float salesOther;
}

/// <summary>
/// Struct for holding all the data of a developer from query
/// </summary>
[System.Serializable]
public struct DevData
{
    public string name;
    public int id;

    public string city;
    public string country;

    public int established;
    public string note;

    public int gamesCount;

    public int[] notableGameRanks;
    public string[] notableGames;
    public string[] notableGameImgURLs;

    public override string ToString()
    {
        return string.Format("name={0} id={1} city={2} country={3} estab={4} note={5} gcount={6}", name, id, city, country, established, note, gamesCount);
    }
}