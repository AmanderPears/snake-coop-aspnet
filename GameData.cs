using System.Collections.Generic;
using System;
public class GameData
{
    public static List<string> players = new List<string>();

    public GameData()
    {
        Console.WriteLine(GameData.players.Count);
    }

    public static void RemovePlayer(string id)
    {
        players.Remove(players.Find(p => p == id));
    }
}