using System.Collections.Generic;
using UnityEngine;

using Core;

// USAGE NOTES:
// - see LocalSoundPlayer usage notes

namespace Audio {
    namespace Sound {

        public class LocalSound : MonoBehaviour {
            const string INVALID_SOUND_NAME = "__INVALID_SOUND_NAME__";

            [SerializeField][Range(0f, 0.5f)] protected float volumeVariance = 0.1f;
            [SerializeField][Range(0f, 0.5f)] protected float pitchVariance = 0.1f;
            [SerializeField][Range(0f, 0.03f)] protected float delayVariance = 0f;
            [SerializeField][Range(0f, 5f)] protected float courseDelayVariance = 0f;
            [SerializeField] bool oneShot = false;
            [SerializeField] bool playOnAwake = false;

            [Space]

            [SerializeField][Range(0f, 2f)] float volumeMultiplier = 1f;
            [SerializeField][Range(0f, 2f)] float pitchMultiplier = 1f;

            [Space]

            [SerializeField]
            [Tooltip("Max number of simultaneous sounds with the same name that should be allowed to play. Set to 99 to disable.")]
            [Range(1, 99)] int maxSimultaneousClips = 99;
            [SerializeField]
            [Tooltip("Time threshold for determining whether sounds are playing simultaneously or not (seconds).")]
            [Range(0f, 1f)] float simulPlayThreshold = 0.05f;

            // simul play state
            static Dictionary<string, double> simulPlayLookup = new Dictionary<string, double>(100);
            double simulPlayStepAmount = 0.0;

            // props
            string soundName = INVALID_SOUND_NAME;
            AudioSource[] sources;
            AudioSource currentSource;

            // state
            int currentClipIndex;
            float currentVolumeMultiplier = 1f;
            float currentPitchMultiplier = 1f;

            // public
            public new string name => gameObject.name;

            public void Play() {
                if (!ValidateSound()) return;
                if (!CanPlaySimultaneousSound()) return;
                if (oneShot) {
                    UpdateVariance();
                    currentSource.PlayOneShot(currentSource.clip);
                } else {
                    UpdateVariance();
                    currentSource.Stop();
                    currentSource.PlayDelayed(UnityEngine.Random.Range(0f, delayVariance + courseDelayVariance));
                }
            }

            public void PlayAtLocation(Vector3 location) {
                if (!ValidateSound()) return;
                UpdateVariance();
                AudioSource.PlayClipAtPoint(currentSource.clip, location, currentSource.volume);
            }

            public void Stop() {
                if (!ValidateSound()) return;
                currentSource.Stop();
            }

            void Awake() {
                soundName = gameObject.name;
                sources = GetComponentsInChildren<AudioSource>();
                foreach (var source in sources) source.playOnAwake = false;
                InitSimultaneousSoundLookup();
                if (playOnAwake) Play();
            }

            void InitSimultaneousSoundLookup() {
                if (maxSimultaneousClips >= 99) return;
                if (!simulPlayLookup.ContainsKey(soundName)) simulPlayLookup[soundName] = 0.0;
            }

            bool CanPlaySimultaneousSound() {
                if (maxSimultaneousClips >= 99) return true;
                InitSimultaneousSoundLookup();
                if (AudioSettings.dspTime + simulPlayThreshold <= simulPlayLookup[soundName]) return false;
                if (simulPlayLookup[soundName] < AudioSettings.dspTime) {
                    simulPlayLookup[soundName] = AudioSettings.dspTime;
                }
                simulPlayLookup[soundName] += simulPlayStepAmount;

                return true;
            }

            void UpdateVariance() {
                currentClipIndex = UnityEngine.Random.Range(0, sources.Length);
                currentSource = sources[currentClipIndex];
                currentVolumeMultiplier = Utils.RandomVariance(volumeMultiplier, volumeVariance, 0f, 1f);
                currentPitchMultiplier = Utils.RandomVariance(pitchMultiplier, pitchVariance, 0f, 1f);
            }

            bool ValidateSound() {
                if (soundName == INVALID_SOUND_NAME) return false;
                if (sources.Length == 0) return false;
                return true;
            }
        }
    }
}
