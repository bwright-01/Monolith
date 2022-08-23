
using System.Collections;
using UnityEngine;

namespace Audio {
    namespace Music {
        public static class MusicUtils {

            public static IEnumerator FadeIn(Track track, float fadeTime, float finalVolume) {
                float durationQuotient = 1f / fadeTime;

                track.Source.volume = 0;

                while (track.Source != null && track.Source.volume < finalVolume) {
                    track.Source.volume += finalVolume * Time.deltaTime * durationQuotient;
                    yield return new WaitForEndOfFrame();
                }

                track.Source.volume = finalVolume;
            }

            public static IEnumerator FadeOut(Track track, float fadeTime) {
                float durationQuotient = 1f / fadeTime;
                float startVolume = track.Source.volume;

                while (track.Source != null && track.Source.volume > 0) {
                    track.Source.volume -= startVolume * Time.deltaTime * durationQuotient;
                    yield return new WaitForEndOfFrame();
                }

                if (track.Source != null) track.Source.volume = 0;
            }

            public static IEnumerator CrossFade(Track trackFrom, Track trackTo, float duration, float endVolumeTarget, System.Action<Track> AfterCrossFade) {
                if (trackTo.Source != null) trackTo.Source.volume = 0f;

                float startVolume = trackFrom.Source != null ? trackFrom.Source.volume : 0f;
                float durationQuotient = 1f / duration;

                while (trackFrom.Source != null && trackTo.Source != null && trackFrom.Source.volume > 0f && trackTo.Source.volume < endVolumeTarget) {
                    trackFrom.Source.volume -= startVolume * Time.deltaTime * durationQuotient;
                    trackTo.Source.volume += endVolumeTarget * Time.deltaTime * durationQuotient;
                    yield return new WaitForEndOfFrame();
                }

                if (trackFrom.Source != null) trackFrom.Source.volume = 0f;
                if (trackTo.Source != null) trackTo.Source.volume = endVolumeTarget;

                AfterCrossFade(trackTo);
            }
        }
    }
}