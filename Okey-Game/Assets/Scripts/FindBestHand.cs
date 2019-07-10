using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// this class is for checking series and groups in players hand and finding the best one
    /// I haven't got enough time to make this algorithm better but this will work.
    /// What also needs to be done :
    /// 1-) Algorith should check every combination of sets with "runs" and "groups" in order to find which set is leaving out least amount of point
    /// 2-) Also a remainder that "2345" is almost twice better from "1234" because both 1 and 6 can help but for the second one only 5 will help    
    /// </summary>
    public class FindBestHand
    {
        public static List<TileModel> GetBestHand(Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                GetSets(players[i].Tiles);
            }

            return null; // this will return the player who has the best hand
        }

        public static List<TileModel> GetSets(List<TileModel> tileModels)
        {
            //we are ordering the player's hand with tiles numbers...1,2,3 etc
            List<TileModel> sortedTileModel = tileModels.OrderBy(o => o.Number).ToList();

            string hand = "";
            for (int i = 0; i < sortedTileModel.Count; i++)
            {
                hand += sortedTileModel[i].Color + ", " + sortedTileModel[i].Number + " - ";
            }
            Debug.LogWarning("sorted Hand by Number: " + hand);

            List<TileModel> meld = new List<TileModel>(); // creating an empty list to hold a meld
            List<TileModel> allMelds = new List<TileModel>();//([1,2,3],[5,6,7],[9,10,11]) this list is responsible of holding all melds
            //List<List<TileModel>> also an option or maybe a dictionary could be use but since the indexes doesnt matter dictionary will be pointless

            var firstTile = sortedTileModel[0]; // getting the first tile
            meld.Add(firstTile);//adding first in the meld list

            #region Finding "GROUPS"
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
                        else // if our meld's count is not < 3 then we dont have to broke that meld but start a new one and add them to allMelds list
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

            meld.Clear();

            #region Finding "RUNS"
            //we are ordering the player's hand with tiles Colors and Numbers...Yellow0,Yellow1,Red0,Red1 etc
            sortedTileModel = tileModels.OrderBy(o => o.Color).ThenBy(o => o.Number).ToList();

            hand = "";
            for (int i = 0; i < sortedTileModel.Count; i++)
            {
                hand += sortedTileModel[i].Color + ", " + sortedTileModel[i].Number + " - ";
            }
            Debug.LogWarning("sorted Hand by Color: " + hand);

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

            hand = "";
            for (int i = 0; i < allMelds.Count; i++)
            {
                hand += allMelds[i].Color + ", " + allMelds[i].Number + " - ";
            }
            Debug.LogWarning("All Sets : " + hand);

            return null;            
        }

        //protected Meld getGreatestMeld(ArrayList<Meld> melds)
        //{
        //    if (melds.size() == 0) return null;
        //    Meld greatestMeld = melds.get(0);
        //    for (Meld tempMeld : melds)
        //    {
        //        if (tempMeld.getValue() > greatestMeld.getValue())
        //        {
        //            greatestMeld = tempMeld;
        //        }
        //    }
        //    return greatestMeld;
        //}
    }
}
