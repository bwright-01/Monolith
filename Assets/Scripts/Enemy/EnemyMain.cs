using UnityEngine;

using Core;

namespace Enemy {

    [RequireComponent(typeof(Actor.Health))]
    public class EnemyMain : MonoBehaviour, Actor.iActor {
        [SerializeField] Region region;

        [Space]

        [SerializeField] EventChannelSO eventChannel;

        [SerializeField] GameObject[] killObjectsOnDeath = new GameObject[] { };

        // props
        System.Guid guid = new System.Guid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.

        // cached
        Actor.Health health;

        public System.Guid Guid() {
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
            // TODO: ADD DAMAGE NOISE
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            // TODO: if (wasKilled) GLOAT()
        }

        public void OnDeath(float damage, float hp) {
            eventChannel.OnEnemyDeath.Invoke(this);
            foreach (var obj in killObjectsOnDeath) {
                Destroy(obj);
            }
        }

        void Awake() {
            health = GetComponent<Actor.Health>();
            if (region == null) Destroy(gameObject);
            region.RegisterActor(this);
        }
    }
}
