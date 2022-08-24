using UnityEngine;
using UnityEngine.UI;

// This slider will display while the
// player is holding down the USE key -
// this is to give a short cooldown so
// that it's less likely for the player
// to accidentally pick stuff up.

namespace Interactable {

    public class InteractableProgressBar : MonoBehaviour {

        Slider slider;

        public void SetValue(float value) {
            if (slider == null) return;
            slider.value = value;
        }

        void Awake() {
            slider = GetComponent<Slider>();
        }

        void Start() {
            if (slider == null) return;
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }
    }
}


