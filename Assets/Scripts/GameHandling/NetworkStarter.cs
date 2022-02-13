using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Fusion.Sockets;
using System.Linq;

public class NetworkStarter : SimulationBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private Text hostNameInput;
    [SerializeField] private GameObject errorPopupPanel;

    private NetworkRunner runner;
    private HashSet<string> sessionNames;

    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
        runner.ProvideInput = true;
    }

    private void Start()
    {
        runner.JoinSessionLobby(SessionLobby.ClientServer);
    }

    public void StartAsHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartAsGuest()
    {
        
        StartGame(GameMode.Client);
    }

    public async void StartGame(GameMode mode)
    {
        string sessionName = GetSessionNameName();
        List<string> errors = ValidateSessionName(sessionName, mode);

        if (errors.Count == 0)
        {
            StartGameArgs gameArgs = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = sessionName,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await runner.StartGame(gameArgs);
        }
        else
        {
            ShowErrorMessages(errors);
        }
    }

    private string GetSessionNameName()
    {
        string sessionName = "testroom";

        if (!string.IsNullOrEmpty(hostNameInput.text))
        {
            sessionName = hostNameInput.text;
        }

        return sessionName;
    }

    private List<string> ValidateSessionName(string sessionName, GameMode mode)
    {
        List<string> errors = new List<string>();

        if(sessionNames == null)
        {
            errors.Add("Still retrieving sessions available");
            return errors;
        }

        bool sessionExists = sessionNames.Contains(sessionName);

        if(mode == GameMode.Host)
        {
            if (sessionExists)
                errors.Add("Session name is already taken");
        }

        if(mode == GameMode.Client)
        {
            if (!sessionExists)
                errors.Add("Session does not exist");
        }

        return errors;
    }

    private void ShowErrorMessages(List<string> errors)
    {
        errorPopupPanel.SetActive(true);

        TextMeshProUGUI errorTextField = errorPopupPanel.GetComponentsInChildren<TextMeshProUGUI>()[0];
        errorTextField.text = errors.Aggregate("", (acc, error) => acc += $"- {error}\n\r");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        sessionNames = new HashSet<string>(sessionList.Map(session => session.Name));
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, System.ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}
