
using UnityEngine;

namespace Weapon {

    [System.Serializable]
    public abstract class BaseWeapon : MonoBehaviour {
        public abstract bool TryAttack();
    }
}
