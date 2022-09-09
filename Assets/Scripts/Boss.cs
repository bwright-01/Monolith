using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

using Actor;
using Core;
using Weapon;
using Movement;
using Player;

public class Boss : MonoActor {

    [SerializeField] Gun gun;
    [SerializeField] Slider healthSlider;

    [Space]
    [Space]

    [SerializeField][Range(0f, 30f)] float timeWaitBeforeInitialAttack = 0f;
    [SerializeField][Range(0f, 30f)] float timeWaitAfterAttack = 2f;
    [SerializeField][Range(0f, 10f)] float timeWaitVariance = 0.2f;
    [SerializeField][Range(0f, 1f)] float timeBetweenShots = 1f;
    [SerializeField][Range(0, 10)] int numShotsInARow = 4;

    [Space]
    [Space]

    [SerializeField] Animator animator;
    [SerializeField][Range(0f, 4f)] float timePanToMonolith = 2f;
    [SerializeField][Range(0f, 4f)] float timePause = 1.5f;
    [SerializeField][Range(0f, 4f)] float timePanFromMonolith = 1f;

    [Space]
    [Space]

    [SerializeField][Range(0f, 4f)] float timeBeforeChargePlayer = 10f;

    [Space]
    [Space]

    [SerializeField] GameObject deathFX;

    [Space]
    [Space]

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] CinemachineBrain brain;

    // cached
    ActorMovement movement;
    PlayerMain player;

    // cutscene
    Vector2 playerPosition;
    Vector2 bossPosition;
    Transform prevCameraTarget;
    Timer cutsceneTimer = new Timer(TimerDirection.Increment, TimerStep.UnscaledDeltaTime);

    Timer chargeTimer = new Timer();

    bool hasStartedBattle;

    void OnEnable() {
        SubscribeToEvents();
        eventChannel.OnPlayerSpawned.Subscribe(OnPlayerSpawned);
    }

    void OnDisable() {
        UnsubscribeFromEvents();
        eventChannel.OnPlayerSpawned.Unsubscribe(OnPlayerSpawned);
    }

    void Awake() {
        Init();
        movement = GetComponent<ActorMovement>();
        movement.enabled = false;
        animator.enabled = false;
    }

    void OnPlayerSpawned(Player.PlayerMain incoming) {
        if (incoming == null || !incoming.IsAlive()) return;
        player = incoming;
        chargeTimer.SetDuration(timeBeforeChargePlayer);
        chargeTimer.Start();
        if (!hasStartedBattle) {
            hasStartedBattle = true;
            StartCoroutine(IPanToMonolith());
            StartCoroutine(IAttack());
        }
    }

    public override Region GetRegion() {
        return null;
    }

    public override void OnDamageGiven(float damage, bool wasKilled) {
        // nada
    }

    public override void OnDamageTaken(float damage, float hp) {
        CommonDamageActions();
    }

    public override void OnDeath(float damage, float hp) {
        CommonDeathActions();
        deathFX.SetActive(true);
        eventChannel.OnStopMusic.Invoke();
        Destroy(healthSlider.gameObject);
        StartCoroutine(IWin());
    }

    public override void OnHealthGained(float amount, float hp) {
        // nada
    }

    void GotoWinScreen() {
        SceneManager.LoadScene("WinScreen");
    }

    private void Update() {
        UpdateHealthUI();
        HandleChargePlayer();
        chargeTimer.Tick();
    }

    void UpdateHealthUI() {
        if (healthSlider != null) {
            healthSlider.value = actorHealth.healthPercentage;
        }
    }

    void HandleChargePlayer() {
        if (!IsAlive()) return;
        if (chargeTimer.tEnd) {
            rigidbody.AddForce(transform.up * 5f);
            chargeTimer.Start();
        }
    }

    float GetTimeWaitAfterAttack() {
        return Utils.RandomVariance(timeWaitAfterAttack, timeWaitVariance, timeWaitAfterAttack * 0.5f, timeWaitAfterAttack * 2f);
    }

    void TargetPlayer() {
        if (!IsAlive()) return;
        if (player != null && player.IsAlive()) {
            movement.SetTarget(player.transform);
        }
    }

    IEnumerator IWin() {
        yield return new WaitForSeconds(3f);
        eventChannel.OnResetMusic.Invoke();
        GotoWinScreen();
    }

    IEnumerator IAttack() {
        yield return new WaitForSeconds(timeWaitBeforeInitialAttack);
        movement.gameObject.SetActive(true);
        movement.enabled = true;
        TargetPlayer();
        while (IsAlive()) {
            TargetPlayer();
            for (int i = 0; i < numShotsInARow; i++) {
                gun.TryAttack();
                TargetPlayer();
                yield return new WaitForSeconds(timeBetweenShots);
            }
            TargetPlayer();
            yield return new WaitForSeconds(timeWaitAfterAttack);
        }
        movement.enabled = false;
    }

    IEnumerator IPanToMonolith() {
        var target = new GameObject("CutsceneCameraTarget");

        prevCameraTarget = virtualCamera.LookAt;
        target.transform.position = prevCameraTarget.position;
        playerPosition = prevCameraTarget.position;
        bossPosition = transform.position;

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
            target.transform.position = Vector2.Lerp(playerPosition, bossPosition, Easing.InOutQuad(cutsceneTimer.value));
            cutsceneTimer.Tick();
            yield return null;
        }

        animator.enabled = true;

        yield return new WaitForSecondsRealtime(timePause);

        cutsceneTimer.SetDuration(timePanFromMonolith);
        cutsceneTimer.Start();

        while (cutsceneTimer.active) {
            target.transform.position = Vector2.Lerp(bossPosition, playerPosition, Easing.InOutQuad(cutsceneTimer.value));
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
}
