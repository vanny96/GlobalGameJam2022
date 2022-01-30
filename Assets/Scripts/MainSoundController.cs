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
    private Dictionary<int, AudioClip[]> shantyMap;

    //Audio Sources
    [SerializeField] private AudioSource siphonAS;
    [SerializeField] private AudioSource ghostStun;
    [SerializeField] private AudioSource ghostLockOn;
    [SerializeField] private AudioSource coinSteal;
    [SerializeField] private AudioSource music;


    void Start()
    {
        shantyMap = new Dictionary<int, AudioClip[]>()
            {
                {0, shanty1 },
                {1, shanty2 },
                {2, shanty3 }
            };
    }

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
        int pickTrackToPlay = Random.Range(0, 3);
        AudioClip[] trackToPlay = shantyMap[pickTrackToPlay];

    }

    public void GhostStun()
    {
        this.ghostStun.Play();
    }

    public void GhostLockOn()
    {
        Debug.Log("Play sounds");
        this.ghostLockOn.Play();
    }

    public void CoinSteal()
    {
        this.coinSteal.Play();
    }
}
