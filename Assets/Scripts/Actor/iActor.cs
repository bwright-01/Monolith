
namespace Actor {
    public interface iActor : iGuid {
        bool IsAlive();
        void OnDamageTaken(float damage, float hp);
        void OnDeath(float damage, float hp);
    }
}
