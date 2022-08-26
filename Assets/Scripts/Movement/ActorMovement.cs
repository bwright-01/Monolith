using UnityEngine;

namespace Movement {

    public enum ActorMovementMode {
        Heading,
        Target,
    }

    public class ActorMovement : MonoBehaviour {

        [SerializeField] ActorMovementMode mode = ActorMovementMode.Heading;
        [SerializeField][Range(0f, 100f)] float maxSpeed = 5f;
        [SerializeField][Tooltip("How quickly the actor accelerates to max speed")][Range(0.001f, 2f)] float throttleUpTime = 0.3f;
        [SerializeField][Tooltip("How quickly the actor comes to a stop")][Range(0.001f, 2f)] float throttleDownTime = 0.05f;
        [SerializeField][Tooltip("How fast the actor can change directions")][Range(0.001f, 2f)] float speedDelta = 0.1f;
        [SerializeField][Tooltip("How fast the actor can rotate (degrees / sec)")][Range(0f, 1080f)] float rotateSpeed = 720f;

        [Space]
        [Space]

        [SerializeField] Vector2 aimOrientation = Vector2.up;

        // cached
        Rigidbody2D rb;

        // props
        float initialDrag;

        // state
        Transform moveTarget;
        Transform aimTarget;
        Vector2 heading;
        float throttle; // 0.0 to 1.0 --> what percentage of maxSpeed to move the actor
        Vector2 desiredVelocity;
        Vector2 prevVelocity;
        Vector2 currentForces;
        Quaternion desiredHeading;

        public void LookAt(Transform value) {
            aimTarget = value;
        }

        public void SetTarget(Transform value) {
            moveTarget = value;
            heading = Vector2.zero;
            mode = ActorMovementMode.Target;
        }

        public void SetHeading(Vector2 value) {
            heading = value;
            moveTarget = null;
            mode = ActorMovementMode.Heading;
        }

        public void Halt() {
            moveTarget = null;
            heading = Vector2.zero;
            mode = ActorMovementMode.Heading;
        }

        void Awake() {
            rb = GetComponent<Rigidbody2D>();
            initialDrag = rb.drag;
        }

        void Update() {
            SetThrottle();
            SetDrag();
            HandleReachTarget();
        }

        void FixedUpdate() {
            HandleRotate();
            HandleMove();
        }

        void SetDrag() {
            rb.drag = HasMoveTarget() ? 0f : initialDrag;
        }

        void SetThrottle() {
            throttle = HasMoveTarget() ? throttle + Time.deltaTime / throttleUpTime : throttle - Time.deltaTime / throttleDownTime;
            throttle = Mathf.Clamp01(throttle);
        }

        void HandleMove() {
            currentForces = rb.velocity - prevVelocity;
            desiredVelocity = GetHeadingToMoveTarget() * maxSpeed * throttle;
            rb.velocity = Vector2.MoveTowards(rb.velocity, desiredVelocity, speedDelta);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            rb.velocity += currentForces;
            prevVelocity = rb.velocity;
        }

        void HandleRotate() {
            if (!HasAimTarget() && !HasMoveTarget()) return;
            desiredHeading = Quaternion.Euler(0, 0, Vector2.SignedAngle(aimOrientation, GetHeadingToAimTarget()));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredHeading, rotateSpeed * Time.deltaTime);
        }

        void HandleReachTarget() {
            if (moveTarget == null) return;
            if (Vector2.Distance(moveTarget.position, transform.position) > float.Epsilon) return;
            Halt();
        }

        Vector2 GetHeadingToMoveTarget() {
            if (mode == ActorMovementMode.Heading) return heading;
            if (moveTarget == null) return Vector2.zero;
            return (moveTarget.position - transform.position).normalized;
        }

        Vector2 GetHeadingToAimTarget() {
            if (HasAimTarget()) return (aimTarget.position - transform.position).normalized;
            return GetHeadingToMoveTarget();
        }

        bool HasAimTarget() {
            return aimTarget != null;
        }

        bool HasMoveTarget() {
            if (mode == ActorMovementMode.Heading) return heading != Vector2.zero;
            return moveTarget != null;
        }
    }
}
