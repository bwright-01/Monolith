using System.Collections;
using UnityEngine;

using Core;
using Audio.Sound;

namespace Weapon {

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Actor.DamageDealer))]

    public class Melee : BaseWeapon {
        [SerializeField] bool debug = false;

        [Space]
        [Space]

        [SerializeField][Range(0f, 2f)] float timeWaitAfterAttack = 0.2f;

        [Space]
        [Space]

        [SerializeField] string attackTriggerName = "Attack";
        [SerializeField] Animator animator;
        [SerializeField] SpriteRenderer debugSprite;

        [Space]
        [Space]

        [SerializeField] SingleSound attackSound;
        [SerializeField] SingleSound hitEnemySound;
        [SerializeField] SingleSound hitEnvironmentSound;

        // cached
        new Collider2D collider;

        // state
        Coroutine ieAttack;

        void Awake() {
            collider = GetComponent<Collider2D>();
            collider.enabled = false;
            if (debugSprite != null) debugSprite.enabled = false;
            attackSound.Init(this);
            hitEnemySound.Init(this);
            hitEnvironmentSound.Init(this);
        }

        public override void TryAttack() {
            if (ieAttack != null) return;

            attackSound.Play();

            if (animator != null && animator.runtimeAnimatorController != null) {
                animator.SetTrigger(attackTriggerName);
            } else {
                ApplyDamage();
            }
        }

        public void ApplyDamage() {
            ieAttack = StartCoroutine(IAttack());
        }

        public void OnHit(int layer) {
            int mask = Layer.Enemy.mask | Layer.NPC.mask;
            if (LayerUtils.LayerMaskContainsLayer(mask, layer)) {
                hitEnemySound.Play();
            } else {
                hitEnvironmentSound.Play();
            }
        }

        IEnumerator IAttack() {
            collider.enabled = true;
            if (debug && debugSprite != null) debugSprite.enabled = true;

            yield return new WaitForFixedUpdate();

            collider.enabled = false;

            yield return new WaitForSeconds(timeWaitAfterAttack);

            if (debug && debugSprite != null) debugSprite.enabled = false;
            ieAttack = null;
        }

        void OnGUI() {
            if (debug) {
                GUILayout.TextField("MELEE");
                if (GUILayout.Button("Attack")) {
                    TryAttack();
                }
            }
        }
    }
}

