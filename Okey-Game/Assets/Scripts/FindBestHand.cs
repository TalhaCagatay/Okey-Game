using System;
using System.Collections.Generic;
using System.Linq;

namespace TÇI
{
    internal struct TileSet
    {
        public readonly List<TileModel> tiles;
        public readonly int jokersCount;

        public readonly int cost;

        public TileSet(List<TileModel> tiles, int jokersCount)
        {
            this.tiles = tiles;
            this.jokersCount = jokersCount;
            this.cost = tiles.Sum(tile => tile.Number) + jokersCount * 30;
        }

    }

    internal class JokerRunData // contained in list and modified from there, so can't be struct
    {
        public readonly int runStart;
        public int runEnd;
        public List<int> jokersUsed;

        public JokerRunData(int runStart)
        {
            this.runStart = runStart;
            this.runEnd = -1;
            this.jokersUsed = new List<int>(2);
        }
    }

    public static class FindBestHand
    {
        public static Player GetBestHand(Player[] players) =>
        players.Select(pl => Tuple.Create(pl, GetScore(pl.Tiles)))
        .Aggregate((t1, t2) => t1.Item2 < t2.Item2 ? t1 : t2).Item1;

        public static int GetScore(List<TileModel> playerTiles)
        {
            var allSets = new List<TileSet>();

            var jokerCount = playerTiles.Count(tile => tile.IsJoker);



            #region Finding "GROUPS"

            {
                var groups = new HashSet<TileColor>[13]; // can't be 7red + 7red + 7black, need 3 differenct colors, so HashSet
                for (int i = 0; i < 13; i++) groups[i] = new HashSet<TileColor>();

                foreach (var tile in playerTiles)
                    if (!tile.IsJoker)
                        groups[tile.Number].Add(tile.Color);

                for (int i = 12; i >= 0; i--) // for in reverse to sort less leter. this way groups of 13 would be before groups of 1, and we need this to spend 13's first
                    if (groups[i].Count >= 3)
                        allSets.Add(new TileSet(
                            groups[i].Select(color => new TileModel(i + 1, color, false)).ToList(),
                            0
                        ));
                    else if (groups[i].Count + jokerCount >= 3) // can 2 jokers be used in 1 set?
                        allSets.Add(new TileSet(
                            groups[i].Select(color => new TileModel(i + 1, color, false)).ToList(),
                            3 - groups[i].Count
                        ));

            }

            #endregion Finding "GROUPS"

            #region Finding "RUNS"

            {
                var coloredTiles = new Dictionary<TileColor, int[]>(4); // Value's are array of counts of tiles of said number and corresponding Key-color
                foreach (TileColor color in Enum.GetValues(typeof(TileColor))) coloredTiles[color] = new int[13];

                foreach (var tile in playerTiles)
                    if (!tile.IsJoker)
                        coloredTiles[tile.Color][tile.Number] += 1;

                foreach (var color in coloredTiles.Keys)
                {
                    var currTiles = coloredTiles[color];

                    var mainRunStart = -1; // -1 if no run is being enumerated and [i] if tile with .Number==i+1 is last in currently enumerated run
                    var jokerRuns = new List<JokerRunData>();

                    for (int i = 12; i >= 0; i--)
                    {

                        if (currTiles[i] != 0)
                        {
                            if (mainRunStart == -1) mainRunStart = i;

                            if (currTiles[i] == 2) // if we have 1,2,3,3,4,5 - we can make 123+345 instead of 12345, where last would clearly cost less
                            {

                                if (i < mainRunStart - 1)
                                {

                                    allSets.Add(new TileSet(
                                        Enumerable.Range(i, mainRunStart - i + 1).Select(num => new TileModel(num + 1, color, false)).ToList(),
                                        0
                                    ));

                                    mainRunStart = i;
                                }

                                for (var jri = 0; jri < jokerRuns.Count; jri++)
                                    if (jokerRuns[jri].runEnd == -1)
                                        if (i < jokerRuns[jri].runStart - 1)
                                            jokerRuns[jri].runEnd = i;

                            }

                        }
                        else
                        {

                            if (mainRunStart != -1)
                            {
                                if (jokerCount != 0)
                                {

                                    var jokerRun = new JokerRunData(mainRunStart);
                                    jokerRun.jokersUsed.Add(i);
                                    jokerRuns.Add(jokerRun);

                                    if (i < 12)
                                    {
                                        jokerRun = new JokerRunData(mainRunStart + 1);
                                        jokerRun.jokersUsed.Add(jokerRun.runStart);
                                        jokerRuns.Add(jokerRun);

                                        if ((i < 11) && (jokerCount == 2))
                                        {
                                            jokerRun = new JokerRunData(mainRunStart + 2);
                                            jokerRun.jokersUsed.Add(jokerRun.runStart);
                                            jokerRun.jokersUsed.Add(jokerRun.runStart - 1);
                                            jokerRuns.Add(jokerRun);
                                        }

                                    }

                                }

                                if (i < mainRunStart - 1)
                                    allSets.Add(new TileSet(
                                        Enumerable.Range(i, mainRunStart - i + 1).Select(num => new TileModel(num + 1, color, false)).ToList(),
                                        0
                                    ));

                                mainRunStart = -1;
                            }

                            for (var jri = 0; jri < jokerRuns.Count; jri++)
                                if (jokerRuns[jri].runEnd == -1)
                                {
                                    if (jokerRuns[jri].jokersUsed.Count < 2)
                                        jokerRuns[jri].jokersUsed.Add(i);
                                    else
                                        jokerRuns[jri].runEnd = i + 1;
                                }

                        }

                    }

                    if (0 < mainRunStart - 1)
                        allSets.Add(new TileSet(
                            Enumerable.Range(0, mainRunStart + 1).Select(num => new TileModel(num + 1, color, false)).ToList(),
                            0
                        ));

                    foreach (var jokerRun in jokerRuns)
                    {
                        allSets.Add(new TileSet(
                            Enumerable.Range(jokerRun.runEnd, jokerRun.runStart - jokerRun.runEnd + 1)
                            .Where(num => !jokerRun.jokersUsed.Contains(num))
                            .Select(num => new TileModel(num + 1, color, false)).ToList(),
                            jokerRun.jokersUsed.Count
                        ));

                        if (jokerRun.jokersUsed.Count == 2)
                        {
                            allSets.Add(new TileSet(
                                Enumerable.Range(jokerRun.jokersUsed[1], jokerRun.runStart - jokerRun.jokersUsed[1] + 1)
                                .Where(num => jokerRun.jokersUsed[0] != num)
                                .Select(num => new TileModel(num + 1, color, false)).ToList(),
                                1
                            ));
                        }

                    }

                }

            }

            #endregion Finding "RUNS"



            {
                var n_allSets = new List<TileSet>(allSets.Count);
                n_allSets.AddRange(allSets.OrderByDescending(ts => ts.cost));
                allSets = n_allSets;
            }

            var n_playerTiles = playerTiles.ToList();

            while (allSets.Any())
            {
                var curr = allSets[0];
                allSets.RemoveAt(0);
                var inds = curr.tiles.ConvertAll(c_tile => n_playerTiles.FindIndex(tile => (tile.Color == c_tile.Color) && (tile.Number == c_tile.Number)));
                if (inds.Any(ind => ind == -1)) continue;

                jokerCount -= curr.jokersCount;

                foreach (var ind in inds.OrderByDescending(ind => ind))
                    n_playerTiles.RemoveAt(ind);

            }

            return n_playerTiles.Sum(tile => tile.Number) + jokerCount * 30;
        }
    }
}
