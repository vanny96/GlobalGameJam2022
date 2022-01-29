using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;

public class GameSceneManager : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private NetworkPrefabRef ghostPrefab;
    [SerializeField] private int ghostsPerArea;

    private Dictionary<PlayerRef, NetworkObject> spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private IEnumerable<Bounds> ghostsSpawnAreas;


    void Awake()
    {
        
        runner.AddCallbacks(this);

        Terrain terrain = FindObjectOfType<Terrain>();
        ghostsSpawnAreas = terrain.GetComponentsInChildren<Collider>()
            .Where(collider => collider.gameObject.tag == "SpawnArea")
            .Select(collider => collider.bounds);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Vector3 spawnPosition = new Vector3(0, 0, 41);
        NetworkObject playerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
        
        Debug.Log("Player " + player + " joined the lobby");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            spawnedCharacters.Remove(player);
        }

        Debug.Log("Player " + player + " left the lobby");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        foreach(Bounds ghostSpawnArea in ghostsSpawnAreas)
        {
            for(int i=0; i< ghostsPerArea; i++)
            {
                Vector3 spawnPosition = RandomCoordinates.FromBoundsAndY(ghostSpawnArea, 1);
                NetworkObject ghostObject = runner.Spawn(ghostPrefab, spawnPosition, Quaternion.identity);
                ghostObject.GetComponent<GhostBehaviour>().destinationBounds = ghostSpawnArea;
            }
        }
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

    public NetworkObject GetMyPlayerObject()
    {
        if (!runner.IsPlayer) return null;
        if (spawnedCharacters.TryGetValue(runner.LocalPlayer, out NetworkObject networkObject)) {

            return networkObject;
        }
        
        return null;
    }

    public void AddToDirectory(PlayerRef playerRef,NetworkObject networkObject)
    {
        spawnedCharacters[playerRef] = networkObject;
    }
}
