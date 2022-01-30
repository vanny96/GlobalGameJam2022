using System;
using UnityEngine;
using Fusion;

public class TreasureHolderTrigger : MonoBehaviour
{
    public TreasureHolder ActiveTreasureHolder { get; set; }

    [SerializeField] private TreasureHolder ThisTreasureHolder;

    [SerializeField] private bool outlining;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material normalMaterial;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();

        if (treasureHolder != null &&
            treasureHolder != ThisTreasureHolder &&
            ActiveTreasureHolder == null)
        {
            ActiveTreasureHolder = treasureHolder;
            if (outlining) ApplyMaterial(outlineMaterial);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "ActualCollider") return;

        TreasureHolder treasureHolder = other.GetComponentInParent<TreasureHolder>();
        if (treasureHolder == ActiveTreasureHolder)
        {
            if (outlining) ApplyMaterial(normalMaterial);
            ActiveTreasureHolder = null;
        }
    }

    private void ApplyMaterial(Material material)
    {
        if (GetComponentInParent<NetworkObject>().HasInputAuthority)
        {
            SpriteRenderer targetRenderer = ActiveTreasureHolder.GetComponentInChildren<SpriteRenderer>();
            targetRenderer.material = material;
        }
    }
}
