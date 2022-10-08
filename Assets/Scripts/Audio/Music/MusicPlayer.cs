using System.Collections.Generic;
using UnityEngine;

using Core;

// USAGE NOTES:
// - Add MusicPlayer - can be a persisted GameObject, but doesn't have to be
// - Add Tracks as children - each Track should have an AudioSource component
// that's it!

namespace Audio {
    namespace Music {

        enum TrackStatus {
            Idle,
            Playing,
            FadingIn,
            FadingOut,
        }

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
            Dictionary<Track, TrackStatus> coroutineMap = new Dictionary<Track, TrackStatus>();

            // state
            bool isPlaying => currentTrack != null;
            Track foundTrack;
            Track currentTrack;
            Track incomingTrack;
            Track outgoingTrack;

            void OnEnable() {
                eventChannel.OnPlayMusic.Subscribe(OnPlayMusic);
                eventChannel.OnStopMusic.Subscribe(OnStopMusic);
                eventChannel.OnResetMusic.Subscribe(OnResetMusic);
            }

            void OnDisable() {
                eventChannel.OnPlayMusic.Unsubscribe(OnPlayMusic);
                eventChannel.OnStopMusic.Unsubscribe(OnStopMusic);
                eventChannel.OnResetMusic.Unsubscribe(OnResetMusic);
            }

            void OnResetMusic() {
                StopAllCoroutines();
                currentTrack = null;
                incomingTrack = null;
                outgoingTrack = null;
                foreach (var track in tracks) {
                    track.Source.volume = 0;
                    track.Source.Stop();
                    track.Source.PlayScheduled(AudioSettings.dspTime + 1f);
                }
            }

            public void OnPlayMusic(string trackName) {
                if (IsTrackPlayingOrEnqueued(trackName)) return;
                foundTrack = LookupTrack(trackName);
                if (foundTrack == null) {
                    Debug.LogError($"No track was found matching name of \"{trackName}\"");
                    return;
                }
                StopAllCoroutines();
                if (isPlaying) {
                    if (debug) Debug.Log($"CROSSFADE >> current={currentTrack.name} found={foundTrack.name} incoming={(incomingTrack != null ? incomingTrack.name : "null")} outgoing={(outgoingTrack != null ? outgoingTrack.name : "null")}");
                    // in the last iteration, the incoming track would eventually become the current track
                    // here, `incomingTrack` represents the track that we now should be fading FROM
                    currentTrack = incomingTrack != null ? incomingTrack : currentTrack;
                    if (outgoingTrack != null) outgoingTrack.Source.volume = 0f;
                    incomingTrack = foundTrack;
                    outgoingTrack = currentTrack;

                    StartCoroutine(MusicUtils.CrossFade(outgoingTrack, incomingTrack, crossfadeDuration, musicVolume, (Track newTrack) => {
                        currentTrack = newTrack;
                        incomingTrack = null;
                        outgoingTrack = null;

                        if (debug) Debug.Log($"CROSSFADE_FINISHED >> current={currentTrack.name}");
                    }));
                } else {
                    if (debug) Debug.Log($"FADEIN >> found={foundTrack.name}");
                    currentTrack = foundTrack;
                    ZeroOutAllTracks();
                    StartCoroutine(MusicUtils.FadeIn(currentTrack, fadeInDuration, musicVolume));
                }
            }

            public void StartMusic() {
                OnPlayMusic(currentTrack != null ? currentTrack.name : tracks[0].name);
            }

            public void OnStopMusic() {
                if (debug) Debug.Log($"FADEOUT");
                StopAllCoroutines();
                foreach (var track in tracks) {
                    StartCoroutine(MusicUtils.FadeOut(track, fadeOutDuration));
                }
                currentTrack = null;
                incomingTrack = null;
            }

            public void StopMusic() {
                OnStopMusic();
            }

            void Start() {
                tracksMap.Clear();
                tracks = GetComponentsInChildren<Track>();
                foreach (var track in tracks) {
                    tracksMap.TryAdd(track.name, track);
                    if (track.Source == null) { Debug.LogWarning($"MusicPlayer track \"{track.name}\" has no audio source!!"); continue; }
                    track.Source.volume = 0;
                    track.Source.PlayScheduled(AudioSettings.dspTime + 1f);
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
