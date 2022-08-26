using UnityEngine;

namespace Enemy {

    public class EnemyMain : Actor.MonoActor {
        [SerializeField] Region region;

        void OnEnable() {
            SubscribeToEvents();
        }

        void OnDisable() {
            UnsubscribeFromEvents();
        }

        void Awake() {
            Init();
            if (region != null) region.RegisterActor(this);
        }

        public override Region GetRegion() {
            return region;
        }

        public void SetRegion(Region value) {
            region = value;
        }

        public override void OnDamageTaken(float damage, float hp) {
            CommonDamageActions();
        }

        public override void OnDamageGiven(float damage, bool wasKilled) {
            // TODO: if (wasKilled) GLOAT()
        }

        public override void OnDeath(float damage, float hp) {
            CommonDeathActions();
        }
    }
}
