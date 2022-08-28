using System.Collections;
using UnityEngine;

using Core;
using Player;
using Movement;

// NOTES
// This component will constantly try to attack the player as long
// as it is enabled. Control its enabled state to alter its behaviour.

namespace Enemy {

    public class EnemyAttack : MonoBehaviour {

        [SerializeField] bool debug;

        [Space]

        [SerializeField] bool disableOnAwake = true;
        [SerializeField] Weapon.BaseWeapon weapon;
        [SerializeField][Range(0f, 50f)] float attackRange = 5f;
        [Space]
        [Space]
        [SerializeField][Range(0f, 30f)] float timeWaitBeforeInitialAttack = 0f;
        [SerializeField][Range(0f, 30f)] float timeWaitAfterAttack = 2f;
        [SerializeField][Range(0f, 10f)] float timeWaitVariance = 0.2f;

        [Space]
        [Space]

        [SerializeField] string animatorAttackTriggerName = "Attacking";
        [SerializeField] Animator animator;

        // cached
        ActorMovement movement;
        PlayerMain player;
        EnemyMain enemy;
        EnemySight sight;

        public void AttackImmediately() {
            weapon.TryAttack();
        }

        public bool IsPlayerInAttackRange() {
            player = PlayerUtils.FindPlayer();
            if (!PlayerUtils.CheckIsAlive(player)) return false;
            return Vector2.Distance(player.transform.position, transform.position) <= attackRange;
        }

        void OnEnable() {
            StopAllCoroutines();
            StartCoroutine(IConstantlyAttackPlayer());
        }

        void OnDisable() {
            StopAllCoroutines();
            if (HasAnimator()) animator.SetBool(animatorAttackTriggerName, false);
        }

        void Awake() {
            enemy = GetComponent<EnemyMain>();
            sight = GetComponent<EnemySight>();
            movement = GetComponent<ActorMovement>();
            if (animator == null) animator = GetComponent<Animator>();
            if (disableOnAwake) enabled = false;
        }

        void TryAttack() {
            if (!ScreenUtils.IsObjectOnScreen(gameObject)) return;
            if (!IsPlayerInAttackRange()) return;
            sight.LookAtPlayer();
            if (HasAnimator()) {
                animator.SetBool(animatorAttackTriggerName, true);
            } else {
                AttackImmediately();
            }
        }

        float GetTimeWaitAfterAttack() {
            return Utils.RandomVariance(timeWaitAfterAttack, timeWaitVariance, timeWaitAfterAttack * 0.5f, timeWaitAfterAttack * 2f);
        }

        bool HasAnimator() {
            return animator != null && animator.runtimeAnimatorController != null;
        }

        IEnumerator IConstantlyAttackPlayer() {
            yield return new WaitForSeconds(timeWaitBeforeInitialAttack);

            while (enemy.IsAlive()) {
                TryAttack();
                yield return new WaitForSeconds(GetTimeWaitAfterAttack());
            }
        }

        void OnDrawGizmos() {
            if (!debug) return;
            float transparency = IsPlayerInAttackRange() ? 0.5f : 0.2f;
            Utils.DebugDrawCircle(transform.position, attackRange, Utils.Transparentize(Color.red, transparency));
        }
    }
}
