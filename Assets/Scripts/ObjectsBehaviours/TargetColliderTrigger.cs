using System;
using UnityEngine;
using Fusion;

public class TargetColliderTrigger : MonoBehaviour
{
    public TargetCollider activeTarget { get; set; }

    [SerializeField] private TargetCollider thisTargetCollider;

    [SerializeField] private bool outlining;
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private Material normalMaterial;

    private void OnTriggerStay(Collider other)
    {
        TargetCollider targetCollider = other.GetComponent<TargetCollider>();

        if (targetCollider != null &&
            targetCollider != thisTargetCollider &&
            activeTarget == null)
        {
            activeTarget = targetCollider;
            if (outlining) ApplyMaterial(outlineMaterial);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        TargetCollider target = other.GetComponent<TargetCollider>();
        if (target != null &&
            target == this.activeTarget)
        {
            if (outlining) ApplyMaterial(normalMaterial);
            this.activeTarget = null;
        }
    }

    private void ApplyMaterial(Material material)
    {
        if (GetComponentInParent<NetworkObject>().HasInputAuthority)
        {
            SpriteRenderer renderer = activeTarget.parent.GetComponentInChildren<SpriteRenderer>();
            renderer.material = material;
        }
    }
}
