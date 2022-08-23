using UnityEngine;

namespace Game {

    public class SystemUtils {

        public static T ManageSingleton<T>(T instance = null, T incoming = null, bool shouldSetDontDestroyOnLoad = true) where T : UnityEngine.MonoBehaviour {
            var objectsInScene = Object.FindObjectsOfType<T>();
            if (objectsInScene.Length > 1 && instance != incoming) {
                Object.Destroy(incoming.gameObject);
                return instance;
            } else {
                if (shouldSetDontDestroyOnLoad) Object.DontDestroyOnLoad(incoming.gameObject);
                return incoming;
            }
        }

        public static void CleanupSingleton<T>(T instance = null) where T : UnityEngine.MonoBehaviour {
            if (instance != null) {
                Object.Destroy(instance.gameObject);
            }
            GameObject[] items = Object.FindObjectsOfType<T>() as GameObject[];
            foreach (var item in items) {
                if (item != null && item.gameObject != null) {
                    Object.Destroy(item.gameObject);
                }
            }
        }
    }
}