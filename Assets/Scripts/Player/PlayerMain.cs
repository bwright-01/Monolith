using System.Collections;
using UnityEngine;
using Cinemachine;

using Actor;
using Core;
using Audio.Sound;

namespace Player {

    [RequireComponent(typeof(Health))]
    public class PlayerMain : MonoActor {

        [SerializeField] CinemachineImpulseSource screenShakeOnDamage;
        [SerializeField] CinemachineImpulseSource screenShakeOnDeath;

        // cached
        PlayerController controller;
        PlayerMovement movement;

        void OnEnable() {
            SubscribeToEvents();
        }

        void OnDisable() {
            UnsubscribeFromEvents();
        }

        void Awake() {
            Init();
            controller = GetComponent<PlayerController>();
            movement = GetComponent<PlayerMovement>();
        }

        public override Region GetRegion() {
            return null;
        }

        public override void OnDamageTaken(float damage, float hp) {
            CommonDamageActions();
            StartCoroutine(ScreenShakeOnDamage(damage));
            eventChannel.OnShakeGamepad.Invoke(.2f, .5f);
        }

        public override void OnDamageGiven(float damage, bool wasKilled) {
            // TODO: if (wasKilled) saySarcasticPun();
        }

        public override void OnDeath(float damage, float hp) {
            CommonDeathActions();

            controller.enabled = false;
            movement.enabled = false;

            StartCoroutine(ScreenShakeOnDeath());
            eventChannel.OnShakeGamepad.Invoke(1f, .7f);
            eventChannel.OnFreezeTime.Invoke(1f, 0.3f);
        }

        IEnumerator ScreenShakeOnDamage(float damage) {
            screenShakeOnDamage.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * damage * 0.1f);
            yield return new WaitForSeconds(0.1f);
            screenShakeOnDamage.GenerateImpulse(UnityEngine.Random.insideUnitCircle.normalized * damage * 0.1f);
        }

        IEnumerator ScreenShakeOnDeath() {
            screenShakeOnDeath.GenerateImpulse(Vector3.right);
            yield return new WaitForSeconds(0.1f);
            screenShakeOnDeath.GenerateImpulse(Vector3.up);
        }
    }
}
