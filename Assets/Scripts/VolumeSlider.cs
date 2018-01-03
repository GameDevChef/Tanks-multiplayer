using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour {

    Slider m_slider;

    void Awake()
    {
        m_slider = GetComponent<Slider>();
    }

    public void ChangeVolume(float _volume)
    {
        AudioListener.volume = _volume;
        m_slider.value = _volume;
    } 
}
