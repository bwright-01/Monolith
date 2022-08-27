using UnityEngine;

using Core;

namespace Player {

    [RequireComponent(typeof(Rigidbody2D))]

    public class PlayerMovement : MonoBehaviour {

        [SerializeField][Range(0f, 100f)] float maxSpeed = 5f;
        [SerializeField][Tooltip("How quickly the player accelerates to max speed after receiving input")][Range(0.001f, 2f)] float throttleUpTime = 0.1f;
        [SerializeField][Tooltip("How quickly the player comes to a stop after no input")][Range(0.001f, 2f)] float throttleDownTime = 0.05f;
        [SerializeField][Tooltip("How fast the player can change directions")][Range(0.001f, 2f)] float speedDelta = 0.1f;
        // [SerializeField][Tooltip("How fast the player can rotate (degrees / sec)")][Range(0f, 1080f)] float rotateSpeed = 720f;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        // cached
        PlayerController controller;
        Rigidbody2D rb;

        // props
        float initialDrag;

        // state
        float throttle; // 0.0 to 1.0 --> what percentage of maxSpeed to move the player
        Vector2 desiredVelocity;
        Vector2 prevVelocity;
        Vector2 currentForces;
        Quaternion desiredHeading;

        public bool HasMoveInput() {
            if (!enabled) return false;
            if (!controller.enabled) return false;
            return controller.Move.magnitude > float.Epsilon;
        }

        bool IsAiming() {
            if (!enabled) return false;
            if (!controller.enabled) return false;
            return controller.IsAiming;
        }

        void OnEnable() {
            eventChannel.OnAbilityUpgraded.Subscribe(OnAbilityUpgraded);
        }

        void OnDisable() {
            eventChannel.OnAbilityUpgraded.Unsubscribe(OnAbilityUpgraded);
            rb.drag = initialDrag;
        }

        void Awake() {
            controller = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
            initialDrag = rb.drag;
        }

        void Start() {
            CheckForUpgrades();
        }

        void Update() {
            SetThrottle();
            SetDrag();
        }

        void FixedUpdate() {
            HandleRotate();
            HandleMove();
        }

        void OnAbilityUpgraded(Game.UpgradeType upgradeType) {
            CheckForUpgrades();
        }

        void CheckForUpgrades() {
            if (Game.GameSystems.current.state.IsMovementUpgraded) {
                maxSpeed = Game.GameSystems.current.state.UpgradedMoveSpeed;
            }
        }

        void SetDrag() {
            rb.drag = HasMoveInput() ? 0f : initialDrag;
        }

        void SetThrottle() {
            throttle = HasMoveInput() ? throttle + Time.deltaTime / throttleUpTime : throttle - Time.deltaTime / throttleDownTime;
            throttle = Mathf.Clamp01(throttle);
        }

        void HandleMove() {
            currentForces = rb.velocity - prevVelocity;
            desiredVelocity = controller.Move * maxSpeed * throttle;
            rb.velocity = Vector2.MoveTowards(rb.velocity, desiredVelocity, speedDelta);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            rb.velocity += currentForces;
            prevVelocity = rb.velocity;
        }

        void HandleRotate() {
            if (!HasMoveInput()) return;
            if (IsAiming()) return;
            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, Utils.GetNearestCardinal(controller.Move))); ;
            // desiredHeading = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, Utils.GetNearestCardinal(controller.Move)));
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredHeading, rotateSpeed * Time.deltaTime);
        }
    }
}
