
using UnityEngine;

using Audio.Sound;
using Core;

namespace Actor {

    [RequireComponent(typeof(Health))]
    public class GenericActor : MonoBehaviour, iActor, iLocalSoundPlayer {
        [SerializeField] string damageSound = "ActorDamageSound";
        [SerializeField] string deathSound = "ActorDeathSound";

        [Space]

        // props
        System.Guid guid = new System.Guid();

        // cached
        Health health;
        DamageFlash damageFlash;

        public event StringEvent OnPlaySound;
        public void PlaySound(string soundName) {
            if (OnPlaySound != null) OnPlaySound.Invoke(soundName);
        }

        public System.Guid Guid() {
            return guid;
        }

        public bool IsAlive() {
            return health.IsAlive();
        }

        public bool TakeDamage(float damage, Vector2 force) {
            if (damageFlash != null) damageFlash.StartFlashing();
            return health.TakeDamage(damage);
        }

        public void OnDamageTaken(float damage, float hp) {
            PlaySound(damageSound);
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            // do nothing
        }

        public void OnDeath(float damage, float hp) {
            PlaySound(deathSound);
            Destroy(gameObject);
        }

        void Awake() {
            health = GetComponent<Health>();
            damageFlash = GetComponent<DamageFlash>();
        }
    }
}
