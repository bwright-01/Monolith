
using UnityEngine;

using Audio.Sound;

namespace Actor {

    [RequireComponent(typeof(Health))]
    public class GenericActor : MonoActor {

        void OnEnable() {
            SubscribeToEvents();
        }

        void OnDisable() {
            UnsubscribeFromEvents();
        }

        void Awake() {
            Init();
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
            // do nothing
        }

        public override void OnDeath(float damage, float hp) {
            CommonDeathActions();
        }
    }
}
