using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Fusion;

public class StunEntity : NetworkBehaviour
{
    [Networked] private bool stunned { get; set; } = false;
    [SerializeField] private float stunDuration;
    [SerializeField] private float stunCooldown;
    [Networked] private float currentStunCooldown { get; set; } = 0;


    [SerializeField] private UnityEvent OnStunned;
    [SerializeField] private UnityEvent OnStunRemoved;


    public override void FixedUpdateNetwork()
    {
        if (currentStunCooldown >= 0) currentStunCooldown -= Runner.DeltaTime;
    }

    public void Stun(StunEntity target)
    {
        if(currentStunCooldown <= 0)
        {
            target.StartCoroutine(target.GetStunned());
            currentStunCooldown = stunCooldown;
        }
    }

    public bool IsStunned()
    {
        return stunned;
    }

    public IEnumerator GetStunned()
    {
        stunned = true;
        OnStunned.Invoke();

        yield return new WaitForSeconds(stunDuration);

        stunned = false;
        OnStunned.Invoke();
    }
}
