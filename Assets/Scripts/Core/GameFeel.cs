using System.Collections;
using UnityEngine;
using XInputDotNetPure;

namespace Core {

    public class GameFeel : MonoBehaviour {
        [SerializeField] EventChannelSO eventChannel;

        Coroutine ieFreezeTime;
        Coroutine ieShakeGamepad;

        void OnEnable() {
            eventChannel.OnFreezeTime.Subscribe(OnFreezeTime);
            eventChannel.OnShakeGamepad.Subscribe(OnShakeGamepad);
        }

        void OnDisable() {
            eventChannel.OnFreezeTime.Unsubscribe(OnFreezeTime);
            eventChannel.OnShakeGamepad.Unsubscribe(OnShakeGamepad);
            ResetGamepadShake();
        }

        void OnFreezeTime(float duration = 0.1f, float timeScale = 0f) {
            if (ieFreezeTime != null) StopCoroutine(ieFreezeTime);
            ieFreezeTime = StartCoroutine(FreezeTime(duration, timeScale));
        }

        void OnShakeGamepad(float duration = 0.1f, float intensity = 0.5f) {
            if (ieShakeGamepad != null) StopCoroutine(ieShakeGamepad);
            ieShakeGamepad = StartCoroutine(ShakeGamepad(duration, intensity, intensity));
        }

        IEnumerator FreezeTime(float duration = 0.1f, float timeScale = 0f) {
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }

        IEnumerator ShakeGamepad(float duration = 0.1f, float leftMotor = 0.5f, float rightMotor = 0.5f) {
            GamePad.SetVibration(0, leftMotor, rightMotor);
            yield return new WaitForSecondsRealtime(duration);
            GamePad.SetVibration(0, 0, 0);
            yield return null;
        }

        void ResetGamepadShake() {
            if (ieShakeGamepad != null) StopCoroutine(ieShakeGamepad);
            GamePad.SetVibration(0, 0, 0);
        }
    }
}
