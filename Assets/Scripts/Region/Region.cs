using System.Collections.Generic;
using UnityEngine;

using Core;

public class Region : MonoBehaviour, Actor.iGuid {
    [SerializeField] bool debug;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Color inactiveColor;
    [SerializeField] Color activeColor;

    [Space]
    [Space]

    [SerializeField] string musicTrack;
    [SerializeField] bool playMusicOnAwake;

    [Space]
    [Space]

    [SerializeField] EventChannelSO eventChannel;

    // cache
    Actor.DamageReceiver cachedDamageReceiver;
    Region cachedRegion;

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
        if (playMusicOnAwake) PlayMusic();
    }

    void OnEnemyDeath(Enemy.EnemyMain enemy) {
        enemies.Remove(enemy);
    }

    void PlayMusic() {
        eventChannel.OnPlayMusic.Invoke(musicTrack);
    }

    void Activate() {
        sr.color = activeColor;
        eventChannel.OnRegionActivate.Invoke(guid);
        PlayMusic();
        // foreach (var enemy in enemies) enemy.gameObject.SetActive(true);
    }

    void Deactivate() {
        sr.color = inactiveColor;
        eventChannel.OnRegionDeactivate.Invoke(guid);
        // foreach (var enemy in enemies) enemy.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other) {
        Layer.Init();

        if (other.CompareTag("Player")) {
            Activate();
            return;
        }

        if (Layer.Enemy.Equals(other.gameObject.layer)) {
            SetEnemyRegion(other);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        Layer.Init();

        if (other.CompareTag("Player")) {
            Deactivate();
            return;
        }

        if (Layer.Enemy.Equals(other.gameObject.layer)) {
            KillEnemyIfOutsideOwnRegion(other);
        }
    }

    void SetEnemyRegion(Collider2D other) {
        cachedDamageReceiver = other.gameObject.GetComponent<Actor.DamageReceiver>();
        if (cachedDamageReceiver == null) return;
        cachedRegion = cachedDamageReceiver.GetRegion();
        if (cachedRegion != null) return;
        cachedDamageReceiver.SetRegion(this);
        RegisterActor((Enemy.EnemyMain)cachedDamageReceiver.rootActor);
    }

    void KillEnemyIfOutsideOwnRegion(Collider2D other) {
        cachedDamageReceiver = other.gameObject.GetComponent<Actor.DamageReceiver>();
        if (cachedDamageReceiver == null) return;
        cachedRegion = cachedDamageReceiver.GetRegion();
        if (cachedRegion == null) return;
        if (cachedRegion != this) return;
        cachedDamageReceiver.TakeDamage(Actor.Constants.INSTAKILL, Vector2.zero);
    }
}
