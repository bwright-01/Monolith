using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

using Actor;
using Audio.Sound;

namespace Player {

    [RequireComponent(typeof(Health))]
    public class PlayerMain : MonoActor {

        [SerializeField] LoopableSound footstepsSound;
        [SerializeField] LoopableSound hazardLavaSound;

        [Space]
        [Space]

        [SerializeField] CinemachineImpulseSource screenShakeOnDamage;
        [SerializeField] CinemachineImpulseSource screenShakeOnDeath;

        // cached
        PlayerController controller;
        PlayerMovement movement;
        PlayerShooter shooter;
        PlayerInput input;

        public PlayerShooter GetShooter() {
            return shooter;
        }

        void OnEnable() {
            SubscribeToEvents();
            eventChannel.OnHazardEnter.Subscribe(OnHazardEnter);
            eventChannel.OnHazardExit.Subscribe(OnHazardExit);
        }

        void OnDisable() {
            UnsubscribeFromEvents();
            eventChannel.OnHazardEnter.Unsubscribe(OnHazardEnter);
            eventChannel.OnHazardExit.Unsubscribe(OnHazardExit);
            StopAllCoroutines();
        }

        void Awake() {
            Init();
            controller = GetComponent<PlayerController>();
            movement = GetComponent<PlayerMovement>();
            shooter = GetComponent<PlayerShooter>();
            input = GetComponent<PlayerInput>();
            footstepsSound.Init(this);
            hazardLavaSound.Init(this);
        }

        private void Update() {
            if (IsAlive() && movement != null && movement.HasMoveInput()) {
                footstepsSound.Play();
            } else {
                footstepsSound.Stop();
            }
        }

        public override Region GetRegion() {
            return null;
        }

        void OnHazardEnter(Environment.HazardType hazardType) {
            hazardLavaSound.Play();
        }

        void OnHazardExit(Environment.HazardType hazardType) {
            hazardLavaSound.Stop();
        }

        public override void OnHealthGained(float amount, float hp) {
            // do nothing
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
            gameObject.name = "Player (DEAD)";

            footstepsSound.Stop();
            hazardLavaSound.Stop();

            footstepsSound.Unload();
            hazardLavaSound.Unload();

            shooter.enabled = false;
            controller.enabled = false;
            movement.enabled = false;
            input.enabled = false;
            Destroy(shooter);
            Destroy(input);
            Destroy(controller);

            GameObject[] objectsWithPlayerTag = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in objectsWithPlayerTag) {
                obj.tag = "Untagged";
            }

            eventChannel.OnPlayerDeath.Invoke();

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
