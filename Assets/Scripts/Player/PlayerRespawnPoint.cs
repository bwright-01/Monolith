
using UnityEngine;

namespace Player {

    public class PlayerRespawnPoint : MonoBehaviour {

        void Awake() {
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sprite in sprites) {
                sprite.enabled = false;
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                Game.GameSystems.current.state.SetRespawnPoint(transform.position);
            }
        }
    }
}
