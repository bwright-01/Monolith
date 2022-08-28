using System.Collections;
using Core;
using UnityEngine;

namespace Audio {
    namespace Music {

        public class PlayMusicOnAwake : MonoBehaviour {
            [SerializeField] string musicTrack;
            [SerializeField] EventChannelSO eventChannel;

            IEnumerator Start() {
                yield return new WaitForSeconds(1f);
                eventChannel.OnPlayMusic.Invoke(musicTrack);
            }
        }
    }
}
