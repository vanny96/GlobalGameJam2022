using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSightTrigger : MonoBehaviour
{
    public TreasureHolder ActiveTreasureHolder { get; set; }
    [SerializeField] private TreasureHolder ThisTreasureHolder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();
        if (treasureHolder == null ||
            treasureHolder == ThisTreasureHolder ||
            treasureHolder == ActiveTreasureHolder)
        {
            return;
        }

        if (ActiveTreasureHolder == null)
        {
            ActiveTreasureHolder = treasureHolder;
        }

        if(ActiveTreasureHolder != null)
        {
            int currentTargetTreasure = ActiveTreasureHolder.treasure;
            int newTreasureHolderTreasure = treasureHolder.treasure;

            if(newTreasureHolderTreasure > currentTargetTreasure)
            {
                ActiveTreasureHolder = treasureHolder;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();
        if (treasureHolder == ActiveTreasureHolder)
        {
            ActiveTreasureHolder = null;
        }
    }
}
