using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class NetworkStarter : MonoBehaviour
{
    private NetworkRunner runner;

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
        StartGameArgs gameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "DesignRoom2",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneObjectProvider = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        await runner.StartGame(gameArgs);
    }
}
