using System.Collections;
using UnityEngine;

namespace Core {

    class ScreenUtils {
        // cached state
        static Camera cachedCamera;
        static Vector2 minScreenBoundsWorld;
        static Vector2 maxScreenBoundsWorld;

        public static Camera GetCamera(Camera camera = null) {
            if (camera != null) return camera;
            if (cachedCamera == null) cachedCamera = Camera.main;
            return cachedCamera;
        }

        public static (Vector2, Vector2) GetScreenBounds(Camera camera = null, float offscreenPadding = 1f, bool forceCalc = false) {
            camera = GetCamera(camera);
            minScreenBoundsWorld = camera.ViewportToWorldPoint(Vector2.zero);
            maxScreenBoundsWorld = camera.ViewportToWorldPoint(Vector2.one);
            return (
                minScreenBoundsWorld - Vector2.one * offscreenPadding,
                maxScreenBoundsWorld + Vector2.one * offscreenPadding);
        }

        public static bool IsObjectOnScreen(GameObject obj, Camera camera = null, float offscreenPadding = 1f) {
            camera = GetCamera(camera);
            (Vector2 _minBoundsWorld, Vector2 _maxBoundsWorld) = ScreenUtils.GetScreenBounds(camera, offscreenPadding);
            return
                obj.transform.position.x > _minBoundsWorld.x &&
                obj.transform.position.x < _maxBoundsWorld.x &&
                obj.transform.position.y > _minBoundsWorld.y &&
                obj.transform.position.y < _maxBoundsWorld.y;
        }
    }
}
