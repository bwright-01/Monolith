using UnityEngine;
using UnityEngine.UI;

using Core;

namespace UI {

    public class PlayerUI : MonoBehaviour {

        [SerializeField] Color damageColor = Color.red;
        [SerializeField][Range(0f, 1f)] float timeShowDamage = 0.5f;

        [Space]
        [Space]

        [SerializeField] Image life1;
        [SerializeField] Image life2;
        [SerializeField] Image life3;
        [SerializeField] Image life4;
        [SerializeField] Image life5;

        [Space]
        [Space]

        [SerializeField] Canvas canvas;
        [SerializeField] Slider healthSlider;
        [SerializeField] Image healthBarFill;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        // cached
        Color initialHealthColor;
        Player.PlayerMain player;
        Actor.Health health;

        Timer showingDamage = new Timer();

        // state
        // Coroutine ieChangeHealth;
        // float healthValue = 0.5f;

        void OnEnable() {
            eventChannel.OnPlayerSpawned.Subscribe(OnPlayerSpawned);
            eventChannel.OnPlayerDeath.Subscribe(OnPlayerDeath);
        }

        void OnDisable() {
            eventChannel.OnPlayerSpawned.Unsubscribe(OnPlayerSpawned);
            eventChannel.OnPlayerDeath.Unsubscribe(OnPlayerDeath);
            Reset();
        }

        void Awake() {
            initialHealthColor = healthBarFill.color;
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            Reset();
        }

        void Update() {
            UpdateUI();
            showingDamage.Tick();
        }

        void Reset() {
            canvas.enabled = false;
            if (health != null) {
                health.OnHealthGained.Unsubscribe(OnHealthGained);
                health.OnDamageTaken.Unsubscribe(OnDamageTaken);
            }
            player = null;
            health = null;
            StopAllCoroutines();
        }

        void Activate() {
            canvas.enabled = true;
        }

        void OnPlayerDeath() {
            Reset();
        }

        void OnPlayerSpawned(Player.PlayerMain incoming) {
            if (incoming == null || !incoming.IsAlive()) return;
            player = incoming;
            health = player.actorHealth;
            health.OnHealthGained.Subscribe(OnHealthGained);
            health.OnDamageTaken.Subscribe(OnDamageTaken);
            UpdateUI();
            Activate();
        }

        public void OnHealthGained(float amount, float hp) {
            // nothing for now
        }

        public void OnDamageTaken(float damage, float hp) {
            showingDamage.SetDuration(timeShowDamage);
            showingDamage.Start();
        }

        bool IsPlayerAlive() {
            if (player == null) return false;
            if (health == null) return false;
            if (!player.IsAlive()) return false;
            if (!health.IsAlive()) return false;
            return true;
        }

        void UpdateUI() {
            if (!IsPlayerAlive()) return;
            healthSlider.value = health.healthPercentage;
            healthBarFill.color = showingDamage.active ? damageColor : initialHealthColor;
            life1.enabled = Game.GameSystems.current.state.lives >= 1;
            life2.enabled = Game.GameSystems.current.state.lives >= 2;
            life3.enabled = Game.GameSystems.current.state.lives >= 3;
            life4.enabled = Game.GameSystems.current.state.lives >= 4;
            life5.enabled = Game.GameSystems.current.state.lives >= 5;
        }
    }
}
