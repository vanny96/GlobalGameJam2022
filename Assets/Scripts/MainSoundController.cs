using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MainSoundController : MonoBehaviour
{
    //Audio Clips
    [SerializeField] private AudioClip[] siphonAC;
    [SerializeField] private AudioClip[] shanty1;
    [SerializeField] private AudioClip[] shanty2;
    [SerializeField] private AudioClip[] shanty3;
   // [SerializeField] private shantyList;

    //Audio Sources
    [SerializeField] private AudioSource siphonAS;
    [SerializeField] private AudioSource ghostStun;
    [SerializeField] private AudioSource ghostLockOn;
    [SerializeField] private AudioSource coinSteal;
    [SerializeField] private AudioSource music;



    // Update is called once per frame
    void Update()
    {
        
    }

    public void SiphonSound()
    {
        int pickSiphonSound = Random.Range(0, siphonAC.Length);
        this.siphonAS.clip = siphonAC[pickSiphonSound];
        this.siphonAS.Play();
    }

    public void musicPlay()
    {
       // int pickTrackToPlay = Random.Range(shanty1, shanty2, shanty3);
    }

    public void GhostStun()
    {
        this.ghostStun.Play();
    }

    public void GhostLockOn()
    {
        this.ghostLockOn.Play();
    }

    public void CoinSteal()
    {
        this.coinSteal.Play();
    }
}
