using UnityEngine;


namespace Enemy {
    namespace Parts {

        public class CentipedeSegment : MonoBehaviour {
            [SerializeField][Range(0f, 1080f)] float rotateSpeed = 90f;

            // cached
            Rigidbody2D rb;
            Transform parentTransform;
            float initialDistanceToParent;

            // state
            bool isInitialized = false;

            private void Awake() {
                rb = GetComponent<Rigidbody2D>();
                parentTransform = transform.parent;
                initialDistanceToParent = Vector2.Distance(transform.position, parentTransform.position);
                rb.simulated = false;
            }

            public void Init() {
                transform.SetParent(null);
                transform.up = GetHeadingToParent();
                rb.simulated = true;
                isInitialized = true;
            }

            public void HandleMove(float distance) {
                if (!enabled || !isInitialized) return;
                transform.position += transform.up * distance;
                transform.position = parentTransform.position + GetHeadingFromParent() * initialDistanceToParent;
                transform.up = Vector2.MoveTowards(transform.up, GetHeadingToParent(), rotateSpeed * Mathf.Clamp01(distance) * Time.deltaTime);
            }

            Vector3 GetHeadingToParent() {
                return (parentTransform.position - transform.position).normalized;
            }

            Vector3 GetHeadingFromParent() {
                return (transform.position - parentTransform.position).normalized;
            }
        }
    }
}
