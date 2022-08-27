using UnityEngine;

using Core;
using Audio.Sound;

namespace Player {

    [RequireComponent(typeof(PlayerController))]
    public class PlayerShooter : MonoBehaviour {
        [SerializeField] Weapon.Gun gun;
        [SerializeField] Weapon.Melee melee;

        [Space]
        [Space]

        [SerializeField] SingleSound noAmmoClick;

        [Space]
        [Space]

        [SerializeField][Range(0f, 10f)] float timeRecharge = 3f;
        [SerializeField][Range(0f, 10)] int numberOfShots = 5;

        PlayerController controller;

        Timer recharging = new Timer();

        public int GetNumShots() {
            return numberOfShots;
        }

        public int GetNumAvailableShots() {
            return (int)Mathf.Floor(recharging.value * numberOfShots);
        }

        void OnEnable() {
            controller.OnFirePress.Subscribe(OnFirePress);
            controller.OnMeleePress.Subscribe(OnMeleePress);
        }

        void OnDisable() {
            controller.OnFirePress.Unsubscribe(OnFirePress);
            controller.OnMeleePress.Unsubscribe(OnMeleePress);
        }

        void Awake() {
            controller = GetComponent<PlayerController>();
            if (gun == null) Debug.LogError($"Gun is null in {Utils.FullGameObjectName(gameObject)}");
            if (melee == null) Debug.LogError($"Melee is null in {Utils.FullGameObjectName(gameObject)}");
            noAmmoClick.Init(this);
            recharging.SetDuration(timeRecharge);
            recharging.Start();
        }

        private void Update() {
            recharging.TickReversed();

            Debug.Log($"{GetNumAvailableShots()} {recharging.ToString()}");
        }

        void OnFirePress() {
            if (HasAvailableBullet()) {
                gun.TryAttack();
                SpendAmmo();
            } else {
                noAmmoClick.Play();
            }
        }

        void OnMeleePress() {
            melee.TryAttack();
        }

        bool HasAvailableBullet() {
            return GetNumAvailableShots() >= 1;
        }

        void SpendAmmo() {
            recharging.SetValue(recharging.value - (1f / numberOfShots));
        }
    }
}
