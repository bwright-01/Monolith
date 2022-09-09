using System.Collections;
using Core;
using UnityEngine;

namespace Movement {

    [RequireComponent(typeof(ActorMovement))]

    public class Wander : MonoBehaviour {

        [SerializeField] bool debug;
        [SerializeField] Sprite debugSprite;
        [SerializeField] Material debugMaterial;

        [Space]
        [Space]

        [SerializeField] bool disableOnAwake = true;

        [Space]
        [Space]

        [SerializeField][Range(0f, 50f)] float wanderRadius = 10f;
        [SerializeField][Range(0f, 1f)] float wanderEdgeThickness = 0.1f;
        [SerializeField][Range(0f, 5f)] float wanderTargetThreshold = 1f;
        [SerializeField][Range(0f, 5f)] float wanderMinDistance = 2f;
        [SerializeField][Range(0f, 30f)] float wanderTimeout = 10f;

        Actor.iActor actor;
        ActorMovement movement;

        Vector2 initialPosition;
        Vector2 lastTargetPosition;
        GameObject temp;
        GameObject target;
        SpriteRenderer targetSprite;

        float t;
        int tries;

        void OnEnable() {
            StopAllCoroutines();
            StartCoroutine(IWander());
        }

        void OnDisable() {
            StopAllCoroutines();
            movement.Halt();
        }

        private void OnDestroy() {
            Destroy(target.gameObject);
        }

        void Awake() {
            initialPosition = transform.position;
            actor = GetComponent<Actor.iActor>();
            movement = GetComponent<ActorMovement>();
            InitTarget();
            if (disableOnAwake) enabled = false;
        }

        void Update() {
            targetSprite.enabled = debug;
        }

        void InitTarget() {
            temp = new GameObject("WanderTarget");
            target = Instantiate(temp, transform.position, Quaternion.identity);
            target.name = "WanderTarget";
            targetSprite = target.AddComponent<SpriteRenderer>();
            targetSprite.sprite = debugSprite;
            targetSprite.material = debugMaterial;
            targetSprite.color = Utils.Transparentize(Color.cyan, 0.7f);
            targetSprite.enabled = false;
            Destroy(temp);
        }

        Vector2 GetWanderPoint() {
            return initialPosition + UnityEngine.Random.insideUnitCircle * wanderRadius;
        }

        Vector2 GetRandomUnitRing() {
            return UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(1f - wanderEdgeThickness, 1f);
        }

        bool IsAtTargetPosition() {
            return Vector2.Distance(transform.position, target.transform.position) <= wanderTargetThreshold;
        }

        bool IsCloseToTargetPosition() {
            return Vector2.Distance(transform.position, target.transform.position) <= Mathf.Min(wanderMinDistance, wanderRadius * (1f - wanderEdgeThickness));
        }

        IEnumerator IWander() {
            while (actor.IsAlive()) {
                t = 0;
                tries = 0;

                do {
                    target.transform.position = GetWanderPoint();
                    tries++;
                    yield return null;
                } while (IsCloseToTargetPosition() && tries < 10);

                movement.SetTarget(target.transform);

                while (!IsAtTargetPosition() && t < wanderTimeout) {
                    t += Time.deltaTime;
                    yield return null;
                }

                yield return null;
            }
        }

        void OnDrawGizmos() {
            if (!debug) return;
            Utils.DebugDrawCircle(initialPosition != Vector2.zero ? initialPosition : transform.position, wanderRadius, Utils.Transparentize(Color.yellow, 0.1f));
            if (target != null) Utils.DebugDrawRect(target.transform.position, 0.2f, Color.cyan);
        }
    }
}
