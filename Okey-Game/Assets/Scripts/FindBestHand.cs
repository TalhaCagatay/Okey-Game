using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// this class is for checking series and groups in players hand and finding the best one
    /// IN PROGRESS
    /// </summary>
    public class FindBestHand
    {
        public static List<TileModel> GetBestHand(Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                CheckConsecutiveSeries(players[i].Tiles);
            }

            return null;
        }

        public static List<TileModel> CheckConsecutiveSeries(List<TileModel> tileModels)
        {
            List<TileModel> sortedTileModel = tileModels.OrderBy(o => o.Number).ToList();

            bool consecutive = true;
            int count = 0;
            var firstValue = sortedTileModel[0].Number;
            for (var i = 0; i < sortedTileModel.Count; i++)
            {
                if (sortedTileModel[i].Number - i != firstValue)
                {
                    consecutive = false;
                    break;
                }
            }

            Debug.LogError("Player ConsecutiveRunCount : " + count);

            
            

            return sortedTileModel;



//#-------Variables
//            total_results = []
//    #-------Variables
//    total_results.append(mycards[start])
//    for item in mycards: 
//        if (item[0] == total_results[-1][0] + 1 or item[0] == 20) and(item[1] == total_results[-1][1] or item[1] == "jocker"):
//            total_results.append(item)
//    if len(total_results) >= 3:
//        return total_results
//    else: 
//        return False
        }
    }
}
