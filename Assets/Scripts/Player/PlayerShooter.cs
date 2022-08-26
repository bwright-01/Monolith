using UnityEngine;

using Core;

namespace Player {

    [RequireComponent(typeof(PlayerController))]
    public class PlayerShooter : MonoBehaviour {
        [SerializeField] Weapon.Gun gun;
        [SerializeField] Weapon.Melee melee;

        PlayerController controller;

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
        }

        void OnFirePress() {
            gun.TryAttack();
        }

        void OnMeleePress() {
            melee.TryAttack();
        }
    }
}
