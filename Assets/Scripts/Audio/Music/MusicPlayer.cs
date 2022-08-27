using System.Collections.Generic;
using UnityEngine;

using Core;

// USAGE NOTES:
// - Add MusicPlayer - can be a persisted GameObject, but doesn't have to be
// - Add Tracks as children - each Track should have an AudioSource component
// that's it!

namespace Audio {
    namespace Music {

        public class MusicPlayer : MonoBehaviour {
            [SerializeField] bool debug = false;
            [SerializeField][Range(0f, 20f)] float fadeInDuration = 4f;
            [SerializeField][Range(0f, 20f)] float fadeOutDuration = 2f;
            [SerializeField][Range(0f, 20f)] float crossfadeDuration = 10f;
            [SerializeField][Range(0f, 1f)] float musicVolume = 0.7f;

            [Space]
            [Space]

            [SerializeField] EventChannelSO eventChannel;

            // props
            Track[] tracks;
            Dictionary<string, Track> tracksMap = new Dictionary<string, Track>();

            // state
            bool isPlaying = false; // note this keeps track of what can currently be *heard*, not necessarily of what is currently playing (all tracks start playing silently)
            Track currentTrack;
            Track incomingTrack;

            Coroutine ieFadeIn;
            Coroutine ieFadeOut;
            Coroutine ieCrossFade;

            void OnEnable() {
                eventChannel.OnPlayMusic.Subscribe(OnPlayMusic);
            }

            void OnDisable() {
                eventChannel.OnPlayMusic.Unsubscribe(OnPlayMusic);
            }

            public void OnPlayMusic(string trackName) {
                if (IsTrackPlayingOrEnqueued(trackName)) return;
                incomingTrack = LookupTrack(trackName);
                if (incomingTrack == null) {
                    Debug.LogError($"No track was found matching name of \"{trackName}\"");
                    return;
                }
                if (ieFadeIn != null) StopCoroutine(ieFadeIn);
                if (ieFadeOut != null) StopCoroutine(ieFadeOut);
                if (ieCrossFade != null) StopCoroutine(ieCrossFade);
                if (isPlaying) {
                    ieCrossFade = StartCoroutine(MusicUtils.CrossFade(currentTrack, incomingTrack, crossfadeDuration, musicVolume, (Track newTrack) => {
                        currentTrack = newTrack;
                        incomingTrack = null;
                    }));
                } else {
                    isPlaying = true;
                    currentTrack = incomingTrack;
                    ZeroOutAllTracks();
                    ieFadeIn = StartCoroutine(MusicUtils.FadeIn(currentTrack, fadeInDuration, musicVolume));
                }
            }

            public void StartMusic() {
                OnPlayMusic(currentTrack.name);
            }

            public void StopMusic() {
                if (ieFadeIn != null) StopCoroutine(ieFadeIn);
                if (ieFadeOut != null) StopCoroutine(ieFadeOut);
                if (ieCrossFade != null) StopCoroutine(ieCrossFade);
                foreach (var track in tracks) {
                    ieFadeOut = StartCoroutine(MusicUtils.FadeOut(track, fadeOutDuration));
                }
            }

            void Start() {
                tracksMap.Clear();
                tracks = GetComponentsInChildren<Track>();
                foreach (var track in tracks) {
                    tracksMap.TryAdd(track.name, track);
                    if (track.Source == null) { Debug.LogWarning($"MusicPlayer track \"{track.name}\" has no audio source!!"); continue; }
                    track.Source.volume = 0;
                    track.Source.PlayScheduled(AudioSettings.dspTime + 0.5f);
                }
            }

            void ZeroOutAllTracks() {
                foreach (var track in tracks) {
                    track.Source.volume = 0;
                }
            }

            Track LookupTrack(string trackName) {
                if (tracksMap.TryGetValue(trackName, out Track value)) {
                    return value;
                }
                return null;
            }

            bool IsTrackPlayingOrEnqueued(string trackName) {
                if (currentTrack != null && currentTrack.name == trackName) return true;
                if (incomingTrack != null && incomingTrack.name == trackName) return true;
                return false;
            }

            void OnGUI() {
                if (debug) {
                    GUILayout.TextField("SOUNDZ");
                    foreach (var track in tracks) {
                        if (GUILayout.Button(track.name)) {
                            OnPlayMusic(track.name);
                        }
                    }
                }
            }
        }
    }
}
