
using UnityEngine;

using Core;

namespace Actor {

    public class Health : MonoBehaviour {
        [SerializeField] float startingHP = 100f;
        [SerializeField] bool isInvulnerable = false;
        [SerializeField] float timeInvincibleAfterHit = 0f;

        [HideInInspector] public HealthEventHandler OnDamageTaken = new HealthEventHandler();
        [HideInInspector] public HealthEventHandler OnDeath = new HealthEventHandler();

        // public
        public float Hp => hp;

        // state
        float hp = 100f;
        Timer timeInvincible = new Timer();

        void Start() {
            hp = startingHP;
        }

        public void SetIsInvulnerable(bool value) {
            isInvulnerable = value;
        }

        public bool TakeDamage(float damage) {
            if (damage <= 0) return false;
            if (isInvulnerable) return false;
            if (hp <= 0) return false;

            hp -= damage;

            if (hp <= 0) {
                OnDeath.Invoke(damage, hp);
            } else {
                OnDamageTaken.Invoke(damage, hp);
            }

            timeInvincible.SetDuration(timeInvincibleAfterHit);
            timeInvincible.Start();

            return true;
        }

        private void Update() {
            timeInvincible.Tick();
        }
    }
}
