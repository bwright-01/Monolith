using UnityEngine;

namespace Enemy {

    public class EnemyPatrol : MonoBehaviour {
        [SerializeField] float timePatrol = 5f;
        [SerializeField] float timeWait = 1f;
        [SerializeField] Vector2 initialHeading = Vector2.right;

        public float TimePatrol => timePatrol;
        public float TimeWait => timeWait;
        public Vector2 InitialHeading => initialHeading;
    }
}

