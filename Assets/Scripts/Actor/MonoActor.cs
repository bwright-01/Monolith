
using UnityEngine;

using Audio.Sound;
using Core;

namespace Actor {

    [RequireComponent(typeof(Actor.Health))]

    public abstract class MonoActor : MonoBehaviour, iActor {
        // [SerializeField] bool debug = false;

        // [Space]
        // [Space]

        [SerializeField] SingleSound damageSound;
        [SerializeField] SingleSound deathSound;

        [Space]
        [Space]

        [SerializeField] bool hideOnDeath = true;
        [SerializeField] bool destroyOnDeath;
        [SerializeField] MonoActor[] killActorsOnDeath = new MonoActor[] { };
        [SerializeField] GameObject[] disableObjectsOnDeath = new GameObject[] { };

        [Space]
        [Space]

        [SerializeField] protected EventChannelSO eventChannel;

        // props
        System.Guid guid = System.Guid.NewGuid();

        // cached
        Rigidbody2D rb;
        SpriteRenderer sr;
        Health health;
        DamageFlash damageFlash;

        // cached - enemy specific
        Movement.ActorMovement actorMovement;
        Movement.MovementMod movementMod;
        Movement.Wander wander;
        Enemy.EnemyAttack enemyAttack;
        Enemy.EnemyPatrol enemyPatrol;
        Enemy.EnemySight enemySight;
        TheKiwiCoder.BehaviourTreeRunner behaviourTree;

        public Health actorHealth => health;
        public float healthPercentage => health != null ? health.healthPercentage : 0;

        void OnDestroy() {
            CleanupOthers();
            StopAllCoroutines();
            damageSound.Unload();
            deathSound.Unload();
        }

        protected void SubscribeToEvents() {
            health.OnHealthGained.Subscribe(OnHealthGained);
            health.OnDamageTaken.Subscribe(OnDamageTaken);
            health.OnDeath.Subscribe(OnDeath);
        }

        protected void UnsubscribeFromEvents() {
            health.OnHealthGained.Unsubscribe(OnHealthGained);
            health.OnDamageTaken.Unsubscribe(OnDamageTaken);
            health.OnDeath.Unsubscribe(OnDeath);
        }

        protected void Init() {
            Layer.Init();

            rb = GetComponent<Rigidbody2D>();
            sr = GetComponentInChildren<SpriteRenderer>();
            health = GetComponent<Health>();
            damageFlash = GetComponent<DamageFlash>();

            damageSound.Init(this);
            deathSound.Init(this);

            if (Layer.Enemy.Equals(gameObject.layer)) {
                actorMovement = GetComponent<Movement.ActorMovement>();
                movementMod = GetComponent<Movement.MovementMod>();
                wander = GetComponent<Movement.Wander>();
                enemyAttack = GetComponent<Enemy.EnemyAttack>();
                enemyPatrol = GetComponent<Enemy.EnemyPatrol>();
                enemySight = GetComponent<Enemy.EnemySight>();
                behaviourTree = GetComponent<TheKiwiCoder.BehaviourTreeRunner>();
            }
        }

        public System.Guid GUID() {
            return guid;
        }

        public bool IsAlive() {
            if (health == null) return false;
            return health.IsAlive();
        }

        public abstract Region GetRegion();

        public bool GainHealth(float amount) {
            return health.GainHealth(amount);
        }

        public bool TakeDamage(float damage, Vector2 force) {
            if (rb != null) rb.AddForce(force, ForceMode2D.Impulse);
            return health.TakeDamage(damage);
        }

        public abstract void OnHealthGained(float amount, float hp);

        public abstract void OnDamageTaken(float damage, float hp);

        public abstract void OnDamageGiven(float damage, bool wasKilled);

        public abstract void OnDeath(float damage, float hp);

        protected void CommonDamageActions() {
            if (damageFlash != null) damageFlash.StartFlashing();
            damageSound.Play();
        }

        protected void CommonDeathActions() {
            deathSound.Play();

            if (hideOnDeath && sr != null) sr.enabled = false;

            if (behaviourTree != null) behaviourTree.enabled = false;
            if (enemyAttack != null) enemyAttack.enabled = false;
            if (enemyPatrol != null) enemyPatrol.enabled = false;
            if (enemySight != null) enemySight.enabled = false;
            if (wander != null) wander.enabled = false;
            if (movementMod != null) movementMod.enabled = false;
            if (actorMovement != null) actorMovement.enabled = false;

            CleanupOthers();

            if (destroyOnDeath) Destroy(gameObject);
        }

        void CleanupOthers() {
            foreach (var obj in killActorsOnDeath) if (obj != null) obj.TakeDamage(Constants.INSTAKILL, Vector2.zero);
            foreach (var obj in disableObjectsOnDeath) if (obj != null) obj.SetActive(false);
        }

        // void OnGUI() {
        //     if (!debug) return;
        //     GUILayout.TextField(gameObject.name);
        //     if (GUILayout.Button("Take Damage")) {
        //         TakeDamage(10f, Vector2.zero);
        //     }
        // }
    }
}

