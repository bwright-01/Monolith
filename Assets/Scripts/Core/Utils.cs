using System.Collections;
using UnityEngine;

namespace Core {

    public class Utils {

        public static string ToTimeString(float t) {
            string minutes = Mathf.Floor(t / 60).ToString("0");
            string seconds = (t % 60).ToString("00");
            return string.Format("{0}:{1}", minutes, seconds);
        }

        public static Vector2 RandomVector2(float magnitude = 1f) {
            return new Vector2(UnityEngine.Random.Range(0f, magnitude), UnityEngine.Random.Range(0f, magnitude)).normalized;
        }

        public static float RandomMultiplier(float initialValue, float variance = 0f, float min = float.MinValue, float max = float.MaxValue) {
            if (variance <= 0) return initialValue;
            return Mathf.Clamp(initialValue * (1 + UnityEngine.Random.Range(-variance / 2f, variance / 2f)), min, max);
        }

        public static float RandomVariance(float initialValue, float variance = 0f, float min = float.MinValue, float max = float.MaxValue) {
            if (variance <= 0) return initialValue;
            return Mathf.Clamp(initialValue + UnityEngine.Random.Range(-variance / 2f, variance / 2f), min, max);
        }

        public static bool RandomBool(float threshold = 0.5f) {
            return Random.value > threshold;
        }

        public static Vector2 GetNearestCardinal(Vector2 direction) {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
                return direction.x >= 0f ? Vector2.right : Vector2.left;
            } else {
                return direction.y >= 0f ? Vector2.up : Vector2.down;
            }
        }

    }
}
