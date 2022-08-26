using System.Collections;
using UnityEngine;
using Cinemachine;

using Actor;
using Core;
using Audio.Sound;

namespace Player {

    [RequireComponent(typeof(Health))]
    public class PlayerMain : MonoBehaviour, iActor {
        [SerializeField] CinemachineImpulseSource screenShakeOnDamage;
        [SerializeField] CinemachineImpulseSource screenShakeOnDeath;

        [Space]
        [Space]

        [SerializeField] SingleSound damageSound;
        [SerializeField] SingleSound deathSound;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        // props
        System.Guid guid = System.Guid.NewGuid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.

        // cached
        Health health;
        Rigidbody2D rb;

        void OnEnable() {
            health.OnDamageTaken.Subscribe(OnDamageTaken);
            health.OnDeath.Subscribe(OnDeath);
        }

        void OnDisable() {
            health.OnDamageTaken.Unsubscribe(OnDamageTaken);
            health.OnDeath.Unsubscribe(OnDeath);
        }

        void Awake() {
            health = GetComponent<Health>();
            rb = GetComponent<Rigidbody2D>();
            damageSound.Init(this);
            deathSound.Init(this);
        }

        public System.Guid GUID() {
            return guid;
        }

        public bool IsAlive() {
            if (health == null) return false;
            return health.IsAlive();
        }

        public bool TakeDamage(float damage, Vector2 force) {
            rb.AddForce(force, ForceMode2D.Impulse);
            return (health.TakeDamage(damage));
        }

        public void OnDamageTaken(float damage, float hp) {
            StartCoroutine(ScreenShakeOnDamage(damage));
            Debug.Log($"OnDamageTaken damage={damage} hp={hp}");
            damageSound.Play();
            eventChannel.OnShakeGamepad.Invoke(.2f, .5f);
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            // TODO: if (wasKilled) saySarcasticPun();
        }

        public void OnDeath(float damage, float hp) {
            StartCoroutine(ScreenShakeOnDeath());
            Debug.Log($"OnDeath damage={damage} hp={hp}");
            deathSound.Play();
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
