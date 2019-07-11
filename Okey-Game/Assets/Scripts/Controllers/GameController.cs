using System;
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

            LogScoresAndBestPlayer(out hand);
        }

        private void LogScoresAndBestPlayer(out string hand)
        {
            hand = "";
            Debug.Log("Player1 score : " + FindBestHand.GetScore(_players[0].Tiles));
            hand = "";
            Debug.Log("Player2 score : " + FindBestHand.GetScore(_players[1].Tiles));
            hand = "";
            Debug.Log("Player3 score : " + FindBestHand.GetScore(_players[2].Tiles));
            hand = "";
            Debug.Log("Player4 score : " + FindBestHand.GetScore(_players[3].Tiles));

            var bestPlayer = FindBestHand.GetBestHand(_players);

            hand = "";
            for (int i = 0; i < bestPlayer.Tiles.Count; i++)
            {
                hand += bestPlayer.Tiles[i].Color + " " + bestPlayer.Tiles[i].Number + " - ";
            }

            for (int i = 0; i < _players.Length; i++)
            {
                if (_players[i] == bestPlayer)
                {
                    Debug.Log("Player" + (i + 1) + " has the best hand with : " + hand);
                    Debug.Log("Player" + (i + 1) + " has the best score with : " + FindBestHand.GetScore(_players[i].Tiles));

                    return;
                }
            }
        }
    }
}