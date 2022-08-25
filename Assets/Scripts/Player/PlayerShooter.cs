using UnityEngine;

using Core;

namespace Player {

    [RequireComponent(typeof(PlayerController))]
    public class PlayerShooter : MonoBehaviour {
        [SerializeField] Weapon.Gun gun;

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
        }

        void OnFirePress() {
            gun.TryShoot();
        }

        void OnMeleePress() {
            // TODO: IMPLEMENT
        }
    }
}
