using System.Collections;
using UnityEngine;
using Cinemachine;

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
        [SerializeField][Range(0f, 2f)] float timeWaitBeforeAttack = 0f;

        [Space]
        [Space]

        [SerializeField] string attackTriggerName = "Attack";
        [SerializeField] Animator animator;
        [SerializeField] SpriteRenderer debugSprite;

        [Space]
        [Space]

        [SerializeField] SingleSound attackSound;
        [SerializeField] SingleSound upgradedAttackSound;
        [SerializeField] SingleSound hitEnemySound;
        [SerializeField] SingleSound hitEnvironmentSound;

        [Space]
        [Space]

        [SerializeField] CinemachineImpulseSource screenShakeOnMelee;
        [SerializeField] CinemachineImpulseSource screenShakeOnMeleeUpgraded;

        // cached
        new Collider2D collider;
        Actor.DamageDealer damageDealer;

        // state
        Coroutine ieAttack;
        Coroutine ieScreenShake;
        bool isUpgraded;

        public void SetDamageMultiplier(float value) {
            damageDealer.SetDamageMultiplier(value);
            if (value > 1f) isUpgraded = true;
        }

        void Awake() {
            collider = GetComponent<Collider2D>();
            damageDealer = GetComponent<Actor.DamageDealer>();
            collider.enabled = false;
            damageDealer.enabled = false;
            if (debugSprite != null) debugSprite.enabled = false;
            attackSound.Init(this);
            upgradedAttackSound.Init(this);
            hitEnemySound.Init(this);
            hitEnvironmentSound.Init(this);
        }

        public override bool TryAttack() {
            if (ieAttack != null) return false;

            if (ieScreenShake != null) StopCoroutine(ieScreenShake);

            if (isUpgraded) {
                upgradedAttackSound.Play(this);
                ieScreenShake = StartCoroutine(ScreenShakeOnMeleeUpgraded());
            } else {
                attackSound.Play(this);
                ieScreenShake = StartCoroutine(ScreenShakeOnMelee());
            }

            if (animator != null && animator.runtimeAnimatorController != null) {
                animator.SetTrigger(attackTriggerName);
            } else {
                ApplyDamage();
            }
            return true;
        }

        public void ApplyDamage() {
            ieAttack = StartCoroutine(IAttack());
        }

        public void OnHit(int layer) {
            int mask = Layer.Enemy.mask | Layer.NPC.mask;
            if (LayerUtils.LayerMaskContainsLayer(mask, layer)) {
                hitEnemySound.Play(this);
            } else {
                hitEnvironmentSound.Play(this);
            }
        }

        IEnumerator IAttack() {
            yield return new WaitForSeconds(timeWaitBeforeAttack);

            damageDealer.enabled = true;
            collider.enabled = true;
            if (debug && debugSprite != null) debugSprite.enabled = true;

            yield return new WaitForFixedUpdate();

            collider.enabled = false;
            damageDealer.enabled = false;

            yield return new WaitForSeconds(timeWaitAfterAttack);

            if (debug && debugSprite != null) debugSprite.enabled = false;
            ieAttack = null;
        }

        IEnumerator ScreenShakeOnMelee() {
            screenShakeOnMelee.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * 0.05f);
            yield return new WaitForSeconds(0.1f);
            screenShakeOnMelee.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * 0.05f);
        }

        IEnumerator ScreenShakeOnMeleeUpgraded() {
            screenShakeOnMelee.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * 0.1f);
            yield return new WaitForSeconds(0.1f);
            screenShakeOnMelee.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * 0.1f);
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

