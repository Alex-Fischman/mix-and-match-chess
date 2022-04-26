using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class Garage
{
    private static string partSeparator = ",";
    private static string pieceSeparator = ";";
    private static string teamSeparator = ":";

    private static List<int> parts = new List<int>();
    private static List<int[]> pieces = new List<int[]>();
    private static List<List<int[]>> teams = new List<List<int[]>>();

    public static int GetPart(int part)
    {
        return parts[part];
    }

    public static int GetPartCount()
    {
        return parts.Count;
    }

    public static int[] GetPiece(int piece)
    {
        return pieces[piece];
    }

    public static int GetPieceCount()
    {
        return pieces.Count;
    }

    public static List<int[]> GetTeam(int team)
    {
        return teams[team];
    }

    public static int GetTeamCount()
    {
        return teams.Count;
    }

    public static void Save()
    {
        string text = "";
        for (int i = 0; i < parts.Count; ++i)
        {
            text += parts[i].ToString();
            text += partSeparator;
        }
        PlayerPrefs.SetString("Parts", text);
        text = "";
        for (int i = 0; i < pieces.Count; ++i)
        {
            for (int j = 0; j < pieces[i].Length; ++j)
            {
                text += pieces[i][j].ToString();
                text += partSeparator;
            }
            text += pieceSeparator;
        }
        PlayerPrefs.SetString("Pieces", text);
        text = "";
        for (int i = 0; i < teams.Count; ++i)
        {
            for (int j = 0; j < teams[i].Count; ++j)
            {
                for (int k = 0; k < teams[i][j].Length; ++k)
                {
                    text += teams[i][j][k].ToString();
                    if (k < teams[i][j].Length - 1) text += partSeparator;
                }
                if (j < teams[i].Count - 1) text += pieceSeparator;
            }
            if (i < teams.Count - 1) text += teamSeparator;
        }
        PlayerPrefs.SetString("Teams", text);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        parts = new List<int>();
        pieces = new List<int[]>();
        teams = new List<List<int[]>>();

        string[] partStrings = ParseList(PlayerPrefs.GetString("Parts"), partSeparator);
        for (int i = 0; i < partStrings.Length; ++i) parts.Add(int.Parse(partStrings[i]));

        string[] pieceStrings = ParseList(PlayerPrefs.GetString("Pieces"), pieceSeparator);
        List<string[]> piecePartStrings = new List<string[]>();
        for (int i = 0; i < pieceStrings.Length; ++i)
            piecePartStrings.Add(ParseList(pieceStrings[i], partSeparator));
        for (int i = 0; i < piecePartStrings.Count; ++i)
        {
            pieces.Add(new int[3]);
            for (int j = 0; j < 3; ++j)
                pieces[i][j] = int.Parse(piecePartStrings[i][j]);
        }

        string[] teamStrings = 
            ParseList(PlayerPrefs.GetString("Teams"), teamSeparator);
        List<List<string>> teamPieceStrings = new List<List<string>>();
        for (int i = 0; i < teamStrings.Length; ++i)
            teamPieceStrings
                .Add(ArrayToList(ParseList(teamStrings[i], pieceSeparator)));
        List<List<string[]>> teamPiecePartStrings = new List<List<string[]>>();
        for (int i = 0; i < teamPieceStrings.Count; ++i)
        {
            teamPiecePartStrings.Add(new List<string[]>());
            for (int j = 0; j < teamPieceStrings[i].Count; ++j)
            {
                teamPiecePartStrings[i]
                    .Add(ParseList(teamPieceStrings[i][j], partSeparator));
            }
        }
        for (int i = 0; i < teamPiecePartStrings.Count; ++i)
        {
            teams.Add(new List<int[]>());
            for (int j = 0; j < teamPiecePartStrings[i].Count; ++j)
            {
                teams[i].Add(new int[3]);
                for (int k = 0; k < 3; ++k)
                {
                    teams[i][j][k] = int.Parse(teamPiecePartStrings[i][j][k]);
                }
            }
        }
    }

    private static string[] ParseList(string list, string split)
    {
        if (list.Length == 0) return new string[0];
        string[] result = list.Split(new[]{split}, 0);
        if (result[result.Length - 1] == "") result = result.ToList().Take(result.Length - 1).ToArray();
        return result;
    }

    private static List<T> ArrayToList<T>(T[] array)
    {
        List<T> list = new List<T>();
        for (int i = 0; i < array.Length; ++i) list.Add(array[i]);
        return list;
    }

    public static void CreateNewTeam()
    {
        teams.Add(new List<int[]>());
    }

    public static void AddToTeam(int piece, int team)
    {
        int[] seeds = pieces[piece];
        pieces.RemoveAt(piece);
        teams[team].Add(seeds);
    }

    public static void RemoveFromTeam(int team, int unit)
    {
        int[] seeds = teams[team][unit];
        teams[team].RemoveAt(unit);
        pieces.Add(seeds);
    }

    public static void CreateRandomPiece()
    {
        int[] seeds = new int[3];
        for (int i = 0; i < 3; ++i) seeds[i] = 
            Random.Range(0, (int)(int.MaxValue / 3 - 1)) * 3 + i;
        pieces.Add(seeds);
    }

    public static void CreateRandomPart()
    {
        parts.Add(Random.Range(0, int.MaxValue));
    }

    public static void SplitPiece(int piece)
    {
        int[] seeds = pieces[piece];
        pieces.RemoveAt(piece);
        parts.AddRange(seeds);
    }

    public static void CombineParts(int[] seeds) {
        pieces.Add(seeds);
        for (int i = 0; i < 3; ++i) parts.Remove(seeds[i]);
    }
}
