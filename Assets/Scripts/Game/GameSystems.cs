using System.Collections;
using UnityEngine;

using Core;

// This class houses all of the main systems that should persist from one scene to another
// This, and only this MonoBehaviour, should be a Singleton
// Any other systems that need to persist should be childed to this script's GameObject.

namespace Game {

    public class GameSystems : MonoBehaviour {

        [SerializeField] GameState _state;
        [SerializeField] EventChannelSO eventChannel;

        public GameState state => _state;

        // singleton
        static GameSystems _current;
        public static GameSystems current => _current;

        Coroutine ieRespawn;

        private void OnEnable() {
            eventChannel.OnPlayerDeath.Subscribe(OnPlayerDeath);
        }

        private void OnDisable() {
            eventChannel.OnPlayerDeath.Unsubscribe(OnPlayerDeath);
        }

        void Awake() {
            Layer.Init();
            state.Init();
            _current = SystemUtils.ManageSingleton<GameSystems>(_current, this);
        }

        void OnPlayerDeath() {
            state.LoseLife();
            if (state.lives <= 0) {
                // TODO: HANDLE GAME OVER STATE
            }
            eventChannel.OnRespawnPlayer.Invoke();
        }
    }
}
