using System.Collections;
using UnityEngine;

using Core;
using UnityEngine.EventSystems;

// This class houses all of the main systems that should persist from one scene to another
// This, and only this MonoBehaviour, should be a Singleton
// Any other systems that need to persist should be childed to this script's GameObject.

namespace Game {

    public class GameSystems : MonoBehaviour {

        [SerializeField] GameState _state;
        [SerializeField] EventSystem _eventSystem;
        [SerializeField] EventChannelSO eventChannel;

        public GameState state => _state;
        public EventSystem eventSystem => _eventSystem;

        // singleton
        static GameSystems _current;
        public static GameSystems current => _current;

        Coroutine ieRespawn;


        private void OnEnable() {
            eventChannel.OnPlayerDeath.Subscribe(OnPlayerDeath);
            eventChannel.OnMonolithDeath.Subscribe(OnMonolithDeath);
        }

        private void OnDisable() {
            eventChannel.OnPlayerDeath.Unsubscribe(OnPlayerDeath);
            eventChannel.OnMonolithDeath.Unsubscribe(OnMonolithDeath);
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

        void OnMonolithDeath(Environment.MonolithType monolithType) {
            state.SetMonolithDestroyed(monolithType);
        }
    }
}
