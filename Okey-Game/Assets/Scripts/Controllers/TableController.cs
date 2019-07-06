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

        private static Stack<TileModel> _stones = new Stack<TileModel>(); // stones to be dealt to players

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
            TileController.TilesInitialized += OnStonesInitialized;
        }

        private void UnSubscribeEvents()
        {
            TileController.TilesInitialized -= OnStonesInitialized;
        }

        private void OnStonesInitialized()
        {
            for (int i = 0; i < TileController.Instance.TileArray.Length; i++)
            {                
                _stones.Push(TileController.Instance.TileArray[i]);
            }
            Debug.Log("Stones added to stack");

            Debug.Log("Table Initialized");
            if (TableInitialized != null)
            {
                TableInitialized.Invoke();
            }
        }

        public List<TileModel> GetNStone(int N)
        {
            List<TileModel> stoneList = new List<TileModel>();
            for (int i = 0; i < N; i++)
            {
                stoneList.Add(_stones.Pop());
            }

            return stoneList;
        }
    }
}
