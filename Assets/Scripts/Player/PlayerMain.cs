using System.Collections;
using UnityEngine;
using Cinemachine;

using Actor;
using Core;
using Audio.Sound;

namespace Player {

    [RequireComponent(typeof(Health))]
    public class PlayerMain : MonoBehaviour, iActor, iLocalSoundPlayer {
        [SerializeField] CinemachineImpulseSource screenShakeOnDamage;
        [SerializeField] CinemachineImpulseSource screenShakeOnDeath;

        [Space]

        [SerializeField] EventChannelSO eventChannel;

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
        }

        void OnDisable() {
            if (health != null) {
                health.OnDamageTaken.Unsubscribe(OnDamageTaken);
                health.OnDeath.Unsubscribe(OnDeath);
            }
        }

        void Awake() {
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody2D>();
        }

        public System.Guid Guid() {
            return guid;
        }

        public bool IsAlive() {
            if (health == null) return false;
            return health.IsAlive();
        }

        public bool TakeDamage(float damage, Vector2 force) {
            rb.AddForce(force);
            return (health.TakeDamage(damage));
        }

        public void OnDamageTaken(float damage, float hp) {
            StartCoroutine(ScreenShakeOnDamage(damage));
            Debug.Log($"OnDamageTaken damage={damage} hp={hp}");
            PlaySound("PlayerDamage");
            eventChannel.OnShakeGamepad.Invoke(.2f, .5f);
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            // TODO: if (wasKilled) saySarcasticPun();
        }

        public void OnDeath(float damage, float hp) {
            StartCoroutine(ScreenShakeOnDeath());
            Debug.Log($"OnDeath damage={damage} hp={hp}");
            PlaySound("PlayerDeath");
            eventChannel.OnShakeGamepad.Invoke(1f, .7f);
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
