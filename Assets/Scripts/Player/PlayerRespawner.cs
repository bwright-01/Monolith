using System.Collections;
using UnityEngine;
using Cinemachine;

using Core;
using Audio.Sound;

namespace Player {

    public class PlayerRespawner : MonoBehaviour {

        [SerializeField][Range(0f, 5f)] float spawnTime = 1.5f;
        [SerializeField] bool spawnOnAwake = true;

        [Space]
        [Space]

        [SerializeField] GameObject playerPrefab;
        [SerializeField] SingleSound spawnStartSound;
        [SerializeField] SingleSound spawnEndSound;
        [SerializeField] ParticleSystem respawnFX;

        [Space]
        [Space]

        [SerializeField] CinemachineVirtualCamera virtualCamera;
        [SerializeField] EventChannelSO eventChannel;

        Vector2 respawnLocation;
        Coroutine ieRespawn;
        GameObject temp;
        GameObject spawned;

        void OnEnable() {
            eventChannel.OnRespawnPlayer.Subscribe(OnRespawnPlayer);
        }

        void OnDisable() {
            eventChannel.OnRespawnPlayer.Unsubscribe(OnRespawnPlayer);
        }

        void Start() {
            Game.GameSystems.current.state.SetRespawnPoint(transform.position);
            spawnStartSound.Init(this);
            spawnEndSound.Init(this);
            if (spawnOnAwake) OnRespawnPlayer();
        }

        void OnRespawnPlayer() {
            if (ieRespawn != null) StopCoroutine(ieRespawn);
            if (PlayerUtils.FindPlayer() != null) {
                Debug.LogWarning("Tried to respawn the player, but a player already existed in the scene.");
                return;
            }
            ieRespawn = StartCoroutine(IRespawn());
        }

        IEnumerator IRespawn() {
            respawnLocation = Game.GameSystems.current.state.respawnPoint;

            temp = new GameObject("PlayerSpawnPlaceholder");
            temp.transform.position = respawnLocation;
            virtualCamera.LookAt = temp.transform;
            virtualCamera.Follow = temp.transform;

            spawnStartSound.Play();

            if (respawnFX != null) {
                respawnFX.transform.position = respawnLocation;
                respawnFX.Play();
            }

            yield return new WaitForSeconds(spawnTime);

            spawned = Instantiate(playerPrefab, respawnLocation, Quaternion.identity);
            spawned.name = "Player";
            virtualCamera.LookAt = spawned.transform;
            virtualCamera.Follow = spawned.transform;

            spawnEndSound.Play();

            if (respawnFX != null) respawnFX.Stop();

            PlayerUtils.InvalidateCache();

            Destroy(temp);

            yield return null;
        }
    }
}
