using System.Collections;
using UnityEngine;
using Cinemachine;

using Actor;
using Core;
using Audio.Sound;

namespace Player {

    public class PlayerMain : MonoBehaviour, iActor, iLocalSoundPlayer {
        [SerializeField] CinemachineImpulseSource screenShakeOnDamage;
        [SerializeField] CinemachineImpulseSource screenShakeOnDeath;

        // props
        System.Guid guid = new System.Guid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.

        // cached
        Health health;
        Rigidbody2D rb;

        public event StringEvent OnPlaySound;
        public void PlaySound(string soundName) {
            if (OnPlaySound != null) OnPlaySound.Invoke(soundName);
        }

        void OnEnable() {
            if (health != null) {
                health.OnDamageTaken.Subscribe(OnDamageTaken);
                health.OnDeath.Subscribe(OnDeath);
            }

            // TODO: REMOVE
            controller.OnFirePress.Subscribe(OnFirePress);
            controller.OnMeleePress.Subscribe(OnMeleePress);
        }

        void OnDisable() {
            if (health != null) {
                health.OnDamageTaken.Unsubscribe(OnDamageTaken);
                health.OnDeath.Unsubscribe(OnDeath);
            }

            // TODO: REMOVE
            controller.OnFirePress.Unsubscribe(OnFirePress);
            controller.OnMeleePress.Unsubscribe(OnMeleePress);
        }

        void Awake() {
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody2D>();

            // TODO: REMOVE
            controller = GetComponent<PlayerController>();
        }

        // TODO: REMOVE
        PlayerController controller;
        void OnFirePress() {
            OnDamageTaken(10f, 10f);
        }
        void OnMeleePress() {
            OnDeath(10f, 10f);
        }

        public System.Guid Guid() {
            return guid;
        }

        public bool IsAlive() {
            if (health == null) return false;
            return health.Hp > 0;
        }

        public bool TakeDamage(float damage, Vector2 force) {
            rb.AddForce(force);
            return (health.TakeDamage(damage));
        }

        public void OnDamageTaken(float damage, float hp) {
            StartCoroutine(ScreenShakeOnDamage(damage));
            Debug.Log($"OnDamageTaken damage={damage} hp={hp}");
            PlaySound("PlayerDamage");
        }

        public void OnDeath(float damage, float hp) {
            StartCoroutine(ScreenShakeOnDeath());
            Debug.Log($"OnDeath damage={damage} hp={hp}");
            PlaySound("PlayerDeath");
        }

        IEnumerator ScreenShakeOnDamage(float damage) {
            screenShakeOnDamage.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * damage * 0.1f);
            yield return new WaitForSeconds(0.1f);
            screenShakeOnDamage.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * damage * 0.1f);
        }

        IEnumerator ScreenShakeOnDeath() {
            screenShakeOnDeath.GenerateImpulse(Vector3.right);
            yield return new WaitForSeconds(0.1f);
            screenShakeOnDeath.GenerateImpulse(Vector3.up);
        }
    }
}
