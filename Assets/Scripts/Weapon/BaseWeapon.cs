
using UnityEngine;

namespace Weapon {

    [System.Serializable]
    public abstract class BaseWeapon : MonoBehaviour {
        public abstract void TryAttack();
    }
}
