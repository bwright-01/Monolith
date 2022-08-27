
using System.Collections;
using UnityEngine;

using Player;
using Core;

namespace Map {

    public class MinimapComponent : MonoBehaviour {

        [SerializeField] GameObject mapPointPrefab;
        [SerializeField] Minimap minimap;

        // cached
        GameObject mapPoint;
        RectTransform rectTransform;
        PlayerMain player;
        Vector2 vectorFromPlayer;

        // state
        bool isInit;
        float fieldOfViewMod;
        Vector2 position;

        private void OnEnable() {
            if (isInit) StartCoroutine(IRenderPoint());
        }

        private void OnDisable() {
            StopAllCoroutines();
        }

        private void OnDestroy() {
            if (mapPoint != null) Destroy(mapPoint);
        }

        void Start() {
            StopAllCoroutines();
            if (minimap == null) minimap = FindObjectOfType<Minimap>(true);
            if (minimap == null || mapPointPrefab == null) {
                Destroy(gameObject);
                return;
            }
            mapPoint = minimap.AddMapPoint(mapPointPrefab);
            rectTransform = mapPoint.GetComponent<RectTransform>();
            isInit = true;
            StartCoroutine(IRenderPoint());
        }

        void Update() {
            if (minimap == null) {
                StopAllCoroutines();
                Destroy(gameObject);
                return;
            }
        }

        void RenderPoint() {
            if (minimap == null) return;
            if (!minimap.enabled) return;
            player = PlayerUtils.FindPlayer();
            if (!PlayerUtils.CheckIsAlive(player)) return;

            vectorFromPlayer = (transform.position - player.transform.position);
            position.x = vectorFromPlayer.x / minimap.fieldOfView;
            position.y = vectorFromPlayer.y / minimap.fieldOfView;
            rectTransform.anchoredPosition = position;
        }

        IEnumerator IRenderPoint() {
            while (enabled && minimap != null && minimap.enabled) {
                RenderPoint();
                yield return new WaitForSeconds(minimap.refreshFrequency);
            }
        }
    }
}
