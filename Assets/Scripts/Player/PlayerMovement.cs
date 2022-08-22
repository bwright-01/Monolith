using UnityEngine;

namespace Player {

    public class PlayerMovement : MonoBehaviour {

        [SerializeField][Range(0f, 100f)] float maxSpeed = 5f;
        [SerializeField][Tooltip("How quickly the player accelerates to max speed after receiving input")][Range(0.001f, 2f)] float throttleUpTime = 0.1f;
        [SerializeField][Tooltip("How quickly the player comes to a stop after no input")][Range(0.001f, 2f)] float throttleDownTime = 0.05f;
        [SerializeField][Tooltip("How fast the player can change directions")][Range(0.001f, 2f)] float speedDelta = 0.1f;
        // [SerializeField][Tooltip("How fast the player can rotate (degrees / sec)")][Range(0.001f, 2f)] float rotateSpeed = 180f;

        // cached
        PlayerController controller;
        Rigidbody2D rb;

        // state
        bool hasMoveInput;
        float throttle; // 0.0 to 1.0 --> what percentage of maxSpeed to move the player
        Vector2 desiredVelocity;
        Vector2 currentVelocity;
        Quaternion desiredHeading;

        void Awake() {
            controller = GetComponent<PlayerController>();
            rb = GetComponent<Rigidbody2D>();
        }

        void Update() {
            SetThrottle();
        }

        void FixedUpdate() {
            HandleRotate();
            HandleMove();
        }

        void SetThrottle() {
            hasMoveInput = controller.Move.magnitude > float.Epsilon;
            throttle = hasMoveInput ? throttle + Time.deltaTime / throttleUpTime : throttle - Time.deltaTime / throttleDownTime;
            throttle = Mathf.Clamp01(throttle);
        }

        void HandleMove() {
            desiredVelocity = controller.Move * maxSpeed * throttle;
            rb.velocity = Vector2.MoveTowards(rb.velocity, desiredVelocity, speedDelta);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            Debug.Log("-----");
            Debug.Log($"throttle={throttle} move={controller.Move} vel={rb.velocity}");
        }

        void HandleRotate() {
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredHeading, rotateSpeed * Time.deltaTime);
            desiredHeading = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rb.velocity));
            transform.rotation = desiredHeading;
        }
    }
}
