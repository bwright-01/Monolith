using UnityEngine;
using System.Collections;

namespace Actor {

    public class DamageFlash : MonoBehaviour {
        [SerializeField] Material defaultMaterial;
        [SerializeField] Material damageFlashMaterial;
        [SerializeField] float damageFlashDuration = 0.1f;
        [SerializeField] SpriteRenderer damageSpriteTarget;
        [SerializeField] SpriteRenderer[] secondaryDamageSpriteTargets;

        Coroutine ieFlash;

        public void StartFlashing() {
            if (ieFlash != null) return;
            ieFlash = StartCoroutine(IEDamageFlash());
        }

        void ActivateFlash() {
            if (damageSpriteTarget != null) damageSpriteTarget.material = damageFlashMaterial;
            foreach (var sprite in secondaryDamageSpriteTargets) if (sprite != null) sprite.material = damageFlashMaterial;
        }

        void DeactivateFlash() {
            if (damageSpriteTarget != null) damageSpriteTarget.material = defaultMaterial;
            foreach (var sprite in secondaryDamageSpriteTargets) if (sprite != null) sprite.material = defaultMaterial;
        }

        IEnumerator IEDamageFlash() {
            for (int i = 0; i < 2; i++) {
                ActivateFlash();
                yield return new WaitForSeconds(damageFlashDuration);
                DeactivateFlash();
                yield return new WaitForSeconds(damageFlashDuration);
            }
            ieFlash = null;
        }
    }
}
