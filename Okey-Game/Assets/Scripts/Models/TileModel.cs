using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// The data class that holds a stone info
    /// </summary>
    [System.Serializable]
    public class TileModel
    {
        //TODO LATER declare this readonly
        public int Number;
        public TileColor Color;
        public readonly bool IsJoker;

        public TileModel(int Number, TileColor Color, bool IsJoker)
        {
            this.Number = Number;
            this.Color = Color;
            this.IsJoker = IsJoker;
        }

        public static TileColor IntToTileColor(int i)
        {
            switch (i)
            {
                case 0: return TileColor.Yellow;
                case 1: return TileColor.Blue;
                case 2: return TileColor.Black;
                case 3: return TileColor.Red;
                default:
                    Debug.LogError("Error in Stone.IntToStoneColor(int) input has to be between 0-3, but it was " + i);
                    return TileColor.Black;
            }

        }
    }
}
