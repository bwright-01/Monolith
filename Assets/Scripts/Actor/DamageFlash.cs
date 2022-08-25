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

        IEnumerator IEDamageFlash() {
            if (damageSpriteTarget != null) damageSpriteTarget.material = damageFlashMaterial;
            foreach (var sprite in secondaryDamageSpriteTargets) if (sprite != null) sprite.material = damageFlashMaterial;
            yield return new WaitForSeconds(damageFlashDuration);
            if (damageSpriteTarget != null) damageSpriteTarget.material = defaultMaterial;
            foreach (var sprite in secondaryDamageSpriteTargets) if (sprite != null) sprite.material = defaultMaterial;
            ieFlash = null;
        }
    }
}
