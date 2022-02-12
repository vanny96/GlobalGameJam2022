using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class InputHandler : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runner;

    void Awake()
    {
        runner.AddCallbacks(this);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        PirateGameInput pirateGameInput = new PirateGameInput();

        pirateGameInput.Buttons.Set(PirateButtons.Forward, Input.GetKey(KeyCode.W));
        pirateGameInput.Buttons.Set(PirateButtons.Backward, Input.GetKey(KeyCode.S));
        pirateGameInput.Buttons.Set(PirateButtons.Left, Input.GetKey(KeyCode.A));
        pirateGameInput.Buttons.Set(PirateButtons.Right, Input.GetKey(KeyCode.D));
        pirateGameInput.Buttons.Set(PirateButtons.Steal, Input.GetKey(KeyCode.K));
        pirateGameInput.Buttons.Set(PirateButtons.Give, Input.GetKey(KeyCode.L));
        pirateGameInput.Buttons.Set(PirateButtons.Stun, Input.GetKey(KeyCode.J));

        input.Set(pirateGameInput);
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
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
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}

public enum PirateButtons
{
    Forward = 0,
    Backward = 1,
    Left = 2,
    Right = 3,
    Steal = 4,
    Give = 5,
    Stun = 6
}

public struct PirateGameInput : INetworkInput
{
    public NetworkButtons Buttons;
}
