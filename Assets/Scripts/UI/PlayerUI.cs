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

        [SerializeField] GameObject shooterSection;
        [SerializeField] Image shot1;
        [SerializeField] Image shot2;
        [SerializeField] Image shot3;
        [SerializeField] Image shot4;
        [SerializeField] Image shot5;
        [SerializeField] Image shot6;
        [SerializeField] Image shot7;
        [SerializeField] Image shot8;
        [SerializeField] Image shot9;
        [SerializeField] Image shot10;
        [SerializeField] GameObject shotSlot6;
        [SerializeField] GameObject shotSlot7;
        [SerializeField] GameObject shotSlot8;
        [SerializeField] GameObject shotSlot9;
        [SerializeField] GameObject shotSlot10;

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
        Player.PlayerShooter shooter;
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
            canvas.gameObject.SetActive(false);
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
            canvas.gameObject.SetActive(true);
            canvas.enabled = true;
        }

        void OnPlayerDeath() {
            Reset();
        }

        void OnPlayerSpawned(Player.PlayerMain incoming) {
            if (incoming == null || !incoming.IsAlive()) return;
            player = incoming;
            health = player.actorHealth;
            shooter = player.GetShooter();
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
            UpdateHealthUI();
            UpdateWeaponUI();
        }

        void UpdateHealthUI() {
            healthSlider.value = health.healthPercentage;
            healthBarFill.color = showingDamage.active ? damageColor : initialHealthColor;
            life1.enabled = Game.GameSystems.current.state.lives >= 1;
            life2.enabled = Game.GameSystems.current.state.lives >= 2;
            life3.enabled = Game.GameSystems.current.state.lives >= 3;
            life4.enabled = Game.GameSystems.current.state.lives >= 4;
            life5.enabled = Game.GameSystems.current.state.lives >= 5;
        }

        void UpdateWeaponUI() {
            if (shooter == null) {
                shooterSection.SetActive(false);
                return;
            }
            shooterSection.SetActive(true);
            shot1.enabled = shooter.GetNumAvailableShots() >= 1;
            shot2.enabled = shooter.GetNumAvailableShots() >= 2;
            shot3.enabled = shooter.GetNumAvailableShots() >= 3;
            shot4.enabled = shooter.GetNumAvailableShots() >= 4;
            shot5.enabled = shooter.GetNumAvailableShots() >= 5;
            shot6.enabled = shooter.GetNumAvailableShots() >= 6;
            shot7.enabled = shooter.GetNumAvailableShots() >= 7;
            shot8.enabled = shooter.GetNumAvailableShots() >= 8;
            shot9.enabled = shooter.GetNumAvailableShots() >= 9;
            shot10.enabled = shooter.GetNumAvailableShots() >= 10;

            shotSlot6.SetActive(shooter.GetNumShots() >= 6);
            shotSlot7.SetActive(shooter.GetNumShots() >= 7);
            shotSlot8.SetActive(shooter.GetNumShots() >= 8);
            shotSlot9.SetActive(shooter.GetNumShots() >= 9);
            shotSlot10.SetActive(shooter.GetNumShots() >= 10);
        }
    }
}
