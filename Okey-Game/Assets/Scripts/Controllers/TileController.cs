using System;
using System.Collections.Generic;
using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// TileController responsible tile related jobs
    /// </summary>
    public class TileController : MonoBehaviour
    {
        public static TileController Instance = null;

        public static event Action TilesInitialized;

        public TileModel[] TileArray = new TileModel[106]; // tile array holding all of our tiles
        private TileModel _indicator; // "Gösterge"
        private TileModel _okey; // "Okey"

        private void Awake()
        {
            Instance = this;

            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            TilePool.PoolInitialized += OnPoolInitialized;
        }

        private void UnSubscribeEvents()
        {
            TilePool.PoolInitialized -= OnPoolInitialized;
        }

        private void OnPoolInitialized()
        {
            CreateAllTiles();
            Shuffle();
            SetIndicator();
            SetOkey();

            Debug.Log("Tiles Initialized");
            if (TilesInitialized != null)
            {
                TilesInitialized.Invoke();
            }
        }

        // creating tiles, ((13 tiles * 4 colors) +1 joker) * 2 = 106 in total
        // it would be better if i add tiles to list but assignment document of Digitoy Games said array.
        private void CreateAllTiles()
        {
            int i = 0;

            for (int sayi = 0; sayi < 13; sayi++)
            {
                for (int renk = 0; renk < 4; renk++)
                {
                    TileArray[i] = new TileModel(sayi, TileModel.IntToTileColor(renk), false);
                    i++;
                    TileArray[i] = new TileModel(sayi, TileModel.IntToTileColor(renk), false);
                    i++;
                }
            }

            //add 2 jokers (sahte okey)
            TileArray[i] = new TileModel(52, TileModel.IntToTileColor(0), true); // color doesnt matter jokers use Okey's color
            i++;
            TileArray[i] = new TileModel(52, TileModel.IntToTileColor(0), true); // color doesnt matter jokers use Okey's color
            ///////////////////////////

            Debug.Log("Tiles Created");
            Debug.Log("Created tile count : " + TileArray.Length);
        }

        //setting Indicator(Gösterge)
        private void SetIndicator()
        {            
            _indicator = TileArray[UnityEngine.Random.Range(0, TileArray.Length)];
            Debug.Log("Indicator setted. Indicator is : " + _indicator.Number + ", " + _indicator.Color);
        }

        //setting Okey
        private void SetOkey()
        {
            _okey = _indicator;
            _okey.Number += 1;
            Debug.Log("Okey setted. Okey is : " + _okey.Number + ", " + _okey.Color);
        }

        //shuffling all tiles with using ShuffleTiles class
        private void Shuffle()
        {
            ShuffleTiles.Shuffle(TileArray, TileArray.Length);
        }
    }
}
