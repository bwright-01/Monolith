using UnityEngine;

using Core;

// This class houses all of the main systems that should persist from one scene to another
// This, and only this MonoBehaviour, should be a Singleton
// Any other systems that need to persist should be childed to this script's GameObject.

namespace Game {

    public class GameSystems : MonoBehaviour {

        // singleton
        static GameSystems _current;
        public static GameSystems current => _current;

        void Awake() {
            Layer.Init();
            _current = SystemUtils.ManageSingleton<GameSystems>(_current, this);
        }
    }
}
