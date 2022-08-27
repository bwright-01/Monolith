using UnityEngine;

using Core;

namespace Environment {

    public class LevelPortal : MonoBehaviour {

        [SerializeField] Collider2D[] collidersToDisable = new Collider2D[] { };
        [SerializeField] EventChannelSO eventChannel;

        bool hasTeleported;

        void OnTriggerEnter2D(Collider2D other) {
            if (hasTeleported) return;
            if (other.CompareTag("Player")) {
                hasTeleported = true;
                Teleport();
            }
        }

        void Teleport() {

            foreach (var col in collidersToDisable) {
                col.enabled = false;
            }

            eventChannel.OnStopMusic.Invoke();

            // TODO:
            // - fade to black
            // - load boss level
        }
    }
}
