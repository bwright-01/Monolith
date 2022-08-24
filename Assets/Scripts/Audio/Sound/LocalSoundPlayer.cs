using System.Collections.Generic;
using UnityEngine;

using Core;

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
                player = GetComponentInParent<iLocalSoundPlayer>();
                sounds = GetComponentsInChildren<LocalSound>();
                foreach (var sound in sounds) soundsMap.TryAdd(sound.name, sound);
                if (player == null) {
                    Debug.LogWarning($"{Utils.FullGameObjectName(gameObject)}.LocalSoundPlayer has no iLocalSoundPlayer parent!! No sound events will be handled. :(");
                }
            }

            void OnPlaySound(string soundName) {
                currentSound = LookupSound(soundName);
                if (currentSound) {
                    if (debug) Debug.Log($">> Playing sound: {soundName}");
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
