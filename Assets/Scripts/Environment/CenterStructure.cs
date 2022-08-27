
using UnityEngine;

using Core;

namespace Environment {

    public class CenterStructure : MonoBehaviour {
        [SerializeField] SpriteRenderer shieldSprite;
        [SerializeField] Collider2D shieldCollider;
        [SerializeField] EventChannelSO eventChannel;

        Monolith monolith;

        void OnEnable() {
            eventChannel.OnAllMonolithsDestroyed.Subscribe(OnAllMonolithsDestroyed);
        }

        void OnDisable() {
            eventChannel.OnAllMonolithsDestroyed.Unsubscribe(OnAllMonolithsDestroyed);
        }

        void Awake() {
            monolith = GetComponent<Monolith>();
        }

        void OnAllMonolithsDestroyed() {
            monolith.PanToMonolith(() => {
                shieldSprite.enabled = false;
                shieldCollider.enabled = false;
            });
        }
    }
}
