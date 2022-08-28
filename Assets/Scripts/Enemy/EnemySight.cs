
using UnityEngine;

using Core;
using Player;
using Movement;

// TODO: USE RAYCAST

namespace Enemy {

    public class EnemySight : MonoBehaviour {

        [SerializeField] bool debug;
        [SerializeField] float rangeOfSight = 5f;

        [SerializeField] string animatorSightTriggerName = "SeesPlayer";
        [SerializeField] Animator animator;

        PlayerMain player;
        ActorMovement movement;

        public bool CanSeePlayer() {
            player = PlayerUtils.FindPlayer();
            if (!PlayerUtils.CheckIsAlive(player)) return false;
            return Vector2.Distance(player.transform.position, transform.position) <= rangeOfSight;
        }

        public void LookAtPlayer() {
            if (movement == null) return;
            if (CanSeePlayer()) {
                if (HasAnimator()) animator.SetBool(animatorSightTriggerName, true);
                movement.LookAt(player.transform);
            } else {
                if (HasAnimator()) animator.SetBool(animatorSightTriggerName, false);
                movement.LookAt(null);
            }
        }

        private void Awake() {
            movement = GetComponent<ActorMovement>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        bool HasAnimator() {
            return animator != null && animator.runtimeAnimatorController != null;
        }

        void OnDrawGizmos() {
            if (!debug) return;
            float transparency = CanSeePlayer() ? 0.5f : 0.2f;
            Utils.DebugDrawCircle(transform.position, rangeOfSight, Utils.Transparentize(Color.magenta, transparency));
        }
    }
}
