using System;
using System.Collections.Generic;
using UnityEngine;

namespace TÇI
{
    /// <summary>
    /// table is responsible for method for giving stones to players
    /// </summary>
    public class TableController : MonoBehaviour
    {
        public static TableController Instance = null;
        public static event Action TableInitialized;

        private static Stack<TileModel> _tiles = new Stack<TileModel>(); // stones to be dealt to players

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
            TileController.TilesInitialized += OnTilesInitialized;
        }

        private void UnSubscribeEvents()
        {
            TileController.TilesInitialized -= OnTilesInitialized;
        }

        private void OnTilesInitialized()
        {
            for (int i = 0; i < TileController.Instance.TileArray.Length; i++)
            {                
                _tiles.Push(TileController.Instance.TileArray[i]);
            }
            Debug.Log("Tiles added to stack");

            Debug.Log("Table Initialized");
            if (TableInitialized != null)
            {
                TableInitialized.Invoke();
            }
        }

        public List<TileModel> GetNTile(int N)
        {
            List<TileModel> tileList = new List<TileModel>();
            for (int i = 0; i < N; i++)
            {
                tileList.Add(_tiles.Pop());
            }

            return tileList;
        }
    }
}
