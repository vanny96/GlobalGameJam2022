using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public void SetLevel (float sliderValue)
    {
        mixer.SetFloat("Music Volume", Mathf.Log10(sliderValue) * 20);
    }
}
