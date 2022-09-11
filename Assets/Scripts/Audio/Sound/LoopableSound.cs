using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

using Core;

namespace Audio {

    namespace Sound {
        public enum PlayCursor {
            Stopped,
            Head,
            Loop,
            Tail,
        }

        [CreateAssetMenu(fileName = "LoopableSound", menuName = "ScriptableObjects/LoopableSound", order = 0)]
        public class LoopableSound : BaseSound {
            [SerializeField][Range(0f, 0.2f)] protected float pitchVariance = 0f;

            [SerializeField] AudioClip clipHead;
            [SerializeField] AudioClip clipLoop;
            [SerializeField] AudioClip clipTail;

            [SerializeField] bool playLoopToEnd = false;

            // AudioSource sourceHead;
            // AudioSource sourceLoop;
            // AudioSource sourceTail;

            // cached
            // double dspStartDelay = 0.01;
            // double clipHeadDuration = 0.0;
            // double clipLoopDuration = 0.0;
            // double clipTailDuration = 0.0;
            // double timeHeadStartScheduled = 0.0;
            // double timeLoopStartScheduled = 0.0;
            // double timeLoopEndScheduled = 0.0;
            // Coroutine playCoroutine;
            // Coroutine inspectCoroutine;

            // state
            PlayCursor cursor = PlayCursor.Head;
            bool playButtonPressed = false;

            // public override bool isPlaying => playButtonPressed
            //     || (cursor != PlayCursor.Stopped)
            //     || (sourceHead != null && sourceHead.isPlaying)
            //     || (sourceLoop != null && sourceLoop.isPlaying)
            //     || (sourceTail != null && sourceTail.isPlaying);
            // public override bool hasClip => clipLoop != null;
            // public override bool hasSource => sourceLoop != null;
            // public PlayCursor Cursor => cursor;

            Dictionary<MonoBehaviour, AudioSource> soundMap = new Dictionary<MonoBehaviour, AudioSource>();

            public override void Init(MonoBehaviour script, AudioMixerGroup mix = null, AudioSource _source = null) {
                if (mix != null) mixerGroup = mix;

                // if (clipHead != null) {
                //     sourceHead = script.gameObject.AddComponent<AudioSource>();
                //     SetSource(clipHead, sourceHead, false);
                //     clipHeadDuration = GetClipDuration(clipHead);
                // }
                // if (clipLoop != null) {
                //     sourceLoop = script.gameObject.AddComponent<AudioSource>();
                //     SetSource(clipLoop, sourceLoop, true);
                //     clipLoopDuration = GetClipDuration(clipLoop);
                // }
                // if (clipTail != null) {
                //     sourceTail = script.gameObject.AddComponent<AudioSource>();
                //     SetSource(clipTail, sourceTail, false);
                //     clipTailDuration = GetClipDuration(clipTail);
                // }

                // inspectCoroutine = script.StartCoroutine(RealtimeEditorInspection());


                if (clipLoop != null) {
                    AudioSource source = script.gameObject.AddComponent<AudioSource>();
                    SetSource(clipLoop, source, true);
                    soundMap[script] = source;
                }
            }

            public override void Unload(MonoBehaviour script) {
                // if (script != null && playCoroutine != null) script.StopCoroutine(playCoroutine);
                // if (script != null && inspectCoroutine != null) script.StopCoroutine(inspectCoroutine);
                // Destroy(sourceHead);
                // Destroy(sourceLoop);
                // Destroy(sourceTail);
                // sourceHead = null;
                // sourceLoop = null;
                // sourceTail = null;

                if (!ValidateSound(script)) return;
                Destroy(soundMap[script]);
                soundMap[script] = null;
                soundMap.Remove(script);
            }

            void SetSource(AudioClip clip, AudioSource source, bool loop) {
                if (source == null) return;
                source.volume = volume;
                source.pitch = pitch;
                source.loop = loop;
                source.clip = clip;
                source.playOnAwake = false;
                source.ignoreListenerPause = ignoreListenerPause;
                source.outputAudioMixerGroup = mixerGroup;
                // 3d settings
                source.dopplerLevel = dopplerLevel;
                source.spread = spread;
                source.spatialBlend = spatialBlend;
                source.minDistance = minFalloffDistance;
                source.maxDistance = maxFalloffDistance;
            }

            public override void Play(MonoBehaviour script) {
                if (!ValidateSound(script)) return;
                if (!soundMap[script].isPlaying) soundMap[script].Play();

                // if (!ValidateSound()) return;
                // if (playButtonPressed) return;

                // if (sourceHead != null && sourceHead.isPlaying) sourceHead.Stop();
                // if (sourceLoop != null && sourceLoop.isPlaying) sourceLoop.Stop();
                // if (sourceTail != null && sourceTail.isPlaying) sourceTail.Stop();

                // if (playCoroutine != null) _script.StopCoroutine(playCoroutine);
                // if (sourceLoop != null) sourceLoop.pitch = Utils.RandomVariance(pitch, pitchVariance, 0f, 1f);
                // playCoroutine = _script.StartCoroutine(IPlay());
            }

            // IEnumerator IPlay() {
            //     playButtonPressed = true;

            //     if (sourceHead != null && sourceHead.isActiveAndEnabled) {
            //         cursor = PlayCursor.Head;
            //         timeHeadStartScheduled = AudioSettings.dspTime + dspStartDelay;
            //         timeLoopStartScheduled = timeHeadStartScheduled + clipHeadDuration;
            //         sourceHead.PlayScheduled(timeHeadStartScheduled);
            //         sourceLoop.PlayScheduled(timeLoopStartScheduled);

            //         while (playButtonPressed && sourceHead.isPlaying) yield return null;

            //         sourceHead.Stop();
            //         if (playButtonPressed) cursor = PlayCursor.Loop;
            //     } else {
            //         cursor = PlayCursor.Loop;
            //         timeLoopStartScheduled = AudioSettings.dspTime + dspStartDelay;
            //         sourceLoop.PlayScheduled(timeLoopStartScheduled);
            //     }

            //     while (playButtonPressed && sourceLoop.isPlaying) yield return null;

            //     if (cursor == PlayCursor.Loop && playLoopToEnd) {
            //         int numFullCycles = GetNumFullLoopCycles(sourceLoop.pitch);
            //         timeLoopEndScheduled = timeLoopStartScheduled + clipLoopDuration * numFullCycles * (double)sourceLoop.pitch;
            //     } else {
            //         // do not wait until loop end; start playing tail immediately
            //         timeLoopEndScheduled = AudioSettings.dspTime + dspStartDelay;
            //     }

            //     if (sourceLoop.isPlaying) sourceLoop.SetScheduledEndTime(timeLoopEndScheduled);
            //     if (sourceTail != null && sourceTail.enabled) sourceTail.PlayScheduled(timeLoopEndScheduled);

            //     while (sourceLoop.isPlaying) yield return null;

            //     cursor = PlayCursor.Tail;

            //     while (sourceTail != null && sourceTail.isPlaying) yield return null;

            //     cursor = PlayCursor.Stopped;
            //     playButtonPressed = false;
            //     playCoroutine = null;
            // }

            public override void Stop(MonoBehaviour script) {
                if (!ValidateSound(script)) return;
                if (!script.enabled) return;
                soundMap[script].Stop();
            }

            protected override IEnumerator RealtimeEditorInspection() {
                yield return null;
                // while (_script != null) {
                //     yield return new WaitForSecondsRealtime(1f);
                //     if (!realtimeEditorInspect) continue;

                //     if (clipHead != null) {
                //         SetSource(clipHead, sourceHead, false);
                //         clipHeadDuration = GetClipDuration(clipHead);
                //     }
                //     if (clipLoop != null) {
                //         SetSource(clipLoop, sourceLoop, true);
                //         clipLoopDuration = GetClipDuration(clipLoop);
                //     }
                //     if (clipTail != null) {
                //         SetSource(clipTail, sourceTail, false);
                //         clipTailDuration = GetClipDuration(clipTail);
                //     }
                // }
            }

            bool ValidateSound(MonoBehaviour script) {
                if (script == null || !script.enabled) return false;
                if (!soundMap.ContainsKey(script)) return false;
                if (soundMap[script] == null) return false;
                if (soundMap[script].clip == null) return false;
                if (!soundMap[script].enabled) return false;
                return true;
            }

            // double GetClipDuration(AudioClip clip) {
            //     return (double)clip.samples / clip.frequency;
            // }

            // // calculate timeLoopEnd based on amount of time has elapsed since timeLoopStart, vs. per the clipLoopDuration
            // // e.g. what the number of cycles would be if the loop had finished
            // int GetNumFullLoopCycles(double currentPitch = 1.0) {
            //     return Mathf.Min(1, Mathf.CeilToInt((float)((AudioSettings.dspTime - timeLoopStartScheduled) / (clipLoopDuration * currentPitch))));
            // }
        }
    }
}
