using UnityEngine;

public class Region : MonoBehaviour {
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

    void Start() {
        // sr.enabled = false;
    }

    void Update() {

    }

    void Activate() {
        sr.color = activeColor;
    }

    void Deactivate() {
        sr.color = inactiveColor;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Activate();
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Deactivate();
        }
    }
}