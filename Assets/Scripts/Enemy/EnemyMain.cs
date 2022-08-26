using UnityEngine;

using Core;
using Audio.Sound;

namespace Enemy {

    [RequireComponent(typeof(Actor.Health))]
    public class EnemyMain : MonoBehaviour, Actor.iActor {
        [SerializeField] Region region;

        [Space]

        [SerializeField] SingleSound damageSound;
        [SerializeField] SingleSound deathSound;

        [Space]

        [SerializeField] EventChannelSO eventChannel;

        [SerializeField] GameObject[] killObjectsOnDeath = new GameObject[] { };

        // props
        System.Guid guid = System.Guid.NewGuid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.

        // cached
        Actor.Health health;
        Actor.DamageFlash damageFlash;

        void OnEnable() {
            health.OnDamageTaken.Subscribe(OnDamageTaken);
            health.OnDeath.Subscribe(OnDeath);
        }

        void OnDisable() {
            health.OnDamageTaken.Unsubscribe(OnDamageTaken);
            health.OnDeath.Unsubscribe(OnDeath);
        }

        void Awake() {
            health = GetComponent<Actor.Health>();
            damageFlash = GetComponent<Actor.DamageFlash>();
            if (region == null) Destroy(gameObject);
            region.RegisterActor(this);
            damageSound.Init(this);
            deathSound.Init(this);
        }

        public System.Guid GUID() {
            return guid;
        }

        public void SetRegion(Region value) {
            region = value;
        }

        public bool IsAlive() {
            return health.IsAlive();
        }

        public bool TakeDamage(float damage, Vector2 force) {
            return health.TakeDamage(damage);
        }

        public void OnDamageTaken(float damage, float hp) {
            if (damageFlash != null) damageFlash.StartFlashing();
            damageSound.Play();
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            // TODO: if (wasKilled) GLOAT()
        }

        public void OnDeath(float damage, float hp) {
            deathSound.Play();
            eventChannel.OnEnemyDeath.Invoke(this);
            foreach (var obj in killObjectsOnDeath) {
                Destroy(obj);
            }
        }
    }
}
