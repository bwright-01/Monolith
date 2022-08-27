using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using Core;
using System.Collections;

namespace Player {

    public class PlayerController : MonoBehaviour {

        [SerializeField][Range(0f, 1f)] float timeAimAfterFiring = 0.4f;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        Vector2 move;
        bool isAiming;

        public VoidEventHandler OnFirePress = new VoidEventHandler();
        public VoidEventHandler OnMeleePress = new VoidEventHandler();

        // TODO: ADD PAUSE EVENT HANDLING
        bool _isPaused = false;

        // public
        public Vector2 Move => move;
        public bool IsAiming => isAiming || recentlyFired.active;

        // cached
        PlayerInput input;
        InputSystemUIInputModule uiModule;

        Timer recentlyFired = new Timer();

        void Start() {
            input = GetComponent<PlayerInput>();
            input.SwitchCurrentActionMap("Player");
            input.enabled = true;
            recentlyFired.SetDuration(timeAimAfterFiring);
            StartCoroutine(IHackUiModule());
        }

        private void Update() {
            recentlyFired.SetDuration(timeAimAfterFiring);
            recentlyFired.Tick();
        }

        void OnMove(InputValue value) {
            if (_isPaused) return;
            move = Vector2.ClampMagnitude(value.Get<Vector2>(), 1f);
        }

        void OnFire(InputValue value) {
            if (_isPaused) return;
            if (value.isPressed) {
                recentlyFired.Start();
                OnFirePress.Invoke();
            }
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

        void OnAim(InputValue value) {
            if (_isPaused) return;
            isAiming = value.isPressed;
            recentlyFired.End();
        }

        // UNITY WHYYYYYYY??
        // This fixes a bug where keyboard events were not registered.
        IEnumerator IHackUiModule() {
            uiModule = Game.GameSystems.current.eventSystem.GetComponent<InputSystemUIInputModule>();
            uiModule.enabled = false;
            yield return new WaitForFixedUpdate();
            uiModule.enabled = true;
        }
    }
}

