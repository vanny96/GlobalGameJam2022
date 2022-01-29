using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureHolderTrigger : MonoBehaviour
{
    public TreasureHolder ActiveTreasureHolder { get; set; }
    [SerializeField] private TreasureHolder ThisTreasureHolder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();

        if (treasureHolder != null &&
            treasureHolder != ThisTreasureHolder &&
            ActiveTreasureHolder == null)
        {
            ActiveTreasureHolder = treasureHolder;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();
        if(treasureHolder == ActiveTreasureHolder)
        {
            ActiveTreasureHolder = null;
        }
    }
}
