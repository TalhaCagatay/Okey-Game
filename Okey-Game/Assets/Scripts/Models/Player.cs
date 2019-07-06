using UnityEngine;
using System.Collections.Generic;

namespace TÇI
{
    /// <summary>
    /// this is a base class for all players and all players rather it is human or AI should inherit from this class
    /// </summary>
    public abstract class Player : MonoBehaviour
    {
        // we dont have an AI but if we did, boolenas below can be used
        public bool IsAI = true;
        public bool IsMyTurn;
        //////////////////////////////////////////////////////////////

        // this list holding player's hand
        public List<TileModel> Tiles = new List<TileModel>();

        public void GiveStones(List<TileModel> tiles)
        {
            Tiles.AddRange(tiles);
        }

        //incase we want to use...(not used at the moment)
        public abstract void PlayTurn();
    }
}