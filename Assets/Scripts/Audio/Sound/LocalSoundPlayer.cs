using System.Collections.Generic;
using UnityEngine;

// USAGE NOTES:
// - implement iLocalSoundPlayer in a top-level MonoBehaviour script
// - add LocalSoundPlayer as a child or descendent
// - add LocalSound as a child of LocalSoundPlayer's GameObject
// - add one or more AudioSource(s) directly to LocalSound's GameObject, or as children of the same
// - highly recommend making LocalSound GameObjects prefabs for reusability
// that's it!

namespace Audio {
    namespace Sound {

        public class LocalSoundPlayer : MonoBehaviour {
            [SerializeField] bool debug = false;

            iLocalSoundPlayer player;

            // props
            LocalSound[] sounds;
            Dictionary<string, LocalSound> soundsMap = new Dictionary<string, LocalSound>();

            // state
            LocalSound currentSound;

            void OnEnable() {
                if (player == null) return;
                player.OnPlaySound += OnPlaySound;
            }

            void OnDisable() {
                if (player == null) return;
                player.OnPlaySound -= OnPlaySound;
            }

            void Awake() {
                iLocalSoundPlayer player = GetComponentInParent<iLocalSoundPlayer>();
                sounds = GetComponentsInChildren<LocalSound>();
                foreach (var sound in sounds) soundsMap.TryAdd(sound.gameObject.name, sound);
                if (player == null) {
                    Debug.LogWarning("A LocalSoundPlayer exists but has no iLocalSoundPlayer parent!! No sound events will be handled. :(");
                }
            }

            void OnPlaySound(string soundName) {
                currentSound = LookupSound(soundName);
                if (currentSound) {
                    currentSound.Play();
                } else {
                    Debug.LogError($"No sound was found matching name of \"{soundName}\"");
                }
            }

            LocalSound LookupSound(string soundName) {
                if (soundsMap.TryGetValue(soundName, out LocalSound value)) {
                    return value;
                }
                return null;
            }

            void OnGUI() {
                if (debug) {
                    GUILayout.TextField("SOUNDZ");
                    foreach (var sound in sounds) {
                        if (GUILayout.Button(sound.name)) {
                            sound.Play();
                        }
                    }
                }
            }
        }
    }
}
