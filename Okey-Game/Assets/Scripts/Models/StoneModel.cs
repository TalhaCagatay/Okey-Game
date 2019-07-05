using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// The data class that holds a stone info
    /// </summary>
    [System.Serializable]
    public class StoneModel
    {
        //TODO LATER declare this readonly
        public readonly int Number;
        public readonly StoneColor Color;
        public readonly bool IsJoker;

        public StoneModel(int Number, StoneColor Color, bool IsJoker)
        {
            this.Number = Number;
            this.Color = Color;
            this.IsJoker = IsJoker;
        }

        public static StoneColor IntToStoneColor(int i)
        {
            switch (i)
            {
                case 0: return StoneColor.Yellow;
                case 1: return StoneColor.Blue;
                case 2: return StoneColor.Black;
                case 3: return StoneColor.Red;
                default:
                    Debug.LogError("Error in Stone.IntToStoneColor(int) input has to be between 0-3, but it was " + i);
                    return StoneColor.Black;
            }

        }
    }
}
