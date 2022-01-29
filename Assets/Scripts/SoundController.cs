using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    //Audio clips
    [SerializeField] private AudioClip[] pirateFootStepsAC;
    [SerializeField] private AudioClip[] musicAC;

    //Audio sources
    [SerializeField] private AudioSource pirateFootStepsAS;
    [SerializeField] private AudioSource musicAS;

    //Vars
    [SerializeField] private float nextFootStep;
    [SerializeField] private float footStepDelay;

    private int index;
    private bool isWalking = false;

    // Update is called once per frame
    void Update()
    {
      /*  if(this.isWalking = true)
        {
            this.PlaySteps(this.pirateFootStepsAC);
        }
      */
    }

    public void StartWalking()
    {
        this.isWalking = true;
    }

    public void StopWalking()
    {
        this.isWalking = false;
    }

    private void PlaySteps(AudioClip[] stepClips)
    {
        this.nextFootStep -= Time.deltaTime;
        if (this.nextFootStep <= 0)
        {
            this.pirateFootStepsAS.PlayOneShot(pirateFootStepsAC[index]);
            this.index = (this.index + 1) % pirateFootStepsAC.Length;
            this.nextFootStep += this.footStepDelay;
        }
    }



}
