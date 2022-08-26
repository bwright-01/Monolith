
using UnityEngine;

using Player;
using Core;

// TODO: USE RAYCAST

namespace Enemy {

    public class EnemySight : MonoBehaviour {

        [SerializeField] bool debug;
        [SerializeField] float rangeOfSight = 5f;

        PlayerMain player;

        public bool CanSeePlayer() {
            player = PlayerUtils.FindPlayer();
            if (!PlayerUtils.CheckIsAlive(player)) return false;
            return Vector2.Distance(player.transform.position, transform.position) <= rangeOfSight;
        }

        void OnDrawGizmos() {
            if (!debug) return;
            float transparency = CanSeePlayer() ? 0.5f : 0.2f;
            Utils.DebugDrawCircle(transform.position, rangeOfSight, Utils.Transparentize(Color.magenta, transparency));
        }
    }
}
