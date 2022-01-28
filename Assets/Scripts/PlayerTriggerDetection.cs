using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerTriggerDetection : MonoBehaviour
{
    public HashSet<Collider> CollidersInTrigger { get; set; } = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        CollidersInTrigger.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            if (CollidersInTrigger.Contains(other))
                CollidersInTrigger.Remove(other);
        }
    }
}