using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// this class is for checking series and groups in players hand and finding the best one
    /// I haven't got enough time to make this algorithm better but this will work.
    /// 
    /// /// What does the algorithm do :
    /// 1-) Firstly it is ordering tiles by number and trying to find the "groups"
    /// 2-) After finding groups it is ordering tiles by colors and numbers and trying to find "runs"
    /// 
    /// What also needs to be done :
    /// 1-) Algorith should check every combination of sets with "runs" and "groups" in order to find which set is leaving out least amount of point
    /// 2-) Also a remainder that "2345" is almost twice better from "1234" because both 1 and 6 can help but for the second one only 5 will help    
    /// 
    /// So, the algorithm is working correctly by not in an effective way but i will make it more efficient shortly...
    /// I didn't add a check for jokers yet
    /// </summary>
    public class FindBestHand
    {
        public static Player GetBestHand(Player[] players)
        {
            int score = int.MaxValue;
            Player bestPlayer = null;

            for (int i = 0; i < players.Length; i++)
            {
                if (GetScore(players[i].Tiles) < score)
                {
                    score = GetScore(players[i].Tiles);
                    bestPlayer = players[i];
                }
            }

            return bestPlayer; // this will return the player who has the best hand
        }

        public static int GetScore(List<TileModel> tileModels)
        {           
            List<TileModel> meld = new List<TileModel>(); // creating an empty list to hold a meld
            List<TileModel> allMelds = new List<TileModel>();//([1,2,3],[5,6,7],[9,10,11]) this list is responsible of holding all melds
            //List<List<TileModel>> also an option or maybe a dictionary could be use but since the indexes doesnt matter dictionary will be pointless

            //we are ordering the player's hand with tiles numbers...1,2,3 etc
            List<TileModel> sortedTileModel = tileModels.OrderBy(o => o.Number).ToList();

            #region Finding "GROUPS"
            var firstTile = sortedTileModel[0]; // getting the first tile
            meld.Add(firstTile);//adding first tile in meld list

            for (int x = 1; x < sortedTileModel.Count; x++)
            {
                for (int y = 0; y < meld.Count; y++)
                {
                    if(sortedTileModel[x].Color == meld[y].Color || sortedTileModel[x].Number != meld[y].Number)//comparing new tile with every tile in the meld to check if we should add it or remove it
                    {
                        if(meld.Count < 3)
                        {
                            meld.Clear();
                            meld.Add(sortedTileModel[x]);
                            break;
                        }
                        else // if our meld's count is >= 3 then we dont have to broke that meld but start a new one and add them to allMelds list
                        {
                            for (int i = 0; i < meld.Count; i++)
                            {
                                allMelds.Add(meld[i]);
                            }

                            meld.Clear();
                        }
                    }
                }

                if(sortedTileModel[x].Color != firstTile.Color && sortedTileModel[x].Number == firstTile.Number)
                {
                    meld.Add(sortedTileModel[x]);
                }

                firstTile = sortedTileModel[x];
            }
            #endregion

            // removing the groups from all tiles
            for (int i = 0; i < meld.Count; i++)
            {
                allMelds.Remove(meld[i]);
            }
            meld.Clear();

            #region Finding "RUNS"
            //we are ordering the player's hand with tiles Colors and Numbers...Yellow0,Yellow1,Red0,Red1 etc
            sortedTileModel = tileModels.OrderBy(o => o.Color).ThenBy(o => o.Number).ToList();            

            firstTile = sortedTileModel[0]; // getting the first tile
            meld.Add(firstTile);//adding first in the meld list

            for (int x = 1; x < sortedTileModel.Count; x++)
            {
                for (int y = 0; y < meld.Count; y++)
                {
                    if(sortedTileModel[x].Color != meld[y].Color || (sortedTileModel[x].Number -1 != firstTile.Number && sortedTileModel[x].Number != firstTile.Number))
                    {
                        if (meld.Count < 3)
                        {
                            meld.Clear();
                            meld.Add(sortedTileModel[x]);
                            break;
                        }
                        else
                        {
                            for (int i = 0; i < meld.Count; i++)
                            {
                                allMelds.Add(meld[i]);
                            }

                            meld.Clear();
                            break;
                        }
                    }                                                            
                }

                if (sortedTileModel[x].Color == firstTile.Color && sortedTileModel[x].Number - 1 == firstTile.Number)
                {
                    meld.Add(sortedTileModel[x]);
                }

                if (sortedTileModel[x].Number == firstTile.Number)
                {
                    continue;
                }

                firstTile = sortedTileModel[x];
            }
            #endregion

            List<TileModel> tempTileList = new List<TileModel>(); // this is temporary and will be cleaned up later

            for (int i = 0; i < tileModels.Count; i++)
            {
                tempTileList.Add(tileModels[i]);
            }

            //removing melds from initial hand
            for (int i = 0; i < allMelds.Count; i++)
            {
                tempTileList.Remove(allMelds[i]);
            }

            //now we can calculate score by adding all tiles number those are out of sets--meaning out of any groups/runs
            int score = 0;
            for (int i = 0; i < tempTileList.Count; i++)
            {
                score += tempTileList[i].Number;
            }

            return score;            
        }
    }
}
