using UnityEngine;

namespace Actor {

    [RequireComponent(typeof(Renderer))]
    public class KillWhenOffscreen : MonoBehaviour {
        iActor actor;

        void Awake() {
            actor = GetComponentInParent<iActor>();
        }

        void OnBecameInvisible() {
            if (actor != null) actor.TakeDamage(Constants.INSTAKILL, Vector2.zero);
        }
    }
}
