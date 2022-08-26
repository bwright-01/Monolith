using Core;
using UnityEngine;

// USAGE NOTES
// - place a DamageReceiver on any GameObject with a collider
// - implement iActor on the top-level MonoBehaviour (which will likely be the parent or grandparent to this script)
// that's it!

namespace Actor {

    public class DamageReceiver : MonoBehaviour {
        // components
        iActor actor;
        Rigidbody2D rb;
        Enemy.EnemyMain enemy;

        public iActor rootActor => actor;
        public new Rigidbody2D rigidbody => rb;

        public System.Nullable<System.Guid> guid => actor != null ? actor.GUID() : null;

        void Awake() {
            Layer.Init();
            rb = GetComponentInParent<Rigidbody2D>();
            actor = GetComponentInParent<iActor>();
            if (Layer.Enemy.Equals(gameObject.layer)) {
                enemy = GetComponentInParent<Enemy.EnemyMain>();
            }
        }

        public bool IsAlive() {
            return actor != null && actor.IsAlive();
        }

        public Region GetRegion() {
            if (actor == null) return null;
            return actor.GetRegion();
        }

        public void SetRegion(Region region) {
            if (enemy == null) return;
            enemy.SetRegion(region);
        }

        public bool TakeDamage(float damage, Vector2 force) {
            if (actor == null) return false;
            return actor.TakeDamage(damage, force);
        }
    }
}