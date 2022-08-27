using UnityEngine;

using Core;

namespace Map {

    public enum MinimapPointType {
        Player,
        Monolith,
        Enemy,
    }

    public class Minimap : MonoBehaviour {
        [SerializeField][Range(0f, 100f)] float _fieldOfView = 5f;
        [SerializeField][Range(0f, 1f)] float _refreshFrequency = 0.3f;

        [Space]
        [Space]

        [SerializeField] Canvas canvas;
        [SerializeField] GameObject minimap;
        [SerializeField] GameObject mapPoints;

        [Space]
        [Space]

        [SerializeField] EventChannelSO eventChannel;

        // cached
        Player.PlayerMain player;
        RectTransform rectTransform;

        // props
        public float width { get; private set; }
        public float height { get; private set; }
        public float fieldOfView => _fieldOfView;
        public float refreshFrequency => _refreshFrequency;

        public GameObject AddMapPoint(GameObject mapPointPrefab) {
            return Instantiate(mapPointPrefab, mapPoints.transform);
        }

        void OnEnable() {
            eventChannel.OnPlayerSpawned.Subscribe(OnPlayerSpawned);
            eventChannel.OnPlayerDeath.Subscribe(OnPlayerDeath);
        }

        void OnDisable() {
            eventChannel.OnPlayerSpawned.Unsubscribe(OnPlayerSpawned);
            eventChannel.OnPlayerDeath.Unsubscribe(OnPlayerDeath);
            Reset();
        }

        void Awake() {
            Reset();
            rectTransform = minimap.GetComponent<RectTransform>();
            width = rectTransform.rect.width;
            height = rectTransform.rect.height;
        }

        void Activate() {
            canvas.enabled = true;
        }

        void Reset() {
            canvas.enabled = false;
        }

        void OnPlayerSpawned(Player.PlayerMain incoming) {
            if (incoming == null || !incoming.IsAlive()) return;
            player = incoming;
            Activate();
        }

        void OnPlayerDeath() {
            Reset();
        }
    }
}
