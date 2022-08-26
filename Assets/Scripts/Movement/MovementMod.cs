using Core;
using UnityEngine;

namespace Movement {

    enum MovementModType {
        Sine,
    }

    public class MovementMod : MonoBehaviour {

        [SerializeField] bool debug;
        [SerializeField] MovementModType modType;

        [Space]

        [SerializeField][Range(0.01f, 5f)] float freqX = 1f;
        [SerializeField][Range(0f, 20f)] float magX;

        [Space]

        [SerializeField][Range(0.01f, 5f)] float freqY = 1f;
        [SerializeField][Range(0f, 20f)] float magY;

        // cached
        ActorMovement movement;

        // state
        Vector2 offset;
        float t;

        void Awake() {
            movement = GetComponent<ActorMovement>();
        }

        void Update() {
            HandleCalcOffset();
            movement.SetOffset(offset);
        }

        void HandleCalcOffset() {
            if (magX > float.Epsilon && freqX > float.Epsilon) {
                offset.x = Mathf.Cos(t / freqX) * magX;
            } else {
                offset.x = 0;
            }

            if (magY > float.Epsilon && freqY > float.Epsilon) {
                offset.y = Mathf.Sin(t / freqY) * magY;
            } else {
                offset.y = 0;
            }
            t += Time.deltaTime;
        }

        void OnDrawGizmos() {
            if (!debug) return;
            Utils.DebugDrawRect(transform.position + (Vector3)offset, 0.5f, Utils.Transparentize(Color.yellow, 0.7f));
        }
    }
}
