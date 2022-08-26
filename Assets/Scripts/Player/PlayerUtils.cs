
using UnityEngine;

using Core;

namespace Player {
    public class PlayerUtils {

        static GameObject cachedPlayerGO;
        static PlayerMain cachedPlayerMain;
        static Timer cacheBustInterval = new Timer(TimerDirection.Decrement, TimerStep.DeltaTime, 0.2f);

        public static PlayerMain FindPlayer() {
            if (cachedPlayerMain != null && cachedPlayerMain.IsAlive() && cachedPlayerMain.isActiveAndEnabled && cachedPlayerMain.gameObject.activeSelf) {
                return cachedPlayerMain;
            }
            if (cacheBustInterval.active) {
                cacheBustInterval.Tick();
                return null;
            }
            cacheBustInterval.Start();
            cachedPlayerGO = GameObject.FindGameObjectWithTag("Player");
            if (cachedPlayerGO == null) return null;
            cachedPlayerMain = cachedPlayerGO.GetComponentInParent<PlayerMain>();
            return cachedPlayerMain;
        }

        public static void InvalidateCache() {
            cacheBustInterval.End();
        }

        public static bool CheckIsAlive(PlayerMain player) {
            return player != null && player.IsAlive() && player.isActiveAndEnabled && player.gameObject.activeSelf;
        }
    }
}
