
using UnityEngine;

namespace Game {

    [CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState", order = 0)]
    public class GameState : ScriptableObject {

        [SerializeField] int initialLives = 5;

        public int lives { get; private set; } = 5;

        public bool IsRedMonolithDestroyed { get; private set; }
        public bool IsYellowMonolithDestroyed { get; private set; }
        public bool IsBlueMonolithDestroyed { get; private set; }
        public bool AreAllMonolithsDestroyed => IsRedMonolithDestroyed && IsYellowMonolithDestroyed && IsBlueMonolithDestroyed;

        public Vector2 respawnPoint { get; private set; }

        public void Init() {
            lives = initialLives;
            IsRedMonolithDestroyed = false;
            IsYellowMonolithDestroyed = false;
            IsBlueMonolithDestroyed = false;
        }

        public void SetMonolithDestroyed(Environment.MonolithType monolithType) {
            switch (monolithType) {
                case Environment.MonolithType.Red:
                    IsRedMonolithDestroyed = true;
                    break;
                case Environment.MonolithType.Yellow:
                    IsYellowMonolithDestroyed = true;
                    break;
                case Environment.MonolithType.Blue:
                    IsBlueMonolithDestroyed = true;
                    break;
            }
        }

        public void SetRespawnPoint(Vector2 value) {
            respawnPoint = value;
        }

        public void LoseLife() {
            lives--;
        }
    }
}
