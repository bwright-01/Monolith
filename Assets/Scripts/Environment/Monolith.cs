using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Core;

namespace Environment {

    public enum MonolithType {
        Red,
        Yellow,
        Blue,
    }

    public class Monolith : Actor.MonoActor {

        [Space]
        [Space]

        [SerializeField] MonolithType type;
        [SerializeField] Game.UpgradeType upgradeType;

        [Space]
        [Space]

        [SerializeField] bool debugCutscene;
        [SerializeField][Range(0f, 4f)] float timePanToMonolith = 2f;
        [SerializeField][Range(0f, 4f)] float timePause = 1.5f;
        [SerializeField][Range(0f, 4f)] float timePanFromMonolith = 1f;

        [Space]
        [Space]

        [SerializeField][Range(0f, 1f)] float brokenCasingThreshold = 0.5f;
        [SerializeField] ParticleSystem brokenFX;

        [Space]
        [Space]

        [SerializeField] CinemachineVirtualCamera virtualCamera;
        [SerializeField] CinemachineBrain brain;

        List<Pylon> pylons = new List<Pylon>();
        List<MonolithDoor> doors = new List<MonolithDoor>();

        bool didBreakCasing;
        bool didReportAllPylonsKilled;

        // cutscene state
        Vector2 playerPosition;
        Vector2 monolithPosition;
        Transform prevCameraTarget;
        Timer cutsceneTimer = new Timer(TimerDirection.Increment, TimerStep.UnscaledDeltaTime);
        System.Action OnRemoveDoors;

        void OnEnable() {
            SubscribeToEvents();
        }

        void OnDisable() {
            UnsubscribeFromEvents();
        }

        void Awake() {
            Init();
            actorHealth.SetIsInvulnerable(true);
        }

        void Start() {
            transform.SetParent(transform.parent.parent);
        }

        public void PanToMonolith(System.Action handleRemoveDoors = null) {
            OnRemoveDoors = handleRemoveDoors;
            StartCoroutine(IPanToMonolith());
        }

        public void RegisterDoor(MonolithDoor door) {
            if (!doors.Contains(door)) doors.Add(door);
        }

        public void RegisterPylon(Pylon pylon) {
            if (!pylons.Contains(pylon)) pylons.Add(pylon);
        }

        public void ReportPylonDeath(Pylon pylon) {
            if (pylons.Contains(pylon)) pylons.Remove(pylon);
            if (pylons.Count == 0) HandleAllPylonsKilled();
        }

        void HandleAllPylonsKilled() {
            if (didReportAllPylonsKilled) return;
            didReportAllPylonsKilled = true;
            actorHealth.SetIsInvulnerable(false);
            PanToMonolith();
        }

        void RemoveAllDoors() {
            foreach (var door in doors) {
                door.Remove();
            }
        }

        public override Region GetRegion() {
            return null;
        }

        public override void OnHealthGained(float amount, float hp) {
            // do nothing
        }

        public override void OnDamageTaken(float damage, float hp) {
            CommonDamageActions();
            if (!didBreakCasing && hp < brokenCasingThreshold) {
                didBreakCasing = true;
                brokenFX.Play();
            }
        }

        public override void OnDamageGiven(float damage, bool wasKilled) {
            // and no D's were given on that day
        }

        public override void OnDeath(float damage, float hp) {
            CommonDeathActions();
            StartCoroutine(IMonolithDestroyedCutscene());
        }

        void AfterMonolithDeath() {
            eventChannel.OnMonolithDeath.Invoke(type);
            eventChannel.OnApplyUpgrade.Invoke(upgradeType);
        }

        // in a larger scale project this would def be a generalized method
        IEnumerator IPanToMonolith() {
            var target = new GameObject("CutsceneCameraTarget");

            prevCameraTarget = virtualCamera.LookAt;
            target.transform.position = prevCameraTarget.position;
            playerPosition = prevCameraTarget.position;
            monolithPosition = transform.position;

            float tempDeadZoneWidth = 0f;
            float tempDeadZoneHeight = 0f;
            float tempLookaheadTime = 0f;
            var body = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
            if (body is CinemachineFramingTransposer) {
                var framingTransposer = body as CinemachineFramingTransposer;
                tempDeadZoneWidth = framingTransposer.m_DeadZoneWidth;
                tempDeadZoneHeight = framingTransposer.m_DeadZoneHeight;
                tempLookaheadTime = framingTransposer.m_LookaheadTime;
                framingTransposer.m_DeadZoneWidth = 0f;
                framingTransposer.m_DeadZoneHeight = 0f;
                framingTransposer.m_LookaheadTime = 0f;
            }

            virtualCamera.LookAt = target.transform;
            virtualCamera.Follow = target.transform;
            brain.m_IgnoreTimeScale = true;
            Time.timeScale = 0.1f;

            cutsceneTimer.SetDuration(timePanToMonolith);
            cutsceneTimer.Start();

            while (cutsceneTimer.active) {
                target.transform.position = Vector2.Lerp(playerPosition, monolithPosition, Easing.InOutQuad(cutsceneTimer.value));
                cutsceneTimer.Tick();
                yield return null;
            }

            yield return new WaitForSecondsRealtime(timePause * 0.2f);

            RemoveAllDoors();
            if (OnRemoveDoors != null) OnRemoveDoors.Invoke();

            yield return new WaitForSecondsRealtime(timePause * 0.2f);

            cutsceneTimer.SetDuration(timePanFromMonolith);
            cutsceneTimer.Start();

            while (cutsceneTimer.active) {
                target.transform.position = Vector2.Lerp(monolithPosition, playerPosition, Easing.InOutQuad(cutsceneTimer.value));
                cutsceneTimer.Tick();
                yield return null;
            }

            if (body is CinemachineFramingTransposer) {
                var framingTransposer = body as CinemachineFramingTransposer;
                framingTransposer.m_DeadZoneWidth = tempDeadZoneWidth;
                framingTransposer.m_DeadZoneHeight = tempDeadZoneHeight;
                framingTransposer.m_LookaheadTime = tempLookaheadTime;
            }

            virtualCamera.LookAt = prevCameraTarget;
            virtualCamera.Follow = prevCameraTarget;
            Time.timeScale = 1f;
            brain.m_IgnoreTimeScale = false;

            Destroy(target);
        }

        IEnumerator IMonolithDestroyedCutscene() {
            // TODO: ADD CUTSCENE
            yield return null;
            AfterMonolithDeath();
        }

        void OnGUI() {
            if (!debugCutscene) return;
            if (!IsAlive()) return;
            GUILayout.TextField(gameObject.name);
            if (GUILayout.Button("Test Pan")) {
                PanToMonolith();
            }
            if (GUILayout.Button("Destroy")) {
                TakeDamage(Actor.Constants.INSTAKILL, Vector2.zero);
            }
        }
    }
}
