
using UnityEngine;

using Audio.Sound;
using Core;

namespace Actor {

    [RequireComponent(typeof(Health))]
    public class GenericActor : MonoBehaviour, iActor {
        [SerializeField] bool debug = false;

        [Space]

        [SerializeField] SingleSound damageSound;
        [SerializeField] SingleSound deathSound;

        // props
        System.Guid guid = System.Guid.NewGuid();

        // cached
        Rigidbody2D rb;
        Health health;
        DamageFlash damageFlash;

        void OnEnable() {
            health.OnDamageTaken.Subscribe(OnDamageTaken);
            health.OnDeath.Subscribe(OnDeath);
        }

        void OnDisable() {
            health.OnDamageTaken.Unsubscribe(OnDamageTaken);
            health.OnDeath.Unsubscribe(OnDeath);
        }

        void Awake() {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            damageFlash = GetComponent<DamageFlash>();
            damageSound.Init(this);
            deathSound.Init(this);
        }

        public System.Guid GUID() {
            return guid;
        }

        public bool IsAlive() {
            return health.IsAlive();
        }

        public bool TakeDamage(float damage, Vector2 force) {
            if (rb != null) rb.AddForce(force, ForceMode2D.Impulse);
            return health.TakeDamage(damage);
        }

        public void OnDamageTaken(float damage, float hp) {
            if (damageFlash != null) damageFlash.StartFlashing();
            damageSound.Play();
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            // do nothing
        }

        public void OnDeath(float damage, float hp) {
            deathSound.Play();
            Destroy(gameObject);
        }

        void OnGUI() {
            if (debug) {
                GUILayout.TextField("Generic Actor");
                if (GUILayout.Button("Take Damage")) {
                    TakeDamage(10f, Vector2.zero);
                }
            }
        }
    }
}
