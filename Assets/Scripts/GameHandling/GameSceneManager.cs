using System.Collections.Generic;
using System;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Linq;

using Random = UnityEngine.Random;

using UnityEngine.UI;


public class GameSceneManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private NetworkPrefabRef ghostPrefab;
    [SerializeField] private NetworkPrefabRef gameStateHandlerPrefab;
    [SerializeField] private int ghostsPerArea;
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private Text versionNumber;
    [SerializeField] private Text playerNameInput;
    [SerializeField] private Text playerNameUI;
    [SerializeField] private GameObject spawnLocationsHolder;

    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private Dictionary<PlayerRef, int> playerSkins = new Dictionary<PlayerRef, int>();
    private IEnumerable<Bounds> ghostsSpawnAreas;


    void Awake()
    {
        runner.AddCallbacks(this);

        ghostsSpawnAreas = FindObjectsOfType<GameObject>()
            .Where(gameObject => gameObject.tag == "World")
            .First()
            .GetComponentsInChildren<Collider>()
            .Where(collider => collider.gameObject.tag == "SpawnArea")
            .Select(collider => collider.bounds);
    }

    void Start()
    {
        if (startScreen != null)
        {
            if (versionNumber != null)
            {
                versionNumber.text = "Version " + Application.version;
            }
            startScreen.SetActive(true);
        }
        if (gameUI != null)
        {
            gameUI.SetActive(false);
        }
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Transform[] spawnLocations = spawnLocationsHolder.GetComponentsInChildren<Transform>();
        int numberOfPlayers = spawnedCharacters.Count;

        runner.Spawn(playerPrefab, spawnLocations[numberOfPlayers].position, Quaternion.identity, player);

        if (startScreen != null)
        {
            startScreen.SetActive(false);
        }
        if (gameUI != null)
        {
            gameUI.SetActive(true);
        }

        Debug.Log("Player " + player + " joined the lobby");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedCharacters.Remove(player);
        }

        if (runner.IsServer)
        {
            playerSkins.Remove(player);
        }
        Debug.Log("Player " + player + " left the lobby");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        foreach (Bounds ghostSpawnArea in ghostsSpawnAreas)
        {
            for(int i=0; i< ghostsPerArea; i++)
            {
                Vector3 spawnPosition = RandomCoordinates.FromBoundsAndY(ghostSpawnArea, 0);
                NetworkObject ghostObject = runner.Spawn(ghostPrefab, spawnPosition, Quaternion.identity);
                ghostObject.GetComponent<GhostBehaviour>().destinationBounds = ghostSpawnArea;
            }
        }

        FindObjectOfType<VictoryDetection>().networkRunner = runner;
        runner.Spawn(gameStateHandlerPrefab, Vector3.zero);
    }

    public NetworkObject GetMyPlayerObject()
    {
        if (!runner.IsPlayer) return null;
        if (spawnedCharacters.TryGetValue(runner.LocalPlayer, out NetworkObject networkObject)) {

            return networkObject;
        }
        
        return null;
    }

    public void RetrieveSkin(PlayerRef playerRef,PlayerController playerController)
    {
        if (runner.IsServer)
        {
            
            var skin = Random.Range(0, 4);
            if (playerSkins.Count > 3)
            {
                Debug.LogError("No skins available: ");
                return;
            }
            while (playerSkins.ContainsValue(skin))
            {
                skin += 1;
                if (skin == 4)
                {
                    skin = 0;
                }
            }
            playerSkins[playerRef] = skin;
            playerController.skin = skin;
            
            
        }
        playerController.ApplySkin();
    }

    public void AddToDirectory(PlayerRef playerRef,NetworkObject networkObject)
    {
        spawnedCharacters[playerRef] = networkObject;
    }

    public void doExitGame()
    {
        Application.Quit();
    }

    public void ShowPlayerName()
    {
        playerNameUI.text = playerNameInput.text;
    }

    public void RetrieveName(PlayerController player)
    {
        player.RPC_UpdatePlayerName(playerNameUI.text);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
}
