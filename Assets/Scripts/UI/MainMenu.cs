using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Core;

namespace UI {

    public class MainMenu : MonoBehaviour {

        [SerializeField] string musicTrack = "PianoA";

        [Space]
        [Space]

        [SerializeField] Canvas fadeToBlackCanvas;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] GameObject buttonContainer;
        [SerializeField] Button startButton;
        [SerializeField] Image blackScreen;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        Timer fadeTimer = new Timer(TimerDirection.Increment);

        void Awake() {
            ActivateBlackScreen();
            buttonContainer.SetActive(false);
            Time.timeScale = 1f;
        }

        void Start() {
            Game.GameSystems.current.ResetGameState();
            eventChannel.OnPlayMusic.Invoke(musicTrack);
            StartCoroutine(IFadeIn());
        }

        public void OnStartGame() {
            StopAllCoroutines();
            StartCoroutine(IFadeToNextScene());
        }

        public void OnQuitGame() {
            Application.Quit();
        }

        void LoadNextScene() {
            SceneManager.LoadScene("Intro");
        }

        void DeactivateBlackScreen() {
            fadeToBlackCanvas.gameObject.SetActive(false);
            fadeToBlackCanvas.enabled = false;
        }

        void ActivateBlackScreen() {
            fadeToBlackCanvas.gameObject.SetActive(true);
            fadeToBlackCanvas.enabled = true;
        }

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
            buttonContainer.SetActive(true);
            startButton.Select();
        }

        IEnumerator IFadeToNextScene() {
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
            LoadNextScene();
        }
    }
}
