using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            this.cost = tiles.Sum(tile => tile.Number + 1) + jokersCount * 30;
        }

        public override string ToString()
        {
            var res = new StringBuilder();

            foreach (var tile in tiles)
            {
                res.Append(tile.Color.ToString());
                res.Append(tile.Number);
                res.Append(" + ");
            }

            res.Append(jokersCount);
            res.Append("*jokers = ");
            res.Append(cost);

            return res.ToString();
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

    /// <summary>
    /// this class is for checking series and groups in players hand and finding the best one
    /// </summary>
    public static class FindBestHand
    {
        public static Player GetBestHand(Player[] players) =>
        players.Select(pl => Tuple.Create(pl, GetScore(pl.Tiles)))
        .Aggregate((t1, t2) => t1.Item2 < t2.Item2 ? t1 : t2).Item1;

        public static int GetScore(List<TileModel> playerTiles)
        {
            var allSets = new List<TileSet>();
            var n_playerTiles = playerTiles.ToList();
            var jokerCount = n_playerTiles.RemoveAll(tile => tile.IsJoker);



            #region Finding "GROUPS"

            {
                var groups = new HashSet<TileColor>[13]; // can't be 7red + 7red + 7black, need 3 differenct colors, so HashSet
                for (int i = 0; i < 13; i++) groups[i] = new HashSet<TileColor>();

                foreach (var tile in n_playerTiles)
                    groups[tile.Number].Add(tile.Color);

                // for in reverse to sort faster at the end
                // this way groups of 13 would be before groups of 1, and we need this to spend 13's first
                for (int i = 12; i >= 0; i--)
                {

                    // non-joker group is posible if we have at least 3 different colors of tile [i]
                    if (groups[i].Count >= 3)
                        allSets.Add(new TileSet(
                            groups[i].Select(color => new TileModel(i, color, false)).ToList(),
                            0
                        ));

                    // joker group is posible if both
                    // 1. we can get a group of at least 3 if we combine all tiles and jokers
                    // 2. we have a place for joker (we don't already have all 4 colors)
                    if ((groups[i].Count < 4) && (groups[i].Count + jokerCount >= 3))
                    {
                        var usedJokersCount = 4 - groups[i].Count;
                        if (usedJokersCount > jokerCount) usedJokersCount = jokerCount;

                        allSets.Add(new TileSet(
                            groups[i].Select(color => new TileModel(i, color, false)).ToList(),
                            usedJokersCount
                        ));

                    }

                }

            }

            #endregion Finding "GROUPS"

            #region Finding "RUNS"

            {
                var coloredTiles = new Dictionary<TileColor, int[]>(4); // Value's are array of counts of tiles of said number and corresponding Key-color
                foreach (TileColor color in Enum.GetValues(typeof(TileColor))) coloredTiles[color] = new int[13];

                foreach (var tile in n_playerTiles)
                    coloredTiles[tile.Color][tile.Number] += 1;

                // all tiles are split in batches by color and computed intependently,
                // because then we don't need to check it to be sure that all tiles in run are same color
                foreach (var color in coloredTiles.Keys)
                {
                    var currTiles = coloredTiles[color];

                    // -1 if no run is being enumerated
                    // [i] if tile with .Number==i is last in currently enumerated run without jokers
                    var mainRunStart = -1;
                    // if we have 12234 - we first go thrue 432 and see that we can split
                    // but then - 12 is not a valid set, so split need's to be reverted
                    // when no split was made in current run - this var == -1
                    // when split was made - this var is index in allSets
                    var mainRunLastRestart = -1;

                    // if we have 123.56.7 - we need to check posible runs 123j56 and 123j56j7
                    // so multiple joker runs can overlap => we need a list
                    // JokerRunData is just a container type
                    var jokerRuns = new List<JokerRunData>();

                    // we enumearate in revers for the same reason as with groups - later sort is faster
                    for (int i = 12; i >= 0; i--)
                    {

                        if (currTiles[i] != 0) // if we have any tiles with number [i] in currently enumerated color batch
                        {
                            if (mainRunStart == -1) mainRunStart = i;

                            if (currTiles[i] == 2) // if we have 1,2,3,3,4,5 - we can make 123+345 instead of 12345, where last would clearly cost less
                            {

                                // only if run has already 3 or more tiles - it can be split here and added to valid set's list
                                if (i < mainRunStart - 1)
                                {
                                    mainRunLastRestart = allSets.Count;

                                    allSets.Add(new TileSet(
                                        Enumerable.Range(i, mainRunStart - i + 1).Select(num => new TileModel(num, color, false)).ToList(),
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
                        else // if (currTiles[i] != 0)
                        {

                            // needs to be calculated before mainRunStart
                            // because otherwise it would add unnecessary jokersUsed values copies
                            for (var jri = 0; jri < jokerRuns.Count; jri++)
                                if (jokerRuns[jri].runEnd == -1)
                                {
                                    if (jokerRuns[jri].jokersUsed.Count < 2)
                                        jokerRuns[jri].jokersUsed.Add(i);
                                    else
                                        jokerRuns[jri].runEnd = i + 1;
                                }

                            // if non-joker run was enumerated just before currTiles[i]
                            if (mainRunStart != -1)
                            {
                                // add posible joker runs
                                if (jokerCount != 0)
                                {

                                    // can be *j45
                                    // jj45 also starts here
                                    // no test for having at least 2 tiles, because we can have jj5, which is valid
                                    {
                                        var jokerRun = new JokerRunData(mainRunStart);
                                        jokerRun.jokersUsed.Add(i);
                                        jokerRuns.Add(jokerRun);
                                    }

                                    if (jokerCount == 2)
                                    {

                                        // can be j5j
                                        if (mainRunStart < 12)
                                        {
                                            var jokerRun = new JokerRunData(mainRunStart + 1);
                                            jokerRun.jokersUsed.Add(jokerRun.runStart);
                                            jokerRun.jokersUsed.Add(i);
                                            jokerRuns.Add(jokerRun);
                                        }

                                        // can be 5jj
                                        if (mainRunStart < 11)
                                        {
                                            var jokerRun = new JokerRunData(mainRunStart + 2);
                                            jokerRun.jokersUsed.Add(jokerRun.runStart);
                                            jokerRun.jokersUsed.Add(jokerRun.runStart - 1);
                                            jokerRun.runEnd = i + 1; // but because we know currTiles[i]==0 - this is already end of this run
                                            jokerRuns.Add(jokerRun);
                                        }

                                    }

                                }

                                // if non-joker run has at least 3 tiles - add it
                                if (i < mainRunStart - 2)
                                    allSets.Add(new TileSet(
                                        Enumerable.Range(i + 1, mainRunStart - i).Select(num => new TileModel(num, color, false)).ToList(),
                                        0
                                    ));
                                // if non-joker run is to short, but there was a split - edit last run's content
                                else if (mainRunLastRestart != -1)
                                    allSets[mainRunLastRestart] = new TileSet(
                                        Enumerable.Range(i, allSets[mainRunLastRestart].tiles.Last().Number - i + 1).Select(num => new TileModel(num, color, false)).ToList(),
                                        0
                                    );

                                // reset non-joker run info
                                mainRunStart = -1;
                                mainRunLastRestart = -1;
                            }

                        }

                    } // for (int i = 12; i >= 0; i--)

                    // if non-joker run was enumerated at the end and it has at least 3 tile - add to valid set's
                    if (mainRunStart > 1)
                        allSets.Add(new TileSet(
                            Enumerable.Range(0, mainRunStart + 1).Select(num => new TileModel(num, color, false)).ToList(),
                            0
                        ));
                    // if non-joker run is to short, but there was a split - edit last run's content
                    else if (mainRunLastRestart != -1)
                        allSets[mainRunLastRestart] = new TileSet(
                            Enumerable.Range(0, allSets[mainRunLastRestart].tiles.Last().Number + 1).Select(num => new TileModel(num, color, false)).ToList(),
                            0
                        );

                    // add all found joker-containing runs, if they are valid
                    foreach (var jokerRun in jokerRuns)
                    {
                        if (jokerRun.runEnd == -1) jokerRun.runEnd = 0; // if this run was still enumerated - seal it
                        if (jokerRun.runStart - jokerRun.runEnd < 2) continue; // it run is too short - skip as invalid

                        // everything is tested, now we can just add to valid set's
                        allSets.Add(new TileSet(
                            Enumerable.Range(jokerRun.runEnd, jokerRun.runStart - jokerRun.runEnd + 1)
                            .Where(num => !jokerRun.jokersUsed.Contains(num))
                            .Select(num => new TileModel(num, color, false)).ToList(),
                            jokerRun.jokersUsed.Count
                        ));

                    }

                }

            }

            #endregion Finding "RUNS"



            // need to sort all valid set's by descending of cost, so that we would first spend more heavy set's
            {
                var n_allSets = new List<TileSet>(allSets.Count);
                n_allSets.AddRange(allSets.OrderByDescending(ts => ts.cost));
                allSets = n_allSets;
            }

            while (allSets.Any())
            {
                // take out heaviest set there is right now
                var curr = allSets[0];
                allSets.RemoveAt(0);

                // maybe jokers were spend by prev run and so we don't have enough now - skip this run then
                if (curr.jokersCount > jokerCount) continue;

                // maybe some tiles used by this run were spend by prev - skip this run then
                var inds = curr.tiles.ConvertAll(c_tile => n_playerTiles.FindIndex(tile => (tile.Color == c_tile.Color) && (tile.Number == c_tile.Number)));
                if (inds.Any(ind => ind == -1)) continue;

                jokerCount -= curr.jokersCount;

                foreach (var ind in inds.OrderByDescending(ind => ind))
                    n_playerTiles.RemoveAt(ind);

            }

            // result is sum of numbers on all tiles, that wasn't spend by any valid set's
            // and + almount of jokers that wasn't spend * it's cost, which is 30
            return n_playerTiles.Sum(tile => tile.Number + 1) + jokerCount * 30;
        }
    }
}
