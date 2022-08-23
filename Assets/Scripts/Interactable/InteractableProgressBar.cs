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
            slider.value = value;
        }

        void Start() {
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }

    }
}


