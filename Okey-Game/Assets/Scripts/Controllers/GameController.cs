﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace TÇI
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance = null;
        public static event Action GameControllerInitialized;

        [SerializeField] private Player[] _players;

        private void Awake()
        {
            Instance = this;

            SubscribeEvents();

            Debug.Log("GameController Initialized");
            if (GameControllerInitialized != null)
            {
                GameControllerInitialized.Invoke();
            }
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            TableController.TableInitialized += OnTableInitialized;
        }

        private void UnSubscribeEvents()
        {
            TableController.TableInitialized -= OnTableInitialized;
        }

        private void OnTableInitialized()
        {
            //15
            _players[0].GiveStones(TableController.Instance.GetNTile(15));
            //14
            _players[1].GiveStones(TableController.Instance.GetNTile(14));
            _players[2].GiveStones(TableController.Instance.GetNTile(14));
            _players[3].GiveStones(TableController.Instance.GetNTile(14));
            Debug.Log("Tiles are dealt to all players");

            LogPlayersHands();
        }

        private void LogPlayersHands()
        {
            string hand = "";
            for (int i = 0; i < _players[0].Tiles.Count; i++)
            {
                hand += _players[0].Tiles[i].Color + " " + _players[0].Tiles[i].Number + " - ";
            }
            Debug.LogWarning("Player1 Hand : " + hand);

            hand = "";
            for (int i = 0; i < _players[1].Tiles.Count; i++)
            {
                hand += _players[1].Tiles[i].Color + " " + _players[1].Tiles[i].Number + " - ";
            }
            Debug.LogWarning("Player2 Hand : " + hand);

            hand = "";
            for (int i = 0; i < _players[2].Tiles.Count; i++)
            {
                hand += _players[2].Tiles[i].Color + " " + _players[2].Tiles[i].Number + " - ";
            }
            Debug.LogWarning("Player3 Hand : " + hand);

            hand = "";
            for (int i = 0; i < _players[3].Tiles.Count; i++)
            {
                hand += _players[3].Tiles[i].Color + " " + _players[3].Tiles[i].Number + " - ";
            }
            Debug.LogWarning("Player4 Hand : " + hand);

            ////finding best hand and logging it
            //List<TileModel> bestHand = new List<TileModel>();
            //bestHand = FindBestHand.GetBestHand(_players[3]);

            //hand = "";
            //for (int i = 0; i < bestHand.Count; i++)
            //{
            //    hand += bestHand[i].Color + ", " + bestHand[i].Number + " - ";
            //}
            //Debug.LogWarning("Best Hand : " + hand);
        }
    }
}
