using UnityEngine;

using Audio.Sound;

namespace Environment {

    public class Pulser : MonoBehaviour {
        [SerializeField] SingleSound pulseSound;
        [SerializeField] SingleSound humSound;

        Animator animator;
        Actor.Health health;

        // animation trigger
        public void PlayPulseSound() {
            pulseSound.Play(this);
        }

        void OnEnable() {
            if (health != null) health.OnDeath.Subscribe(OnDeath);
        }

        void OnDisable() {
            if (health != null) health.OnDeath.Unsubscribe(OnDeath);
        }

        void Awake() {
            health = GetComponentInParent<Actor.Health>();
            animator = GetComponent<Animator>();

            pulseSound.Init(this);
            humSound.Init(this);
        }

        void SetLayerByIndex(int index = 0) {
            for (int i = 0; i < 3; i++) {
                animator.SetLayerWeight(i, i == index ? 1 : 0);
            }
        }

        void Start() {
            humSound.Play(this);
        }

        void OnDeath(float damage, float hp) {
            animator.StopPlayback();
            animator.enabled = false;
            humSound.Stop(this);
            pulseSound.Stop(this);
        }
    }
}
