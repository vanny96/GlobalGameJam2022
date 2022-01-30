using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkStarter : MonoBehaviour
{
    private NetworkRunner runner;
    [SerializeField]
    private Text hostNameInput; 
    private void Awake()
    {
        runner = GetComponent<NetworkRunner>();
        runner.ProvideInput = true;
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
        var hostName = "testroom";

        if (!string.IsNullOrEmpty(hostNameInput.text))
        {
            hostName = hostNameInput.text;
        }

        StartGameArgs gameArgs = new StartGameArgs()
        {
            GameMode = mode,

            SessionName = hostName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };
        Debug.LogError(gameArgs.SessionName);
        await runner.StartGame(gameArgs);
    }
}
