using UnityEngine;

namespace Audio {
    namespace Music {

        public class Track : MonoBehaviour {
            const string INVALID_TRACK_NAME = "__INVALID_TRACK_NAME__";

            // props
            string trackName = INVALID_TRACK_NAME;
            AudioSource source;

            public AudioSource Source => source;

            public new string name => trackName;

            void Awake() {
                trackName = gameObject.name;
                source = GetComponentInChildren<AudioSource>();
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                source.loop = true;
            }
        }
    }
}
