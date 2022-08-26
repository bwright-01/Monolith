using UnityEngine;

namespace Actor {
    public interface iActor : iGuid {
        Region GetRegion();
        bool IsAlive();
        bool TakeDamage(float damage, Vector2 force);
        void OnDamageTaken(float damage, float hp);
        void OnDamageGiven(float damage, bool wasKilled); // negative hpRemaining indicates that the other entity has died
        void OnDeath(float damage, float hp);
    }
}
