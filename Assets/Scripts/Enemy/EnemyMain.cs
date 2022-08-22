using UnityEngine;

using Core;

namespace Enemy {

    public class EnemyMain : MonoBehaviour, Actor.iActor {
        [SerializeField] Region region;

        [Space]

        [SerializeField] EventChannelSO eventChannel;

        // props
        System.Guid guid = new System.Guid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.


        public System.Guid Guid() {
            return guid;
        }

        public void SetRegion(Region value) {
            region = value;
        }

        public bool IsAlive() {
            throw new System.NotImplementedException();
        }

        public void OnDamageTaken(float damage, float hp) {
            throw new System.NotImplementedException();
        }

        public void OnDeath(float damage, float hp) {
            eventChannel.OnEnemyDeath.Invoke(this);
        }

        public bool TakeDamage(float damage, Vector2 force) {
            throw new System.NotImplementedException();
        }

        void Awake() {
            if (region == null) Destroy(gameObject);
            region.RegisterActor(this);
        }
    }
}
