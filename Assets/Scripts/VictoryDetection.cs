using UnityEngine;
using Fusion;

public class VictoryDetection : SimulationBehaviour
{
    public NetworkRunner networkRunner;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        PlayerController playerController = other.GetComponentInParent<PlayerController>();

        if(playerController != null && playerController.isBeacon)
        {
            networkRunner.SetActiveScene(2);
        }
    }
}
