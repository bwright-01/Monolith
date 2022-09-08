using Core;
using UnityEngine;

using Audio.Sound;

namespace Weapon {

    public class Gun : BaseWeapon {
        [Header("Damage")]
        [Space]
        [SerializeField][Range(0f, 10f)] float damageMultiplier = 1f;

        [Header("Ammo / Firing")]
        [Space]
        [SerializeField][Range(0f, 10f)] float firingRate = 0f;
        [SerializeField][Range(2, 40)] int cycleRepeat = 2;
        [SerializeField][Tooltip("whether gun cycles shooting from various gun mounts")] bool cycles;

        [Header("Cooldown / Deployment")]
        [Space]
        [SerializeField] bool overheats = false;
        [SerializeField][Range(0f, 10f)] float overheatTime = 3f;
        [SerializeField][Range(0f, 10f)] float cooldownTime = 1.5f;

        [Header("Physics")]
        [Space]
        [SerializeField] float recoil = 0f;

        [Header("Accuracy")]
        [Space]
        [SerializeField] bool hasPerfectAccuracy = true;
        [SerializeField] float accuracyAngle = 0f;
        [SerializeField]
        [Tooltip("make sure start / end values are -1, 1 respectively")]
        AnimationCurve accuracyCurve = AnimationCurve.Linear(0f, -1f, 1f, 1f);

        [Header("Prefab Settings")]
        [Space]
        [SerializeField] Actor.DamageDealer bulletPrefab;

        [Header("Audio")]
        [Space]
        [SerializeField] SingleSound shotSound;
        [SerializeField] SingleSound overloadedSound;

        int firingCycle = 0;
        Timer firing = new Timer();
        Timer cooldown = new Timer();
        Timer overheated = new Timer();
        Timer burstCooldown = new Timer();

        // cached
        Actor.iActor actor;
        Rigidbody2D rb;

        // state
        bool isTryingToShoot;

        // this method should ideally be called every frame
        public override bool TryAttack() {
            isTryingToShoot = true;
            return true;
        }

        void Awake() {
            actor = GetComponentInParent<Actor.iActor>();
            rb = GetComponentInParent<Rigidbody2D>();
            Init();
            if (bulletPrefab == null) Debug.LogError($"Bullet prefab null in {Utils.FullGameObjectName(gameObject)}");
            shotSound.Init(this);
            overloadedSound.Init(this);
        }

        void Update() {
            if (ShouldFire()) {
                shotSound.Play();
                SpawnProjectile(transform.position, transform.rotation);
                AfterFire();
            } else {
                AfterNoFire();
            }
            TickTimers();
        }

        bool IsCycle(int cycle) { return !cycles || firingCycle % cycleRepeat == cycle; }

        void SpawnProjectile(Vector3 origin, Quaternion rotation) {
            Actor.DamageDealer damager = Object.Instantiate(bulletPrefab, origin, rotation * GetAccuracyMod());
            damager.SetIgnoreUUID(actor.GUID());
            damager.SetDamageMultiplier(damageMultiplier);
            if (rb != null) rb.AddForce(-transform.up * recoil, ForceMode2D.Impulse);
        }

        Quaternion GetAccuracyMod() {
            return Quaternion.AngleAxis(GetAccuracyAngle(UnityEngine.Random.Range(-1f, 1f)), Vector3.forward);
        }


        void Init(bool equipped = true) {
            firingCycle = 0;
            firing.SetDuration(firingRate);
            cooldown.SetDuration(cooldownTime);
            overheated.SetDuration(overheatTime);
            firing.End();
            cooldown.End();
            if (overheats) overheated.Start(); // overheated gets ticked manually
        }

        bool ShouldFire() {
            if (!isTryingToShoot) return false;
            if (firing.active) return false;
            if (overheats && cooldown.active) return false;
            return true;
        }

        void AfterFire() {
            isTryingToShoot = false;
            firingCycle++;
            if (firingCycle > 99) firingCycle = 0;
            if (overheats) {
                overheated.Tick();
                if (overheated.tEnd) {
                    cooldown.Start();
                    overheated.Start();
                }
            }
            firing.Start();
        }

        void AfterNoFire() {
            if (overheats) {
                if (firing.active) {
                    overheated.Tick();
                } else {
                    overheated.TickReversed();
                }
            }
        }

        void TickTimers() {
            firing.Tick();
            cooldown.Tick();
        }

        float GetAccuracyAngle(float t = 0f) {
            if (hasPerfectAccuracy) return 0f;
            return accuracyAngle * accuracyCurve.Evaluate(t);
        }

        bool IsOverheated() {
            return overheats && cooldown.active;
        }
    }
}
