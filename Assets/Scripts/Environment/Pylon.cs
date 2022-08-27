using UnityEngine;

namespace Environment {

    public class Pylon : Actor.MonoActor {
        Monolith monolith;

        void OnEnable() {
            SubscribeToEvents();
        }

        void OnDisable() {
            UnsubscribeFromEvents();
        }

        void Awake() {
            Init();
            monolith = GetComponentInParent<Monolith>();
            monolith.RegisterPylon(this);
            transform.SetParent(monolith.transform.parent.parent);
        }

        public override Region GetRegion() {
            return null;
        }

        public override void OnHealthGained(float amount, float hp) {
            // do nothing
        }

        public override void OnDamageTaken(float damage, float hp) {
            CommonDamageActions();
        }

        public override void OnDamageGiven(float damage, bool wasKilled) {
            //
        }

        public override void OnDeath(float damage, float hp) {
            CommonDeathActions();
            monolith.ReportPylonDeath(this);
        }
    }
}

