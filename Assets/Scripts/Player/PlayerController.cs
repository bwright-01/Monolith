using UnityEngine;
using UnityEngine.InputSystem;

using Core;

namespace Player {

    public class PlayerController : MonoBehaviour {
        Vector2 move;

        public VoidEventHandler OnFirePress = new VoidEventHandler();
        public VoidEventHandler OnMeleePress = new VoidEventHandler();

        // TODO: REMOVE
        [SerializeField] Audio.Sound.SingleSound testSound;

        private void Awake() {
            testSound.Init(this);
        }
        private void OnGUI() {
            GUILayout.TextField("Test Sound");
            if (GUILayout.Button("Play")) {
                testSound.Play();
            }
        }

        [SerializeField] EventChannelSO eventChannel;

        // TODO: ADD PAUSE EVENT HANDLING
        bool _isPaused = false;

        // public
        public Vector2 Move => move;

        void OnMove(InputValue value) {
            if (_isPaused) return;
            move = Vector2.ClampMagnitude(value.Get<Vector2>(), 1f);
        }

        void OnFire(InputValue value) {
            if (_isPaused) return;
            if (value.isPressed) OnFirePress.Invoke();
        }

        void OnMelee(InputValue value) {
            if (_isPaused) return;
            if (value.isPressed) OnMeleePress.Invoke();
        }

        void OnUse(InputValue value) {
            if (_isPaused) return;
            if (value.isPressed) {
                eventChannel.OnUseKeyPress.Invoke();
            } else {
                eventChannel.OnUseKeyRelease.Invoke();
            }
        }
    }
}

