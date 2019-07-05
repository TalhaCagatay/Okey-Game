using System;
using UnityEngine;

namespace TÇI
{
    public class GameController : MonoBehaviour
    {
        public static event Action GameControllerInitialized;

        private void Awake()
        {
            SubscribeEvents();

            if(GameControllerInitialized != null)
            {
                Debug.Log("GameController Initialized");
                GameControllerInitialized.Invoke();
            }
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {

        }

        private void UnSubscribeEvents()
        {

        }
    }
}
