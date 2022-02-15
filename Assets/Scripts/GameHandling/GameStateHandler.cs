using UnityEngine;
using Fusion;

public class GameStateHandler : NetworkBehaviour
{
    public NetworkBool GameStarted { get; private set; } = false;

    public void StartGame()
    {
        GameStarted = true;
        RemoveStartColliders();
    }

    private void RemoveStartColliders()
    {
        GameObject startGameCollidersHolder = GameObject.Find("StartColliders");

        foreach(NetworkObject collider in startGameCollidersHolder.GetComponentsInChildren<NetworkObject>())
        {
            Runner.Despawn(collider);
        }

    }
}
