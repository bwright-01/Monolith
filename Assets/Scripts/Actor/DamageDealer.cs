using System.Collections.Generic;
using UnityEngine;

using Core;
using UnityEngine.Events;

namespace Actor {

    public class DamageDealer : MonoBehaviour {
        [SerializeField] bool debug;
        [SerializeField][Range(0f, 200f)] float damageAmount = 10f;
        [SerializeField][Range(0f, 200f)] float damageVariance = 4f;
        [SerializeField] bool makeFramerateIndependent;

        [Space]
        [Space]

        [SerializeField][Range(0f, 200f)] float impactForce = 0f;

        [Space]
        [Space]

        [SerializeField] bool ignoreParentGUID = true;
        [SerializeField] LayerMask ignoreLayers;
        [SerializeField] string ignoreTag;
        [SerializeField] List<Collider2D> ignoreColliders = new List<Collider2D>();

        [Space]
        [Space]

        [SerializeField] UnityEvent<int> OnHit;

        // props
        float damageMultiplier = 1f;

        // cached
        new Collider2D collider;
        iActor parentActor;

        // state
        System.Nullable<System.Guid> ignoreGUID;
        bool hitThisFrame;
        Dictionary<Collider2D, DamageReceiver> hitMap = new Dictionary<Collider2D, DamageReceiver>();
        DamageReceiver currentReceiver;

        public void SetIgnoreUUID(System.Nullable<System.Guid> uuid) {
            ignoreGUID = uuid;
        }

        public void SetIgnoreLayers(LayerMask layerMask) {
            ignoreLayers = layerMask;
        }

        public void SetIgnoreTag(string tag) {
            ignoreTag = tag;
        }

        public void SetDamageMultiplier(float value) {
            damageMultiplier = value;
        }

        void Awake() {
            parentActor = GetComponentInParent<iActor>();
            if (ignoreParentGUID && parentActor != null && parentActor.GUID() != null) {
                SetIgnoreUUID(parentActor.GUID());
            }
            collider = GetComponent<Collider2D>();
            IgnoreColliders();
        }

        void Update() {
            hitThisFrame = false;
        }

        void OnCollisionEnter2D(Collision2D other) {
            HandleCollision(other.collider);
        }

        void OnTriggerEnter2D(Collider2D other) {
            HandleCollision(other);
        }

        void HandleCollision(Collider2D other) {
            if (debug) Debug.Log($">>> collision occurred! hit: \"{Utils.FullGameObjectName(other.gameObject)}\"");

            if (!enabled) return;
            if (other == null) return;
            if (hitThisFrame) return;
            if (ignoreTag == other.tag) return;
            if (LayerUtils.LayerMaskContainsLayer(ignoreLayers, other.gameObject.layer)) return;
            if (parentActor != null && !parentActor.IsAlive()) return;

            OnHit.Invoke(other.gameObject.layer);

            currentReceiver = GetDamageReceiverFromCollider(other);

            if (currentReceiver == null) return;
            if (ignoreGUID != null && ignoreGUID == currentReceiver.guid) return;

            float damage = GetAppliedDamageAmount();
            if (currentReceiver.TakeDamage(damage, GetHeadingTowardsOtherCollider(other) * impactForce)) {
                hitThisFrame = true;
                if (parentActor != null) parentActor.OnDamageGiven(damage, !currentReceiver.IsAlive());
            } else {
                if (parentActor != null) parentActor.OnDamageGiven(0, false);
            }
        }

        DamageReceiver GetDamageReceiverFromCollider(Collider2D col) {
            if (hitMap.TryGetValue(col, out DamageReceiver value)) {
                return value;
            }
            currentReceiver = col.GetComponent<DamageReceiver>();
            hitMap[col] = currentReceiver;
            return currentReceiver;
        }

        Vector2 GetHeadingTowardsOtherCollider(Collider2D col) {
            return (col.transform.position - transform.position).normalized;
        }

        float GetAppliedDamageAmount() {
            return Utils.RandomVariance(damageAmount, damageVariance, 0f) * damageMultiplier * (makeFramerateIndependent ? Time.deltaTime : 1f);
        }

        void IgnoreColliders() {
            foreach (var ignoreCollider in ignoreColliders) {
                Physics2D.IgnoreCollision(collider, ignoreCollider);
            }
        }

    }
}
