
using UnityEngine;

namespace Game {

    public enum UpgradeType {
        Weapon,
        Melee,
        Movement,
    }

    [CreateAssetMenu(fileName = "GameState", menuName = "ScriptableObjects/GameState", order = 0)]
    public class GameState : ScriptableObject {

        [SerializeField] int initialLives = 5;

        [Space]
        [Space]

        [SerializeField][Range(0, 10)] int _upgradedShotsCount = 10;
        [SerializeField][Range(0f, 10f)] float _upgradedMeleeDamageMod = 2f;
        [SerializeField][Range(0f, 10f)] float _upgradedMoveSpeed = 8;

        [Space]
        [Space]

        [SerializeField] bool redDestroyedAtStart;
        [SerializeField] bool yellowDestroyedAtStart;
        [SerializeField] bool blueDestroyedAtStart;

        [Space]
        [Space]

        [SerializeField] bool weaponUpgradedAtStart;
        [SerializeField] bool meleeUpgradedAtStart;
        [SerializeField] bool movementUpgradedAtStart;

        public int UpgradedShotsCount => _upgradedShotsCount;
        public float UpgradedMeleeDamageMod => _upgradedMeleeDamageMod;
        public float UpgradedMoveSpeed => _upgradedMoveSpeed;

        public int lives { get; private set; } = 3;

        public bool IsRedMonolithDestroyed { get; private set; }
        public bool IsYellowMonolithDestroyed { get; private set; }
        public bool IsBlueMonolithDestroyed { get; private set; }
        public bool AreAllMonolithsDestroyed => IsRedMonolithDestroyed && IsYellowMonolithDestroyed && IsBlueMonolithDestroyed;

        public bool IsWeaponUpgraded { get; private set; }
        public bool IsMeleeUpgraded { get; private set; }
        public bool IsMovementUpgraded { get; private set; }

        public Vector2 respawnPoint { get; private set; }

        public void Init() {
            lives = initialLives;
            IsRedMonolithDestroyed = redDestroyedAtStart;
            IsYellowMonolithDestroyed = yellowDestroyedAtStart;
            IsBlueMonolithDestroyed = blueDestroyedAtStart;
            IsWeaponUpgraded = weaponUpgradedAtStart;
            IsMeleeUpgraded = meleeUpgradedAtStart;
            IsMovementUpgraded = movementUpgradedAtStart;
            // IsMovementUpgraded = System.Convert.ToBoolean(PlayerPrefs.GetInt("IsMovementUpgraded", 0));
        }

        public void ResetForContinue() {
            lives = initialLives;
        }

        public int GetNumMonolithsDestroyed() {
            int num = 0;
            if (IsRedMonolithDestroyed) num++;
            if (IsYellowMonolithDestroyed) num++;
            if (IsBlueMonolithDestroyed) num++;
            return num;
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

        public void ApplyUpgrade(UpgradeType upgradeType) {
            switch (upgradeType) {
                case UpgradeType.Weapon:
                    IsWeaponUpgraded = true;
                    break;
                case UpgradeType.Melee:
                    IsMeleeUpgraded = true;
                    break;
                case UpgradeType.Movement:
                    IsMovementUpgraded = true;
                    break;
            }
        }

        public void SetRespawnPoint(Vector2 value) {
            respawnPoint = value;
        }

        public void GainLife() {
            lives++;
            lives = Mathf.Min(lives, 5);
        }

        public void LoseLife() {
            lives--;
        }
    }
}
