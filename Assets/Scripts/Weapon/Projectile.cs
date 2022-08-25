using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Core;
using System;
using Audio.Sound;

namespace Weapons {

    [RequireComponent(typeof(Actor.Health))]
    [RequireComponent(typeof(Actor.DamageReceiver))]
    [RequireComponent(typeof(Actor.DamageDealer))]
    public class Projectile : MonoBehaviour, Actor.iActor, iLocalSoundPlayer {
        [Header("General Settings")]
        [Space]
        [SerializeField] float moveSpeed = 5f;
        [SerializeField] float lifetime = 10f;
        [SerializeField] Vector3 initialHeading = Vector3.up;
        [SerializeField] int numCollisionsMax = 1;
        [SerializeField][Range(0f, 1f)] float ricochetProbability = 0.2f;
        [SerializeField][Range(0f, 180f)] float ricochetAngle = 60f;
        [SerializeField][Range(0f, 90f)] float ricochetVariance = 20f;
        [SerializeField] float outOfRange = 20f;

        [Header("Effects")]
        [Space]
        [SerializeField] float impactLifetime = 1f;
        [SerializeField] GameObject impactFX;

        // components
        Actor.Health health;
        BoxCollider2D box;
        CircleCollider2D circle;
        CapsuleCollider2D capsule;
        SpriteRenderer sr;
        TrailRenderer[] trails;
        Rigidbody2D rb;

        // cached
        Vector3 startingPosition;
        Vector3 heading;
        Vector3 velocity;
        Transform target;
        UnityEvent OnDamageDealt;
        ParticleSystem particleFx;

        // state
        float timeAlive = 0;
        float height = 0.5f;
        int numCollisions = 0;

        System.Guid guid = new System.Guid();

        public event StringEvent OnPlaySound;
        public void PlaySound(string soundName) {
            if (OnPlaySound != null) OnPlaySound(soundName);
        }

        public Guid Guid() {
            return guid;
        }

        public bool IsAlive() {
            return health.IsAlive();
        }

        public bool TakeDamage(float damage, Vector2 force) {
            return (health.TakeDamage(damage));
        }

        public void OnDamageTaken(float damage, float hp) {
            OnDeath();
        }

        public void OnDeath() { OnDeath(1000f, -1000f); }
        public void OnDeath(float damage, float hp) {
            health.SetIsAlive(false);
            Cleanup();
        }

        public void OnDamageGiven(float damage, bool wasKilled) {
            if (!IsAlive()) return;
            numCollisions++;
            PlaySound("ProjectileImpact");
            bool ShouldRichochet = UnityEngine.Random.Range(0f, 1f) <= ricochetProbability;
            if (wasKilled || numCollisions >= numCollisionsMax || !ShouldRichochet) {
                AddImpactFx();
                OnDeath();
            } else {
                Ricochet();
            }
        }

        public void SetTarget(Transform _target) {
            target = _target;
        }

        void OnEnable() {
            if (health != null) {
                health.OnDamageTaken.Subscribe(OnDamageTaken);
                health.OnDeath.Subscribe(OnDeath);
            }
        }

        void OnDisable() {
            if (health != null) {
                health.OnDamageTaken.Unsubscribe(OnDamageTaken);
                health.OnDeath.Unsubscribe(OnDeath);
            }
        }

        void Init() {
            height = CalcHeight();
            heading = initialHeading;
            // point heading in direction of rotation
            heading = Quaternion.AngleAxis(transform.rotation.eulerAngles.z, Vector3.forward) * heading;
            velocity = heading * moveSpeed;
            target = null;
            startingPosition = transform.position;
        }

        void Start() {
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponentInChildren<SpriteRenderer>();
            trails = GetComponentsInChildren<TrailRenderer>();
            box = GetComponent<BoxCollider2D>();
            circle = GetComponent<CircleCollider2D>();
            capsule = GetComponent<CapsuleCollider2D>();
            health = GetComponent<Actor.Health>();
            particleFx = GetComponentInChildren<ParticleSystem>();
            Init();
        }

        void Update() {
            if (!IsAlive()) return;
            UpdateHeading();
            if (rb == null) MoveViaTransform();
            timeAlive += Time.deltaTime;
            if (timeAlive > lifetime) OnDeath();
            if ((transform.position - startingPosition).magnitude > outOfRange) OnDeath();
        }

        void OnBecameInvisible() {
            OnDeath();
        }

        void FixedUpdate() {
            if (rb != null) MoveViaRigidbody();
        }

        void UpdateHeading() {
            if (!IsAlive()) {
                velocity *= 0.05f;
                return;
            }
            velocity = Vector3.MoveTowards(velocity, heading * moveSpeed, 2f * moveSpeed * Time.fixedDeltaTime);
        }

        void MoveViaTransform() {
            if (!IsAlive()) return;
            transform.position += velocity * Time.deltaTime;
        }

        void MoveViaRigidbody() {
            if (!IsAlive()) return;
            rb.velocity = velocity;
        }

        void Ricochet() {
            if (!IsAlive()) return;
            PlaySound("ProjectileRicochet");
            heading = -heading;
            Quaternion ricochet = GetRicochet();
            heading = (ricochet * heading).normalized;
            velocity = heading * moveSpeed * 2f;
            transform.rotation = transform.rotation * ricochet * Quaternion.Euler(0f, 0f, 180f);
            transform.position += heading * height;
        }

        void AddImpactFx() {
            if (impactFX == null) return;
            Destroy(Instantiate(impactFX, transform.position, transform.rotation * Quaternion.Euler(0, 0, 180f)), impactLifetime);
        }

        void Cleanup() {
            if (sr != null) sr.enabled = false;
            if (particleFx != null) particleFx.Stop();
            if (trails != null) {
                foreach (var tr in trails) {
                    if (tr != null) tr.enabled = false;
                }
            }
            Destroy(gameObject, 5f);
        }

        float CalcHeight() {
            if (box != null) {
                return box.size.y;
            }
            if (circle != null) {
                return circle.radius * 2;
            }
            if (capsule) {
                return capsule.size.y;
            }
            return height;
        }

        Quaternion GetRicochet() {
            return Quaternion.Euler(
                0,
                0,
                ricochetAngle *
                // random [-1, 1]
                ((float)UnityEngine.Random.Range(0, 2) - 0.5f) * 2f +
                // variance
                UnityEngine.Random.Range(-ricochetVariance, ricochetVariance));
        }
    }
}
