using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsSlider : MonoBehaviour
{
    public AudioMixer mixer;
    public void SetLevel (float sliderValue)
    {
        mixer.SetFloat("SFX Volume", Mathf.Log10(sliderValue) * 20);
    }
}
