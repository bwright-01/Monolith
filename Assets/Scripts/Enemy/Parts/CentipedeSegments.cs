using UnityEngine;

namespace Enemy {
    namespace Parts {

        public class CentipedeSegments : MonoBehaviour {

            // props
            CentipedeSegment[] segments;

            // state
            float distanceMoved;
            Vector2 lastPosition;

            void OnEnable() {
                foreach (var segment in segments) if (segment != null) segment.gameObject.SetActive(true);
            }

            void OnDisable() {
                foreach (var segment in segments) if (segment != null) segment.gameObject.SetActive(false);
            }

            void Awake() {
                segments = GetComponentsInChildren<CentipedeSegment>();
                lastPosition = transform.position;
            }

            void Start() {
                for (int i = segments.Length - 1; i >= 0; i--) {
                    segments[i].Init();
                }
            }

            void Update() {
                distanceMoved = Vector2.Distance(transform.position, lastPosition);
                foreach (var segment in segments) if (segment != null) segment.HandleMove(distanceMoved);
                lastPosition = transform.position;
            }
        }
    }
}
