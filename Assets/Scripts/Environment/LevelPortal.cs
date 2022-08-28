using UnityEngine;

using Core;
using UnityEngine.SceneManagement;
using System.Collections;
using Audio.Sound;

namespace Environment {

    public class LevelPortal : MonoBehaviour {

        [SerializeField] Collider2D[] collidersToDisable = new Collider2D[] { };
        [SerializeField] EventChannelSO eventChannel;

        [SerializeField] SingleSound portalSound;
        [SerializeField] ParticleSystem portalFX;
        [SerializeField] float timeToTeleport = 2f;

        bool hasTeleported;

        private void Awake() {
            portalSound.Init(this);
        }

        void OnTriggerEnter2D(Collider2D other) {
            if (hasTeleported) return;
            if (other.CompareTag("Player")) {
                hasTeleported = true;
                Teleport();
            }
        }

        void Teleport() {

            foreach (var col in collidersToDisable) {
                col.enabled = false;
            }

            eventChannel.OnStopMusic.Invoke();
            StartCoroutine(IFadeToNextScene());

            // TODO:
            // - fade to black
        }

        void LoadNextScene() {
            eventChannel.OnResetMusic.Invoke();
            SceneManager.LoadScene("Level02Boss");
        }

        IEnumerator IFadeToNextScene() {
            portalSound.Play();
            if (portalFX != null) portalFX.Play();
            yield return new WaitForSeconds(timeToTeleport);
            LoadNextScene();
        }
    }
}
