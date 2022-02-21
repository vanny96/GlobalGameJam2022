using UnityEngine;
using Fusion;

public class VictoryDetection : SimulationBehaviour
{

    [SerializeField] private Transform WinLocator;
    [SerializeField] private float LerpSpeed = 0.1f;

    private bool winnerFound;
    private Transform winner;

    private void Update()
    {
        if (winnerFound)
        {
            if (winner != null && winner.localPosition != Vector3.zero)
            {
                winner.localPosition = Vector3.Lerp(winner.localPosition, Vector3.zero, LerpSpeed);
            }
            if (winner.localPosition == Vector3.zero)
            {
                winner.gameObject.GetComponent<PlayerController>().Win();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        PlayerController playerController = other.GetComponentInParent<PlayerController>();

        if(playerController != null && playerController.isBeacon)
        {
            playerController.canMove = false;

            winner = playerController.transform;
            winner.parent = WinLocator.transform;
            winner.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
            winner.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            winnerFound = true;

            FindObjectOfType<DataHolder>().AddData("playerName", playerController.playerName);
        }
    }
}
