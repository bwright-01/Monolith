using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {

    public class PlayerController : MonoBehaviour {
        Vector2 move;
        bool isFirePressed;
        bool isFire2Pressed;

        // TODO: ADD PAUSE EVENT HANDLING
        bool _isPaused = false;

        public Vector2 Move { get => move; set => move = value; }
        public bool IsFirePressed { get => isFirePressed; set => isFirePressed = value; }
        public bool IsFire2Pressed { get => isFire2Pressed; set => isFire2Pressed = value; }

        void OnMove(InputValue value) {
            if (_isPaused) return;
            move = Vector2.ClampMagnitude(value.Get<Vector2>(), 1f);
        }

        void OnFire(InputValue value) {
            if (_isPaused) return;
            isFirePressed = value.isPressed;
        }

        void OnFireSecondary(InputValue value) {
            if (_isPaused) return;
            isFire2Pressed = value.isPressed;
        }
    }
}

