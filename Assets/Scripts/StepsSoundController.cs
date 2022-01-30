using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class StepsSoundController : NetworkBehaviour
{
    //Audio clips
    [SerializeField] private AudioClip[] pirateFootStepsAC;

    //Audio sources
    [SerializeField] private AudioSource pirateFootStepsAS;

    //Vars
    private float nextFootStep;
    [SerializeField] private float footStepDelay;

    private int index;
    [Networked] private NetworkBool isWalking { get; set; } = false;

    // Update is called once per frame
    public override void Render()
    {
        if(this.isWalking)
        {
            this.PlaySteps();
        }
    }

    public void StartWalking()
    {
        this.isWalking = true;
        this.nextFootStep = 0.05f;
    }

    public void StopWalking()
    {
        this.isWalking = false;
    }

    private void PlaySteps()
    {
        this.nextFootStep -= Runner.DeltaTime;
        if (this.nextFootStep <= 0)
        {
            this.pirateFootStepsAS.PlayOneShot(pirateFootStepsAC[index]);
            this.index = (this.index + 1) % pirateFootStepsAC.Length;
            this.nextFootStep = this.footStepDelay;
        }
    }
}
