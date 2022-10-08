using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Core;

namespace Audio {

    namespace Sound {

        [CreateAssetMenu(fileName = "SingleSound", menuName = "ScriptableObjects/SingleSound", order = 0)]
        public class SingleSound : BaseSound {
            [Space]
            [SerializeField][Range(0f, 0.5f)] protected float volumeVariance = 0.1f;
            [SerializeField][Range(0f, 0.5f)] protected float pitchVariance = 0.1f;
            [SerializeField][Range(0f, 0.03f)] protected float delayVariance = 0.01f;
            [SerializeField][Range(0f, 5f)] protected float courseDelayVariance = 0f;
            [SerializeField] AudioClip[] clips;
            [SerializeField] bool oneShot = false;
            [SerializeField] bool loops = false;
            [SerializeField][Range(1, 99)] int maxSimultaneousClips = 99;
            [SerializeField][Range(0f, 1f)] float simulPlayThreshold = 0.05f;

            // getters
            // public AudioClip Clip => clips[currentClipIndex];
            // public override bool isPlaying => source != null && source.isPlaying;
            // public override bool hasClip => clips.Length > 0;
            // public override bool hasSource => source != null;

            // state
            int currentClipIndex = 0;

            // simultaneous clips - keep track of timestamps
            static Dictionary<string, double> simulPlayLookup = new Dictionary<string, double>(100);
            double simulPlayStepAmount = 0.0;

            Dictionary<MonoBehaviour, AudioSource> soundMap = new Dictionary<MonoBehaviour, AudioSource>();

            public override void Init(MonoBehaviour script, AudioMixerGroup mix = null, AudioSource existingSource = null) {
                if (existingSource != null && existingSource.clip != null) {
                    clips = new AudioClip[1] { existingSource.clip };
                }
                if (clips.Length == 0) return;
                // nullSound.SetSource(gameObject.AddComponent<AudioSource>(), soundFXMix);
                AudioSource source = existingSource != null ? existingSource : script.gameObject.AddComponent<AudioSource>();
                if (existingSource == null) {
                    source.clip = clips[0];
                    source.loop = loops;
                }
                source.volume = volume;
                source.pitch = pitch;
                source.playOnAwake = false;
                source.outputAudioMixerGroup = mix;
                source.ignoreListenerPause = ignoreListenerPause;

                // 3d settings
                source.dopplerLevel = dopplerLevel;
                source.spread = spread;
                source.spatialBlend = spatialBlend;
                source.minDistance = minFalloffDistance;
                source.maxDistance = maxFalloffDistance;

                // set up player to play the sound on the current gameObject, not the scriptableobject :facepalm
                soundMap[script] = source;

                // simultaneous play / max voices
                simulPlayStepAmount = (double)(simulPlayThreshold / maxSimultaneousClips);

                InitSimultaneousSoundLookup();
            }

            public override void Unload(MonoBehaviour script) {
                if (!ValidateSound(script)) return;
                Destroy(soundMap[script]);
                soundMap[script] = null;
                soundMap.Remove(script);
            }

            public override void Play(MonoBehaviour script) {
                if (!ValidateSound(script)) return;
                if (!CanPlaySimultaneousSound()) return;
                if (oneShot) {
                    UpdateVariance(script);
                    soundMap[script].PlayOneShot(clips[currentClipIndex]);
                } else {
                    UpdateVariance(script);
                    soundMap[script].Stop();
                    soundMap[script].PlayDelayed(UnityEngine.Random.Range(0f, delayVariance + courseDelayVariance));
                }
            }

            public override void Stop(MonoBehaviour script) {
                if (!ValidateSound(script)) return;
                if (!script.enabled) return;
                soundMap[script].Stop();
            }

            // PRIVATE

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

            void UpdateVariance(MonoBehaviour script) {
                currentClipIndex = UnityEngine.Random.Range(0, clips.Length);
                soundMap[script].clip = clips[currentClipIndex];
                soundMap[script].volume = Utils.RandomVariance(volume, volumeVariance, 0f, 1f);
                soundMap[script].pitch = Utils.RandomVariance(pitch, pitchVariance, 0f, 1f);
            }

            bool ValidateSound(MonoBehaviour script) {
                if (script == null || !script.enabled) return false;
                if (!soundMap.ContainsKey(script)) return false;
                if (clips.Length == 0) return false;
                if (!soundMap[script].enabled) return false;
                if (soundMap[script] == null) return false;
                return true;
            }

            protected override IEnumerator RealtimeEditorInspection() {
                yield return null;
            }
        }
    }
}

