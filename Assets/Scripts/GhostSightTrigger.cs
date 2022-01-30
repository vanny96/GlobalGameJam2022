using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSightTrigger : MonoBehaviour
{
    public TreasureHolder ActiveTreasureHolder { get; set; }
    [SerializeField] private TreasureHolder ThisTreasureHolder;
    [SerializeField] private GhostBehaviour ghostBehaviour;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();
        PlayerController playerController = other.GetComponentInParent<PlayerController>();
        if (treasureHolder == null ||
            playerController == null ||
            treasureHolder == ThisTreasureHolder ||
            treasureHolder == ActiveTreasureHolder)
        {
            return;
        }

        if (ActiveTreasureHolder == null)
        {
            AddTarget(treasureHolder, playerController);
        }

        if(ActiveTreasureHolder != null)
        {
            int currentTargetTreasure = ActiveTreasureHolder.treasure;
            int newTreasureHolderTreasure = treasureHolder.treasure;

            if(newTreasureHolderTreasure > currentTargetTreasure)
            {
                AddTarget(treasureHolder, playerController);
            }
        }
    }

    private void AddTarget(TreasureHolder treasureHolder, PlayerController playerController)
    {
        ActiveTreasureHolder = treasureHolder;
        if (ghostBehaviour.Angry)
        {
            playerController.OnTargeted();
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
