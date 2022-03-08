using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Fusion;

public class MainSoundController : MonoBehaviour
{
    //Audio Clips
    [SerializeField] private AudioClip[] siphonAC;
    [SerializeField] private AudioClip[] shanty1;
    [SerializeField] private AudioClip[] shanty2;
    [SerializeField] private AudioClip[] shanty3;
    [SerializeField] private AudioClip[] shanty4;
    private Dictionary<int, AudioClip[]> shantyMap;

    //Audio Sources
    [SerializeField] private AudioSource siphonAS;
    [SerializeField] private AudioSource stunSound;
    [SerializeField] private AudioSource ghostLockOn;
    [SerializeField] private AudioSource coinSteal;
    [SerializeField] private AudioSource music;
    [SerializeField] private AudioSource playerDanger;
    [SerializeField] private AudioSource playerBeacon;
    [SerializeField] private AudioSource instructionOpen;

    [SerializeField] private GameSceneManager gameSceneManager;


    void Start()
    {
        shantyMap = new Dictionary<int, AudioClip[]>()
            {
                {0, shanty1 },
                {1, shanty2 },
                {2, shanty3 },
                {3, shanty4 }
            };
    }

    void Update()
    {
        if (music == null)
        {
            var playerObject = gameSceneManager.GetMyPlayerObject();
            if (playerObject == null) return;

            music = playerObject.GetComponentsInChildren<AudioSource>()
                .Where(source => source.name == "SoundtrackPlayer")
                .First();
        }
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

        StartCoroutine(PlayMusic(trackToPlay));
    }

    private IEnumerator PlayMusic(AudioClip[] audioClips)
    {
        this.music.clip = audioClips[0];
        this.music.Play();

        while (this.music.isPlaying)
        {
            yield return 1;
        }

        this.music.clip = audioClips[1];
        this.music.loop = true;
        this.music.Play();
    }

    public void StunSound()
    {
        this.stunSound.Play();
    }

    public void GhostLockOn()
    {
        this.ghostLockOn.Play();
    }

    public void CoinSteal()
    {
        this.coinSteal.Play();
    }

    public void PlayerInDanger()
    {
        this.playerDanger.Play();
    }

    public void PlayerIsBeacon()
    {
        this.playerBeacon.Play();
    }

    public void InstructionMenuOpen()
    {
        this.instructionOpen.Play();
    }

}