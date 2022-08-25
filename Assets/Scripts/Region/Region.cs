using System.Collections.Generic;
using UnityEngine;

using Core;

public class Region : MonoBehaviour, Actor.iGuid {
    [SerializeField] bool debug;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

    [Space]

    [SerializeField] EventChannelSO eventChannel;

    // props
    System.Guid guid = System.Guid.NewGuid(); // this is the unique ID used for comparing enemies, bosses, pickups, destructibles etc.

    //state
    List<Enemy.EnemyMain> enemies = new List<Enemy.EnemyMain>();

    public System.Guid GUID() {
        return guid;
    }

    public void RegisterActor(Enemy.EnemyMain enemy) {
        enemies.Add(enemy);
    }

    void OnEnable() {
        eventChannel.OnEnemyDeath.Subscribe(OnEnemyDeath);
    }

    void OnDisable() {
        eventChannel.OnEnemyDeath.Unsubscribe(OnEnemyDeath);
    }

    void Start() {
        if (!debug) sr.enabled = false;
        Deactivate();
    }

    void OnEnemyDeath(Enemy.EnemyMain enemy) {
        // TODO: FILTER ENEMY OUT OF ENEMIES LIST
    }

    void Activate() {
        sr.color = activeColor;
        eventChannel.OnRegionActivate.Invoke(guid);
        foreach (var enemy in enemies) enemy.gameObject.SetActive(true);
    }

    void Deactivate() {
        sr.color = inactiveColor;
        eventChannel.OnRegionDeactivate.Invoke(guid);
        foreach (var enemy in enemies) enemy.gameObject.SetActive(false);
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
