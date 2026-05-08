using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityScene = UnityEngine.SceneManagement.Scene;
using UnityLoadSceneMode = UnityEngine.SceneManagement.LoadSceneMode;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Positions by Team")]
    [SerializeField] private Transform spawnPointTeamA;
    [SerializeField] private Transform spawnPointTeamB;

    [Header("Settings")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private float retrySpawnInterval = 0.5f;

    private bool hasSpawned = false;
    private float nextSpawnRetryTime = 0f;

    private void OnEnable()
    {
        UnitySceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnitySceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!spawnOnStart) return;
        TrySpawnPlayer();
    }

    private void Update()
    {
        if (!spawnOnStart || hasSpawned || !PhotonNetwork.InRoom) return;

        if (Time.unscaledTime < nextSpawnRetryTime) return;

        nextSpawnRetryTime = Time.unscaledTime + retrySpawnInterval;
        TrySpawnPlayer();
    }

    public override void OnJoinedRoom()
    {
        hasSpawned = false;
        nextSpawnRetryTime = 0f;

        if (PhotonNetwork.IsMasterClient)
        {
            PlayerTeamAssigner.AssignTeamsToAllPlayers();
        }

        if (!spawnOnStart) return;
        TrySpawnPlayer();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PlayerTeamAssigner.AssignTeamToPlayer(newPlayer);
        }
    }

    public override void OnLeftRoom()
    {
        hasSpawned = false;
        nextSpawnRetryTime = 0f;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer.IsLocal && !hasSpawned && changedProps.ContainsKey("Team"))
        {
            TrySpawnPlayer();
        }
    }

    private void OnSceneLoaded(UnityScene scene, UnityLoadSceneMode mode)
    {
        hasSpawned = false;
        nextSpawnRetryTime = 0f;

        if (!spawnOnStart) return;
        TrySpawnPlayer();
    }

    public void SpawnPlayer()
    {
        hasSpawned = false;
        nextSpawnRetryTime = 0f;
        TrySpawnPlayer();
    }

    private void TrySpawnPlayer()
    {
        if (hasSpawned) return;
        if (!PhotonNetwork.InRoom) return;

        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] Sin playerPrefab asignado.");
            return;
        }

        if (HasLocalPlayerInstance())
        {
            hasSpawned = true;
            return;
        }


        if (PhotonNetwork.IsMasterClient)
        {
            PlayerTeamAssigner.AssignTeamsToAllPlayers();
        }


        PlayerTeam? localTeam = PlayerTeamAssigner.GetLocalPlayerTeam();
        if (localTeam == null)
        {

            return;
        }

        Vector3 spawnPosition = GetSpawnPosition(localTeam.Value);

        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition + Vector3.up/2, Quaternion.identity);
        hasSpawned = true;

        Log.Info($"[PlayerSpawner] Spawned en {localTeam.Value} → {spawnPosition}");
    }

    private Vector3 GetSpawnPosition(PlayerTeam team)
    {
        switch (team)
        {
            case PlayerTeam.TeamA:
                return spawnPointTeamA != null ? spawnPointTeamA.position : transform.position;

            case PlayerTeam.TeamB:
                return spawnPointTeamB != null ? spawnPointTeamB.position : transform.position;

            default:
                return transform.position;
        }
    }

    private bool HasLocalPlayerInstance()
    {
        PhotonView[] views = FindObjectsOfType<PhotonView>();
        for (int i = 0; i < views.Length; i++)
        {
            PhotonView view = views[i];
            if (view == null || !view.IsMine) continue;

            if (view.gameObject.name.StartsWith(playerPrefab.name))
            {
                return true;
            }
        }
        return false;
    }
}
