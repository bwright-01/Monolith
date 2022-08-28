using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Core;
using System.Collections;

namespace UI {

    public class IntroDialog : MonoBehaviour {
        [SerializeField] string musicTrack = "Transition";

        [Space]
        [Space]

        [SerializeField] Canvas fadeToBlackCanvas;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] Button continueButton;
        [SerializeField] Image blackScreen;
        [SerializeField] Animator anim;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        public void OnGotoLevel() {
            Game.GameSystems.current.ResetForContinue();
            StopAllCoroutines();
            StartCoroutine(IFadeToNextScene("Level01"));
            eventChannel.OnStopMusic.Invoke();
        }

        public void GotoMainMenu() {
            StopAllCoroutines();
            StartCoroutine(IFadeToNextScene("MainMenu"));
            eventChannel.OnStopMusic.Invoke();
        }

        void Awake() {
            ActivateBlackScreen();
        }

        void Start() {
            eventChannel.OnPlayMusic.Invoke(musicTrack);
            StartCoroutine(IFadeIn());
        }

        void LoadNextScene(string sceneName = "Level01") {
            eventChannel.OnResetMusic.Invoke();
            SceneManager.LoadScene(sceneName);
        }

        void DeactivateBlackScreen() {
            fadeToBlackCanvas.gameObject.SetActive(false);
            fadeToBlackCanvas.enabled = false;
        }

        void ActivateBlackScreen() {
            fadeToBlackCanvas.gameObject.SetActive(true);
            fadeToBlackCanvas.enabled = true;
        }

        Timer fadeTimer = new Timer(TimerDirection.Increment);

        IEnumerator IFadeIn() {
            fadeTimer.SetDuration(fadeInTime);
            fadeTimer.Start();
            Color currentColor = blackScreen.color;
            while (fadeTimer.active) {
                blackScreen.color = Color.Lerp(currentColor, new Color(0, 0, 0, 0), fadeTimer.value);
                fadeTimer.Tick();
                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 0);
            DeactivateBlackScreen();
            continueButton.Select();

            if (anim != null) anim.enabled = true;
        }

        IEnumerator IFadeToNextScene(string sceneName = "Level01") {
            ActivateBlackScreen();
            fadeTimer.SetDuration(fadeInTime);
            fadeTimer.Start();
            Color currentColor = blackScreen.color;
            while (fadeTimer.active) {
                blackScreen.color = Color.Lerp(currentColor, Color.black, fadeTimer.value);
                fadeTimer.Tick();
                yield return null;
            }
            blackScreen.color = Color.black;
            LoadNextScene(sceneName);
        }
    }
}
