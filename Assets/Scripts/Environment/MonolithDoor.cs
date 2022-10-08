using System.Collections;
using UnityEngine;

using Audio.Sound;

namespace Environment {

    public class MonolithDoor : MonoBehaviour {

        [SerializeField] ParticleSystem doorRemoveFX;
        [SerializeField] SingleSound doorRemoveSound;

        Monolith monolith;
        SpriteRenderer sr;
        new Collider2D collider;

        public void Remove() {
            StartCoroutine(IRemove());
        }

        void Awake() {
            sr = GetComponentInChildren<SpriteRenderer>();
            collider = GetComponent<Collider2D>();

            monolith = GetComponentInParent<Monolith>();
            monolith.RegisterDoor(this);
            transform.SetParent(monolith.transform.parent.parent);

            doorRemoveSound.Init(this);
        }

        void HandleRemoval() {
            sr.enabled = false;
            collider.enabled = false;
            if (doorRemoveFX != null) doorRemoveFX.Play();
            doorRemoveSound.Play(this);
        }

        IEnumerator IRemove() {
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0f, 0.4f));
            HandleRemoval();
        }
    }
}
