using System.Collections;
using UnityEngine;

using Core;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
            eventChannel.OnApplyUpgrade.Subscribe(OnApplyUpgrade);
        }

        private void OnDisable() {
            eventChannel.OnPlayerDeath.Unsubscribe(OnPlayerDeath);
            eventChannel.OnMonolithDeath.Unsubscribe(OnMonolithDeath);
            eventChannel.OnApplyUpgrade.Unsubscribe(OnApplyUpgrade);
        }

        public void ResetGameState() {
            state.Init();
        }

        public void ResetForContinue() {
            state.ResetForContinue();
        }

        void Awake() {
            Layer.Init();
            ResetGameState();
            _current = SystemUtils.ManageSingleton<GameSystems>(_current, this);
        }

        void OnPlayerDeath() {
            state.LoseLife();
            if (state.lives <= 0) {
                StartCoroutine(IGameOver());
            } else {
                eventChannel.OnRespawnPlayer.Invoke();
            }
        }

        void GotoGameOver() {
            eventChannel.OnStopMusic.Invoke();
            SceneManager.LoadScene("GameOver");
        }

        void OnMonolithDeath(Environment.MonolithType monolithType) {
            state.SetMonolithDestroyed(monolithType);
            if (state.AreAllMonolithsDestroyed) {
                eventChannel.OnAllMonolithsDestroyed.Invoke();
            }
        }

        void OnApplyUpgrade(UpgradeType upgradeType) {
            state.ApplyUpgrade(upgradeType);
            eventChannel.OnAbilityUpgraded.Invoke(upgradeType);
        }

        IEnumerator IGameOver() {
            yield return new WaitForSecondsRealtime(2f);
            Time.timeScale = 1f;
            GotoGameOver();
        }
    }
}
