using System.Collections;
using UnityEngine;

using Core;
using Audio.Sound;

namespace Weapon {

    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Actor.DamageDealer))]
    public class Melee : MonoBehaviour, iLocalSoundPlayer {
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

        [SerializeField] string attackSound = "AttackSound";
        [SerializeField] string hitEnemySound = "HitEnemySound";
        [SerializeField] string hitEnvironmentSound = "HitEnvironmentSound";

        // cached
        new Collider2D collider;

        // state
        Coroutine ieAttack;

        public event StringEvent OnPlaySound;
        public void PlaySound(string soundName) {
            if (OnPlaySound != null) OnPlaySound(soundName);
        }

        void Awake() {
            collider = GetComponent<Collider2D>();
            collider.enabled = false;
            if (debugSprite != null) debugSprite.enabled = false;
        }

        public void TryAttack() {
            if (ieAttack != null) return;

            PlaySound(attackSound);

            if (animator != null) {
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
                PlaySound(hitEnemySound);
            } else {
                PlaySound(hitEnvironmentSound);
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

