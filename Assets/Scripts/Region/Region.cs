using UnityEngine;

using Core;

public class Region : MonoBehaviour, Actor.iGuid {
    [SerializeField] bool debug;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

    [Space]

    [SerializeField] Core.EventChannelSO eventChannel;

    // props
    System.Guid guid = new System.Guid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.

    public System.Guid Guid() {
        return guid;
    }

    void Start() {
        if (!debug) sr.enabled = false;
    }

    void Activate() {
        sr.color = activeColor;
        eventChannel.OnRegionActivate.Invoke(guid);
    }

    void Deactivate() {
        sr.color = inactiveColor;
        eventChannel.OnRegionDeactivate.Invoke(guid);
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
