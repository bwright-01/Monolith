
using UnityEngine;

using Core;
using System.Collections.Generic;

namespace Environment {

    public enum HazardType {
        Lava,
        Acid,
    }

    public class Hazard : MonoBehaviour {

        [SerializeField] HazardType hazardType;
        [SerializeField][Range(0f, 100f)] float damagePerSecond;
        [SerializeField] bool instakill;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        Dictionary<Collider2D, Actor.DamageReceiver> hitMap = new Dictionary<Collider2D, Actor.DamageReceiver>();
        Actor.DamageReceiver currentReceiver;

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                eventChannel.OnHazardEnter.Invoke(hazardType);
            }
        }

        void OnTriggerStay2D(Collider2D other) {
            ApplyDamage(other);
        }

        void OnTriggerExit2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                eventChannel.OnHazardExit.Invoke(hazardType);
            }
        }

        void ApplyDamage(Collider2D other) {
            currentReceiver = GetDamageReceiverFromCollider(other);
            if (currentReceiver == null) return;
            float damage = instakill ? Actor.Constants.INSTAKILL : damagePerSecond * Time.deltaTime;
            currentReceiver.TakeDamage(damage, Vector2.zero);
        }

        Actor.DamageReceiver GetDamageReceiverFromCollider(Collider2D col) {
            if (hitMap.TryGetValue(col, out Actor.DamageReceiver value)) {
                return value;
            }
            currentReceiver = col.GetComponent<Actor.DamageReceiver>();
            hitMap[col] = currentReceiver;
            return currentReceiver;
        }
    }
}
