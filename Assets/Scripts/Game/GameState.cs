
using UnityEngine;

namespace Game {

    [CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState", order = 0)]
    public class GameState : ScriptableObject {

        public bool IsRedMonolithDestroyed { get; private set; }
        public bool IsYellowMonolithDestroyed { get; private set; }
        public bool IsBlueMonolithDestroyed { get; private set; }
        public bool AreAllMonolithsDestroyed => IsRedMonolithDestroyed && IsYellowMonolithDestroyed && IsBlueMonolithDestroyed;

        public Vector2 respawnPoint { get; private set; }

        public void SetRespawnPoint(Vector2 value) {
            respawnPoint = value;
        }
    }
}
