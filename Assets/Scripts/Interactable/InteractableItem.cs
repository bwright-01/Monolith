using System.Collections;
using UnityEngine;

using Core;
using Audio.Sound;

namespace Interactable {

    public class InteractableItem : MonoBehaviour, iLocalSoundPlayer {

        [SerializeField] bool isInteractable = true;
        [SerializeField] InteractableType interactableType;
        [SerializeField][Range(0f, 2f)] float useDuration = 1f;

        [Space]

        [SerializeField] SpriteRenderer mainSprite;
        [SerializeField] ParticleSystem particleFx;
        [SerializeField] InteractableProgressBar progressBar;
        [SerializeField] InteractableTooltip tooltip;
        [SerializeField] InteractableTooltip notAllowedTooltip;

        [Space]
        [Header("Health Type")]
        [SerializeField] float hpGainedOnPickup;

        // [Space]
        // [Header("Door Type")]
        // [SerializeField] Door door;

        [Space]

        [SerializeField] EventChannelSO eventChannel;

        // props
        // VoidEventHandler OnUseAction = new VoidEventHandler();

        // state
        bool isUseActionInvoked;
        bool wasInteractable = true;
        bool triggerActive;
        Timer useTimer = new Timer(TimerDirection.Increment);
        Coroutine ieUse;

        public event StringEvent OnPlaySound;
        public void PlaySound(string soundName) {
            if (OnPlaySound != null) OnPlaySound.Invoke(soundName);
        }

        void OnUseAction() {
            if (isUseActionInvoked) return;
            isUseActionInvoked = true;
            triggerActive = false;

            PlaySound("UseComplete");

            switch (interactableType) {
                case InteractableType.Health:
                    eventChannel.OnGainHealth.Invoke(hpGainedOnPickup);
                    break;
            }

            HideTooltip();
            HideProgressBar();
            HideSprite();

            if (particleFx != null) particleFx.Play();

            switch (interactableType) {
                case InteractableType.Health:
                default:
                    Destroy(gameObject, 5f);
                    break;
            }
        }

        void OnEnable() {
            eventChannel.OnUseKeyPress.Subscribe(OnUseKeyPress);
            eventChannel.OnUseKeyRelease.Subscribe(OnUseKeyRelease);
        }

        void OnDisable() {
            eventChannel.OnUseKeyPress.Unsubscribe(OnUseKeyPress);
            eventChannel.OnUseKeyRelease.Unsubscribe(OnUseKeyRelease);
        }

        void OnUseKeyPress() {
            if (ieUse != null) StopCoroutine(ieUse);
            if (isInteractable) {
                PlaySound("UseStart");
                ieUse = StartCoroutine(Use());
            } else {
                PlaySound("UseError");
            }
        }

        void OnUseKeyRelease() {
            if (ieUse != null) StopCoroutine(ieUse);
            HideProgressBar();
        }

        void Start() {
            wasInteractable = isInteractable;
            HideTooltip();
            HideProgressBar();
        }

        void Update() {
            // we need to update the tooltip in case isInteractable state changes while the tooltip is showing
            if (isInteractable != wasInteractable) {
                HideTooltip();
                ShowTooltip();
            }
            wasInteractable = isInteractable;
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (isUseActionInvoked) return;
            if (other.CompareTag("Player")) {
                triggerActive = true;
                ShowTooltip();
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            if (isUseActionInvoked) return;
            if (other.CompareTag("Player")) {
                triggerActive = false;
                HideTooltip();
                HideProgressBar();
            }
        }

        void HideSprite() {
            if (mainSprite != null) mainSprite.gameObject.SetActive(false);
        }

        void ShowTooltip() {
            if (isInteractable) {
                if (tooltip != null) tooltip.gameObject.SetActive(true);
            } else {
                if (notAllowedTooltip != null) notAllowedTooltip.gameObject.SetActive(true);
            }
        }

        void HideTooltip() {
            if (tooltip != null) tooltip.gameObject.SetActive(false);
            if (notAllowedTooltip != null) notAllowedTooltip.gameObject.SetActive(false);
        }

        void ShowProgressBar() {
            if (progressBar != null) progressBar.gameObject.SetActive(true);
        }

        void HideProgressBar() {
            if (progressBar != null) progressBar.gameObject.SetActive(false);
        }

        void SetProgressBarValue(float value) {
            if (progressBar != null) progressBar.SetValue(value);
        }

        IEnumerator Use() {
            if (triggerActive && !isUseActionInvoked) {
                ShowProgressBar();

                useTimer.SetDuration(useDuration);
                useTimer.Start();

                while (useTimer.active) {
                    useTimer.Tick();
                    SetProgressBarValue(useTimer.value);
                    yield return null;
                }

                OnUseAction();
            }
            ieUse = null;
        }
    }
}

