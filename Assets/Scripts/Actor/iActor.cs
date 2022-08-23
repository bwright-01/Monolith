using UnityEngine;

namespace Actor {
    public interface iActor : iGuid {
        bool IsAlive();
        bool TakeDamage(float damage, Vector2 force);
        void OnDamageTaken(float damage, float hp);
        void OnDeath(float damage, float hp);
    }
}
